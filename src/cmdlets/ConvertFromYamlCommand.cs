using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;

namespace psyml
{
    [Cmdlet(VerbsData.ConvertFrom, "Yaml")]
    public class ConvertFromYamlCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        public string InputObject { get; set; }

        private List<string> _inputObjectBuffer = new List<string>();

        [Parameter()]
        public SwitchParameter AsHashtable { get; set; }

        [Alias("Ordered")]
        [Parameter()]
        public SwitchParameter AsOrderedDictionary { get; set; }

        [Parameter()]
        public SwitchParameter NoEnumerate { get; set; }

        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(InputObject);
        }

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
                    else {
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
