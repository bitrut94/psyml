using System.Collections.Generic;
using System.Management.Automation;
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

        [Parameter()]
        public SwitchParameter JsonCompatible { get; set; }

        [Parameter()]
        public SwitchParameter EnableAliases { get; set; }

        [Parameter()]
        public SwitchParameter EnableTags { get; set; }

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

                SerializerSettings settings = new SerializerSettings()
                {
                    EmitAlias = EnableAliases.IsPresent,
                    EmitTags = EnableTags.IsPresent,
                    EmitShortTypeName = true,
                    EmitJsonComptible = JsonCompatible.IsPresent,
                };

                WriteObject(
                    YamlObject.ConvertToYaml(objectToProcess, settings)
                );
            }
        }
    }
}
