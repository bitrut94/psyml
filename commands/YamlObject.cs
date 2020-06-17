using System;
using System.Collections;
using System.Collections.Specialized;

namespace psyml
{
    public static class YamlObject
    {
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