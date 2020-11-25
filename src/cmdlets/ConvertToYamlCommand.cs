using System.Collections.Generic;
using System.Management.Automation;

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

        private List<object> _inputObjectBuffer;

        [Parameter()]
        public SwitchParameter JsonCompatible { get; set; }

        [Parameter()]
        public SwitchParameter EnableAliases { get; set; }

        protected override void BeginProcessing()
        {
            _inputObjectBuffer = new List<object>();
        }

        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            if (_inputObjectBuffer.Count > 0)
            {
                object objectToProcess = (_inputObjectBuffer.Count > 1)
                ? (_inputObjectBuffer.ToArray() as object) : _inputObjectBuffer[0];

                ConvertToYamlHelper(objectToProcess);
            }
        }

        private void ConvertToYamlHelper(object input)
        {
            var context = new YamlObject.ConvertToYamlContext(
                disableAliases: EnableAliases,
                jsonCompatible: JsonCompatible
            );

            object yaml = YamlObject.ConvertToYaml(input, context);

            WriteObject(yaml);
        }
    }
}
