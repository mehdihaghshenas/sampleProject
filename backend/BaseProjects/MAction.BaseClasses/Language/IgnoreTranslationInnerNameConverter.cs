using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseClasses.Language
{
    public class IgnoreTranslationInnerNameConverter : JsonConverter<Translation>
    {
        public override Translation ReadJson(JsonReader reader, Type objectType, Translation existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string json = jo.ToString(Formatting.None);
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return new Translation() { Translate = dic };
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }
                var dic = JsonConvert.DeserializeObject<Translation>(json);
                return dic;
            }
        }


        public override void WriteJson(JsonWriter writer, Translation value, JsonSerializer serializer)
        {
            writer.WriteRawValue(JsonConvert.SerializeObject(value));
        }
    }
}
