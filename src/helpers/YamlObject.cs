using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Threading;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace psyml
{
    public static class YamlObject
    {
        public readonly struct ConvertFromYamlContext
        {
            public readonly Type OutputType;
            public readonly bool ScalarsAsStrings;
            public readonly PSCmdlet Cmdlet;

            public ConvertFromYamlContext(
                Type outputType,
                bool scalarsAsStrings,
                PSCmdlet cmdlet
            )
            {
                this.OutputType = outputType;
                this.ScalarsAsStrings = scalarsAsStrings;
                this.Cmdlet = cmdlet;
            }
        }

        public static object ConvertFromYaml(
            string input,
            ConvertFromYamlContext context
        )
        {
            var yaml = new YamlStream();
            yaml.Load(new StringReader(input));

            if (yaml.Documents.Count > 1)
            {
                var output = new Object[yaml.Documents.Count];
                for (int i = 0; i < yaml.Documents.Count; i++)
                {
                    output[i] = PopulateFromYamlNode(
                        yaml.Documents[i].RootNode, context
                    );
                }
                return output;
            }
            else
            {
                return PopulateFromYamlNode(
                    yaml.Documents[0].RootNode, context
                );
            }
        }

        private static object PopulateFromYamlNode(
            YamlNode node,
            ConvertFromYamlContext context
        )
        {
            switch (node)
            {
                case YamlMappingNode mapping:
                    return PopulateFromMappingNode(mapping, context);
                case YamlSequenceNode sequence:
                    return PopulateFromSequenceNode(sequence, context);
                case YamlScalarNode scalar:
                    return PopulateFromScalarNode(scalar, context);
                default:
                    return node;
            }
        }

        private static object PopulateFromScalarNode(
            YamlScalarNode scalar,
            ConvertFromYamlContext context
        )
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(
                CultureInfo.InvariantCulture.ToString()
            );

            if (String.IsNullOrEmpty(scalar.Value))
            {
                return null;
            }
            else if (!String.IsNullOrEmpty(scalar.Tag) && !context.ScalarsAsStrings)
            {
                switch (scalar.Tag)
                {
                    case "tag:yaml.org,2002:str":
                        return scalar.Value.ToString();
                    case "tag:yaml.org,2002:null":
                        return null;
                    case "tag:yaml.org,2002:bool":
                        return Boolean.Parse(scalar.Value.ToString());
                    case "tag:yaml.org,2002:int":
                        return int.Parse(scalar.Value.ToString());
                    case "tag:yaml.org,2002:float":
                        return float.Parse(scalar.Value.ToString());
                    case "tag:yaml.org,2002:timestamp":
                        return DateTime.Parse(scalar.Value.ToString());
                    default:
                        context.Cmdlet.WriteDebug(String.Format(
                            "Unhandled tag: {0}", scalar.Tag
                        ));
                        return scalar.Value.ToString();
                }
            }
            else if (
              context.ScalarsAsStrings ||
              scalar.Style.Equals(YamlDotNet.Core.ScalarStyle.SingleQuoted) ||
              scalar.Style.Equals(YamlDotNet.Core.ScalarStyle.DoubleQuoted)
          )
            {
                return scalar.Value.ToString();
            }
            else
            {
                var types = new List<Type>(){
                    typeof(Int32),
                    typeof(Int64),
                    typeof(Double),
                    typeof(Boolean)
                };

                var convertedValue = new object();
                foreach (var type in types)
                {
                    try
                    {
                        convertedValue = System.Convert.ChangeType(
                            scalar.Value, type
                        );
                        context.Cmdlet.WriteDebug(
                            String.Format(
                                "Casted value {0} to type {1}",
                                scalar.Value.ToString(),
                                type.ToString()
                            )
                        );
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (String.Equals(convertedValue.ToString().ToLower(), scalar.Value.ToString().ToLower()))
                {
                    return convertedValue;
                }
                else
                {
                    context.Cmdlet.WriteDebug(
                        String.Format(
                            "Value {0} got malformed after conversion: {1}",
                            scalar.Value.ToString(),
                            convertedValue.ToString()
                        )
                    );
                    return scalar.Value;
                }
            }
        }

        private static object PopulateFromMappingNode(
            YamlMappingNode mapping,
            ConvertFromYamlContext context
        )
        {
            switch (context.OutputType.Name)
            {
                case nameof(Hashtable):
                    return PopulateFromMappingNodeAsHashtable(mapping, context);
                case nameof(OrderedDictionary):
                    return PopulateFromMappingNodeAsOrderedDictionary(mapping, context);
                case nameof(PSObject):
                    return PopulateFromMappingNodeAsPSObject(mapping, context);
                default:
                    return mapping;
            }
        }

        private static Hashtable PopulateFromMappingNodeAsHashtable(
            YamlMappingNode mapping,
            ConvertFromYamlContext context
        )
        {
            var output = new Hashtable();

            foreach (var node in mapping)
            {
                output.Add(
                    PopulateFromYamlNode(node.Key, context),
                    PopulateFromYamlNode(node.Value, context)
                );
            }
            return output;
        }

        private static OrderedDictionary PopulateFromMappingNodeAsOrderedDictionary(
            YamlMappingNode mapping,
            ConvertFromYamlContext context
        )
        {
            var output = new OrderedDictionary();

            foreach (var node in mapping)
            {
                output.Add(
                    PopulateFromYamlNode(node.Key, context),
                    PopulateFromYamlNode(node.Value, context)
                );
            }
            return output;
        }

        private static PSObject PopulateFromMappingNodeAsPSObject(
            YamlMappingNode mapping,
            ConvertFromYamlContext context
        )
        {
            var output = new PSObject();

            foreach (var node in mapping)
            {
                output.Properties.Add(
                    new PSNoteProperty(
                        node.Key.ToString(),
                        PopulateFromYamlNode(node.Value, context)
                    )
                );
            }
            return output;
        }

        private static Array PopulateFromSequenceNode(
            YamlSequenceNode list,
            ConvertFromYamlContext context
        )
        {
            var output = new Object[list.Children.Count];
            for (int i = 0; i < list.Children.Count; i++)
            {
                output[i] = PopulateFromYamlNode(list[i], context);
            }
            return output;
        }

        public readonly struct ConvertToYamlContext
        {
            public readonly bool DisableAliases;
            public readonly bool JsonCompatible;
            public readonly int MaxRecursion;

            public ConvertToYamlContext(
                bool disableAliases = true,
                bool jsonCompatible = false,
                int maxRecursion = 1024
            )
            {
                this.DisableAliases = disableAliases;
                this.JsonCompatible = jsonCompatible;
                this.MaxRecursion = maxRecursion;
            }

            public ISerializer BuildSerializer()
            {
                var serializer = new SerializerBuilder()
                    .WithMaximumRecursion(this.MaxRecursion);

                if (this.DisableAliases)
                {
                    serializer.DisableAliases();
                }

                if (this.JsonCompatible)
                {
                    serializer.JsonCompatible();
                }
                else
                {
                    serializer.WithEventEmitter(nextEmitter => new TypeRespectingEmitter(nextEmitter));
                }


                return serializer.Build();
            }
        }

        public static string ConvertToYaml(
            object input,
            ConvertToYamlContext context
        )
        {
            input = PopulateFromObject(input);
            var serializer = context.BuildSerializer();

            return serializer.Serialize(input);
        }

        public static object PopulateFromObject(object input)
        {
            var pso = input as PSObject;

            if (
                pso != null
            )
            {
                input = pso.BaseObject;

                if (
                    input as IEnumerable == null &&
                    input as IDictionary == null
                ) {
                    // it might be pscustomobject
                    input = pso;
                }
            }

            switch (input)
            {
                case IDictionary dict:
                    return PopulateFromDictionary(dict);
                case PSObject obj:
                    return PopulateFromPSObject(obj);
                case IList list:
                    return PopulateFromList(list);
                default:
                    return input;
            }
        }

        public static Dictionary<string, object> PopulateFromDictionary(IDictionary input)
        {
            var output = new Dictionary<string, object>();

            foreach (var key in input.Keys)
            {
                output.Add(key.ToString(), PopulateFromObject(input[key]));
            }

            return output;
        }

        public static Dictionary<string, object> PopulateFromPSObject(PSObject input)
        {
            var output = new Dictionary<string, object>();

            foreach (var property in input.Properties)
            {
                output.Add(property.Name.ToString(), PopulateFromObject(property.Value));
            }

            return output;
        }

        public static List<object> PopulateFromList(IList list)
        {
            var output = new List<object>();

            foreach (var item in list)
            {
                output.Add(PopulateFromObject(item));
            }

            return output;
        }
    }
}