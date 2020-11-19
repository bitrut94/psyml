using System;
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

        [Parameter()]
        public SwitchParameter AsOrderedDictionary { get; set; }

        [Parameter()]
        public SwitchParameter AsPSCustomObject { get; set; }

        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            if (_inputObjectBuffer.Count > 0)
            {
                WriteObject(
                    YamlObject.ConvertFromYaml(
                        string.Join(
                            System.Environment.NewLine,
                            _inputObjectBuffer.ToArray()
                        )
                    )
                );
            }
        }
    }
}
