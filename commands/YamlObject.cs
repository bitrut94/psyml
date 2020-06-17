using System;
using System.Collections;
using System.Collections.Specialized;
using SharpYaml.Serialization;

namespace psyml
{
    public static class YamlObject
    {
        public static object ConvertFromYaml(string input, Type outputType)
        {
            object result = new Serializer()
                .Deserialize(input);

            if (result is IDictionary)
            {
                if (outputType.Equals(typeof(OrderedDictionary)))
                {
                    return YamlObject.PopulateOrderedDictionaryFromDictionary((IDictionary)result);
                }
                else
                {
                    return YamlObject.PopulateHashtableFromDictionary((IDictionary)result);
                }
            }
            else if (result is IList)
            {
                if (outputType.Equals(typeof(OrderedDictionary)))
                {
                    return YamlObject.PopulateOrderedDictionaryFromArray((IList)result);
                }
                else
                {
                    return YamlObject.PopulateHashtableFromList((IList)result);
                }
            }

            return (result != null);
        }
        public static Hashtable PopulateHashtableFromDictionary(IDictionary dict)
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
                                PopulateHashtableFromList(val)
                            );
                            break;
                        }
                    case IDictionary val:
                        {
                            result.Add(
                                key,
                                PopulateHashtableFromDictionary(val)
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

        public static Array PopulateHashtableFromList(IList list)
        {
            var result = new object[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var element = list[i];

                switch (element)
                {
                    case IList val:
                        {
                            result[i] = PopulateHashtableFromList(val);
                            break;
                        }
                    case IDictionary val:
                        {
                            result[i] = PopulateHashtableFromDictionary(val);
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

        public static OrderedDictionary PopulateOrderedDictionaryFromDictionary(IDictionary dict)
        {
            OrderedDictionary result = new OrderedDictionary();

            foreach (var key in dict.Keys)
            {
                switch (dict[key])
                {
                    case IList val:
                        {
                            result.Add(
                                key,
                                PopulateOrderedDictionaryFromArray(val)
                            );
                            break;
                        }
                    case IDictionary val:
                        {
                            result.Add(
                                key,
                                PopulateOrderedDictionaryFromDictionary(val)
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

        public static Array PopulateOrderedDictionaryFromArray(IList list)
        {
            var result = new object[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var element = list[i];

                switch (element)
                {
                    case IList val:
                        {
                            result[i] = PopulateOrderedDictionaryFromArray(val);
                            break;
                        }
                    case IDictionary val:
                        {
                            result[i] = PopulateOrderedDictionaryFromDictionary(val);
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