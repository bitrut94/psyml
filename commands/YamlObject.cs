using System;
using System.Collections;
using System.IO;
using YamlDotNet.RepresentationModel;
namespace psyml
{
    public static class YamlObject
    {
        public static object ConvertFromYaml(string input)
        {
            var yaml = new YamlStream();
            yaml.Load(new StringReader(input));

            return PopulateFromYamlNode(yaml.Documents[0].RootNode);
        }

        private static object PopulateFromYamlNode(YamlNode node)
        {
            switch (node)
            {
                case YamlMappingNode mapping:
                    return PopulateFromMappingNode(mapping);
                case YamlSequenceNode sequence:
                    return PopulateFromSequenceNode(sequence);
                case YamlScalarNode scalar:
                    return PopulateFromScalarNode(scalar);
                default:
                    return node;
            }
        }

        private static object PopulateFromScalarNode(YamlScalarNode scalar)
        {
            return scalar.Value.ToString();
        }

        private static Hashtable PopulateFromMappingNode(YamlMappingNode mapping)
        {
            var output = new Hashtable();

            foreach (var node in mapping)
            {
                output.Add(node.Key.ToString(), PopulateFromYamlNode(node.Value));
            }
            return output;
        }

        private static Array PopulateFromSequenceNode(YamlSequenceNode list)
        {
            var output = new Object[list.Children.Count];
            for (int i = 0; i < list.Children.Count; i++)
            {
                output[i] = PopulateFromYamlNode(list[i]);
            }
            return output;
        }
    }
}