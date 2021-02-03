// Based on JsonEventEmitter from YamlDotNet library by aaubry
// https://github.com/aaubry/YamlDotNet

using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace psyml
{
    internal sealed class TypeRespectingEmitter : ChainedEventEmitter
    {
        public TypeRespectingEmitter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            eventInfo.IsPlainImplicit = true;
            eventInfo.Style = ScalarStyle.Plain;

            var value = eventInfo.Source.Value;
            if (value == null)
            {
                eventInfo.RenderedValue = "null";
            }
            else
            {
                var typeCode = Type.GetTypeCode(eventInfo.Source.Type);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        eventInfo.RenderedValue = YamlFormatter.FormatBoolean(value);
                        break;

                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        var valueIsEnum = eventInfo.Source.Type.IsEnum;
                        if (valueIsEnum)
                        {
                            eventInfo.RenderedValue = value.ToString()!;
                            if (CouldValueBeMissconverted(value.ToString()))
                            {
                                eventInfo.Style = ScalarStyle.DoubleQuoted;
                            }
                            break;
                        }

                        eventInfo.RenderedValue = YamlFormatter.FormatNumber(value);
                        break;

                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        eventInfo.RenderedValue = YamlFormatter.FormatNumber(value);
                        break;

                    case TypeCode.String:
                    case TypeCode.Char:
                        eventInfo.RenderedValue = value.ToString()!;
                        if (((String)value).Contains(Environment.NewLine))
                        {
                            eventInfo.Style = ScalarStyle.Literal;
                        } else if (CouldValueBeMissconverted(value.ToString()))
                        {
                            eventInfo.Style = ScalarStyle.DoubleQuoted;
                        }
                        break;

                    case TypeCode.DateTime:
                        eventInfo.RenderedValue = YamlFormatter.FormatDateTime(value);
                        break;

                    case TypeCode.Empty:
                        eventInfo.RenderedValue = "null";
                        break;

                    default:
                        if (eventInfo.Source.Type == typeof(TimeSpan))
                        {
                            eventInfo.RenderedValue = YamlFormatter.FormatTimeSpan(value);
                            break;
                        }

                        throw new NotSupportedException($"TypeCode.{typeCode} is not supported.");
                }
            }

            emitter.Emit(new YamlDotNet.Core.Events.Scalar(
                anchor: eventInfo.Anchor,
                tag: eventInfo.Tag,
                value: eventInfo.RenderedValue,
                style: eventInfo.Style,
                isPlainImplicit: eventInfo.IsPlainImplicit,
                isQuotedImplicit: eventInfo.IsQuotedImplicit
            ));
        }

        private static bool CouldValueBeMissconverted(string value)
        {
            var types = new List<Type>(){
                typeof(Int32),
                typeof(Int64),
                typeof(Double),
                typeof(Boolean),
                typeof(DateTime)
            };

            if (String.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            foreach (var type in types)
            {
                try
                {
                    var converted = System.Convert.ChangeType(
                        value, type, CultureInfo.InvariantCulture
                    );
                    return true;
                }
                catch
                {
                    continue;
                }
            }

            return false;
        }
    }

    internal static class YamlFormatter
    {
        private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo
        {
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = "_",
            CurrencyGroupSizes = new[] { 3 },
            CurrencySymbol = string.Empty,
            CurrencyDecimalDigits = 99,
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = "_",
            NumberGroupSizes = new[] { 3 },
            NumberDecimalDigits = 99,
            NaNSymbol = ".nan",
            PositiveInfinitySymbol = ".inf",
            NegativeInfinitySymbol = "-.inf"
        };

        public static string FormatNumber(object number)
        {
            return Convert.ToString(number, NumberFormat)!;
        }

        public static string FormatNumber(double number)
        {
            return number.ToString("G17", NumberFormat);
        }

        public static string FormatNumber(float number)
        {
            return number.ToString("G17", NumberFormat);
        }

        public static string FormatBoolean(object boolean)
        {
            return boolean.Equals(true) ? "true" : "false";
        }

        public static string FormatDateTime(object dateTime)
        {
            return ((DateTime)dateTime).ToString("o", CultureInfo.InvariantCulture);
        }

        public static string FormatTimeSpan(object timeSpan)
        {
            return ((TimeSpan)timeSpan).ToString();
        }
    }
}
