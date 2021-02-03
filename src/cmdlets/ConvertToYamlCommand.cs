using System.Collections.Generic;
using System.Management.Automation;

namespace psyml
{
    /// <summary>
    /// <para type="synopsis">
    ///     Converts an object to a YAML-formatted string.
    /// </para>
    /// <para type="description">
    ///     The ConvertTo-Yaml cmdlet converts (almost) any .NET object to a string in YAML format. The properties are converted to field names,
    ///     the field values are converted to property values, and the methods are removed. You can then use the ConvertFrom-Yaml cmdlet to convert
    ///     a YAML-formatted string to a YAML object, which is easily managed in PowerShell.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsData.ConvertTo, "Yaml")]
    public class ConvertToYamlCommand : PSCmdlet
    {
        /// <summary>
        /// <para type="description">
        ///     Specifies the objects to convert to YAML format. Enter a variable that contains the objects, or type a command or expression that gets
        ///     the objects. You can also pipe an object to ConvertTo-Yaml. The InputObject parameter is required, but its value can be null ($null)
        ///     or an empty string. When the input object is $null, ConvertTo-Yaml returns null in YAML notation. When the input object is an empty string,
        ///     ConvertTo-Yaml returns YAML document with empty string (this should be fixed).
        /// </para>
        /// </summary>
        [Alias("Data")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        [AllowNull]
        public object InputObject { get; set; }

        /// <summary>
        ///     InputObjectBuffer buffers all InputObject contents available in the pipeline.
        /// </summary>
        private readonly List<object> _inputObjectBuffer = new List<object>();

        /// <summary>
        /// <para type="description">
        ///     Converts object to JSON compatible YAML string.
        /// </para>
        /// </summary>
        [Parameter()]
        public SwitchParameter JsonCompatible { get; set; }

        /// <summary>
        /// <para type="description">
        ///     Enables YAML aliases on output string.
        /// </para>
        /// </summary>
        [Parameter()]
        public SwitchParameter EnableAliases { get; set; }

        /// <summary>
        /// <para type="description">
        ///     Forces the output to be array type.
        /// </para>
        /// </summary>
        [Parameter]
        public SwitchParameter AsArray { get; set; }

        /// <summary>
        ///     Buffers InputObjet contents available in the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(InputObject);
        }

        /// <summary>
        ///     The main execution method for the ConvertTo-Yaml command.
        /// </summary>
        protected override void EndProcessing()
        {
            if (_inputObjectBuffer.Count > 0)
            {
                object objectToProcess = (_inputObjectBuffer.Count > 1 || AsArray.IsPresent)
                ? (_inputObjectBuffer.ToArray() as object) : _inputObjectBuffer[0];

                ConvertToYamlHelper(objectToProcess);
            }
        }

        /// <summary>
        ///     ConvertToYamlHelper is a helper method to convert the .Net Type to Yaml string.
        /// </summary>
        /// <param name="input">Input string.</param>
        private void ConvertToYamlHelper(object input)
        {
            var context = new YamlObject.ConvertToYamlContext(
                disableAliases: !EnableAliases.IsPresent,
                jsonCompatible: JsonCompatible.IsPresent
            );

            object yaml = YamlObject.ConvertToYaml(input, context);

            WriteObject(yaml);
        }
    }
}
