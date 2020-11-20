using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Management.Automation;
using YamlDotNet.RepresentationModel;
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

            return PopulateFromYamlNode(yaml.Documents[0].RootNode, context);
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
            return scalar.Value.ToString();
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
                output.Add(node.Key.ToString(), PopulateFromYamlNode(node.Value, context));
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
                output.Add(node.Key.ToString(), PopulateFromYamlNode(node.Value, context));
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
    }
}