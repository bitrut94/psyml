using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;
using SharpYaml.Serialization;

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
            object result = new Serializer()
                .Deserialize(input);

            if (result is IDictionary)
            {
                WriteVerbose("Found object: " + result.GetType().ToString());
                if (AsOrderedDictionary.IsPresent) {
                    OrderedDictionary new_result = YamlObject.PopulateOrderedDictionaryFromDictionary((IDictionary)result);
                    WriteObject(new_result);
                    return (new_result != null);
                } else {
                    Hashtable new_result = YamlObject.PopulateHashtableFromDictionary((IDictionary)result);
                    WriteObject(new_result);
                    return (new_result != null);
                }
            }
            else if (result is IList)
            {
                WriteVerbose("Found list: " + result.GetType().ToString());
                if (AsOrderedDictionary.IsPresent) {
                    Array new_result = YamlObject.PopulateOrderedDictionaryFromArray((IList)result);
                    WriteObject(new_result);
                    return (new_result != null);
                } else {
                    Array new_result = YamlObject.PopulateHashtableFromList((IList)result);
                    WriteObject(new_result);
                    return (new_result != null);
                }
            }
            else
            {
                WriteVerbose("Unknown type: " + result.GetType().ToString());
            }

            WriteObject(result);
            return (result != null);
        }
    }
}
