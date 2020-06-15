using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
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
                Hashtable new_result = ConvertDictToHashtable((IDictionary)result);
                WriteObject(new_result);
                return (new_result != null);
            }
            else if (result is IList)
            {
                WriteVerbose("Found list: " + result.GetType().ToString());
                Array new_result = ConvertArrayElementsToHashtable((IList)result);
                WriteObject(new_result);
                return (new_result != null);
            }
            else
            {
                WriteVerbose("Unknown type: " + result.GetType().ToString());
            }

            WriteObject(result);
            return (result != null);
        }

        private Hashtable ConvertDictToHashtable(IDictionary dict)
        {
            Hashtable result = new Hashtable();

            foreach (var key in dict.Keys)
            {
                switch (dict[key])
                {
                    case IList val:
                        {
                            result.Add(
                                key,
                                ConvertArrayElementsToHashtable(val)
                            );
                            break;
                        }
                    case IDictionary val:
                        {
                            result.Add(
                                key,
                                ConvertDictToHashtable(val)
                            );
                            break;
                        }
                    case int val:
                        {
                            result.Add(key, val);
                            break;
                        }
                    case Boolean val:
                        {
                            result.Add(key, val);
                            break;
                        }
                    case string val:
                        {
                            result.Add(key, val);
                            break;
                        }
                }
            }

            return result;
        }

        private Array ConvertArrayElementsToHashtable(IList list)
        {
            var result = new object[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var element = list[i];

                switch (element)
                {
                    case IList val:
                        {
                            result[i] = ConvertArrayElementsToHashtable(val);
                            break;
                        }
                    case IDictionary val:
                        {
                            result[i] = ConvertDictToHashtable(val);
                            break;
                        }
                    case int val:
                        {
                            result[i] = val;
                            break;
                        }
                    case Boolean val:
                        {
                            result[i] = val;
                            break;
                        }
                    case string val:
                        {
                            result[i] = val;
                            break;
                        }
                }
            }

            return result;
        }
    }
}
