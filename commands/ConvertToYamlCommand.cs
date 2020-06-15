using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using SharpYaml.Serialization;

namespace psyml
{
    [Cmdlet(VerbsData.ConvertTo, "Yaml")]
    public class ConvertToYamlCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        public object InputObject { get; set; }

        private List<object> _inputObjectBuffer = new List<object>();

        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            if (_inputObjectBuffer.Count > 0)
            {
                ConvertToYamlHelper(_inputObjectBuffer[0]);
            }
        }

        private bool ConvertToYamlHelper(object input)
        {
            string result = new Serializer().Serialize(input);

            Console.WriteLine(result);

            WriteObject(result);
            return (result != null);
        }
    }
}
