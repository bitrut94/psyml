using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using YamlDotNet.Serialization;

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
                ConvertFromYamlHelper(
                    string.Join(
                        System.Environment.NewLine,
                        _inputObjectBuffer.ToArray()
                    )
                );
            }
        }

        private bool ConvertFromYamlHelper(string input)
        {
            var r = new StringReader(input);

            var deserializer = new DeserializerBuilder()
                .Build();

            object result = deserializer.Deserialize(r);

            WriteObject(result);
            return (result != null);
        }
    }
}
