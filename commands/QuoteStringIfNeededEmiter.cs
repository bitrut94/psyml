using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace psyml
{
    public class QuoteStringIfNeededEmiter : ChainedEventEmitter
    {
        public QuoteStringIfNeededEmiter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, YamlDotNet.Core.IEmitter emitter)
        {
            if (eventInfo.Source.Type == typeof(string) && CouldValueBeMissconverted(eventInfo.Source.Value.ToString()))
            {
                emitter.Emit(new YamlDotNet.Core.Events.Scalar(
                    anchor: null,
                    tag: null,
                    value: eventInfo.Source.Value.ToString().Replace('"','\"'),
                    style: ScalarStyle.DoubleQuoted,
                    isPlainImplicit: true, // ?
                    isQuotedImplicit: true // ?
                ));
            }
            else
            {
                base.Emit(eventInfo, emitter);
            }
        }

        // needs to be refactored
        private bool CouldValueBeMissconverted(string value) {
            if (
                value.ToLower() == "null" ||
                value.ToLower() == "true" ||
                value.ToLower() == "false"
            ) {
                return true;
            }

            try
            {
                Convert.ToInt64(value);
                return true;
            }
            catch {}

            try
            {
                float.Parse(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                return true;
            }
            catch
            {}

            try
            {
                Convert.ToDateTime(value);
                return true;
            }
            catch
            {}

            return false;
        }
    }
}
