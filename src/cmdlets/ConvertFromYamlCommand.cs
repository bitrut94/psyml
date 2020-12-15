using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;

namespace psyml
{
    /// <summary>
    /// <para type="synopsis">
    ///     Converts a YAML-formatted string to a custom object, hash table or ordered dictionary.
    /// </para>
    /// <para type="description">
    ///     The ConvertFrom-Yaml cmdlet converts a YAML formatted string to a custom PSCustomObject object that has a property for each field in the YAML string.
    ///     YAML is commonly used for configuration files and in applications where data is being stored or transmitted.
    /// </para>
    /// </summary>
    /// <example>
    ///     <para>Convert a DateTime object to a YAML object</para>
    ///     <para>
    ///         This command uses the ConvertTo-Yaml and ConvertFrom-Yaml cmdlets to convert a DateTime object from the Get-Date cmdlet to a Yaml object then to a PSCustomObject.
    ///     </para>
    ///     <code>
    ///         PS > Get-Date | Select-Object -Property * | ConvertTo-Yaml | ConvertFrom-Yaml
    ///
    ///         DisplayHint : DateTime
    ///         DateTime    : poniedziałek, 14 grudnia 2020 22:10:03
    ///         Date        : 2020-12-14T00:00:00.0000000+01:00
    ///         Day         : 14
    ///         DayOfWeek   : Monday
    ///         DayOfYear   : 349
    ///         Hour        : 22
    ///         Kind        : Local
    ///         Millisecond : 418
    ///         Minute      : 10
    ///         Month       : 12
    ///         Second      : 3
    ///         Ticks       : 637435806034183959
    ///         TimeOfDay   : 22:10:03.4183959
    ///         Year        : 2020
    ///     </code>
    ///     <para>
    ///         The example uses the Select-Object cmdlet to get all of the properties of the DateTime object. It uses the ConvertTo-Yaml cmdlet to convert the DateTime object
    ///         to a string formatted as a YAML object and the ConvertFrom-Yaml cmdlet to convert the YAML-formatted string to a PSCustomObject object.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsData.ConvertFrom, "Yaml")]
    public class ConvertFromYamlCommand : PSCmdlet
    {
        /// <summary>
        /// <para type="description">
        ///     Specifies the YAML strings to convert to YAML objects. Enter a variable that contains the string, or type a command or expression that gets the string.
        ///     You can also pipe a string to ConvertFrom-Yaml. The InputObject parameter is required, but its value can be an empty string. When the input object is
        ///     an empty string, ConvertFrom-Yaml does not generate any output. The InputObject value cannot be $null.
        /// </para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        [AllowEmptyString]
        public string InputObject { get; set; }

        /// <summary>
        ///     InputObjectBuffer buffers all InputObject contents available in the pipeline.
        /// </summary>
        private readonly List<string> _inputObjectBuffer = new List<string>();

        /// <summary>
        /// <para type="description">
        ///     Converts the YAML to a hash table object.
        /// </para>
        /// </summary>
        [Parameter()]
        public SwitchParameter AsHashtable { get; set; }

        /// <summary>
        /// <para type="description">
        ///     Converts the YAML to a ordered dictionary object.
        /// </para>
        /// </summary>
        [Alias("Ordered")]
        [Parameter()]
        public SwitchParameter AsOrderedDictionary { get; set; }

        /// <summary>
        /// <para type="description">
        ///     Specifies that output is not enumerated.
        ///     Setting this parameter causes arrays to be sent as a single object instead of sending every element separately.
        ///     This guarantees that YAML can be round-tripped via ConvertTo-Yaml.
        /// </para>
        /// </summary>
        [Parameter()]
        public SwitchParameter NoEnumerate { get; set; }

        /// <summary>
        ///     Buffers InputObjet contents available in the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(InputObject);
        }

        /// <summary>
        ///     The main execution method for the ConvertFrom-Yaml command.
        /// </summary>
        protected override void EndProcessing()
        {
            if (_inputObjectBuffer.Count > 0)
            {
                if (_inputObjectBuffer.Count == 1)
                {
                    ConvertFromYamlHelper(_inputObjectBuffer[0]);
                }
                else
                {
                    bool successfullyConverted = true;
                    try
                    {
                        ConvertFromYamlHelper(_inputObjectBuffer[0]);
                    }
                    catch
                    {
                        successfullyConverted = false;
                    }

                    if (successfullyConverted)
                    {
                        for (int i = 1; i < _inputObjectBuffer.Count; i++)
                        {
                            ConvertFromYamlHelper(_inputObjectBuffer[i]);
                        }
                    }
                    else
                    {
                        ConvertFromYamlHelper(
                            string.Join(
                                System.Environment.NewLine,
                                _inputObjectBuffer.ToArray()
                            )
                        );
                    }
                }
            }
        }

        /// <summary>
        ///     ConvertFromYamlHelper is a helper method to convert the Yaml input to .Net Type.
        /// </summary>
        /// <param name="input">Input string.</param>
        private void ConvertFromYamlHelper(string input)
        {
            var outputType = typeof(PSObject);
            if (AsHashtable)
            {
                outputType = typeof(Hashtable);
            }
            else if (AsOrderedDictionary)
            {
                outputType = typeof(OrderedDictionary);
            }

            var contex = new YamlObject.ConvertFromYamlContext(
                outputType,
                false,
                this
            );

            object result = YamlObject.ConvertFromYaml(input, contex);

            WriteObject(result, !NoEnumerate.IsPresent);
        }
    }
}
