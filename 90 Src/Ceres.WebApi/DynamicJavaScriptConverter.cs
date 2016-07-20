using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Ceres.WebApi
{
    public class DynamicJsonConverter : JsonConverter
    {
        public override void WriteJson(
                JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dynamicObject = value as DynamicEntity;
            var propertyNames = dynamicObject.GetKeys();

            writer.WriteStartObject();

            foreach (var name in propertyNames)
            {
                var propertyValue = dynamicObject.GetMember(name);

                writer.WritePropertyName(name);
                writer.WriteValue(propertyValue);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(
            JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            var types = new[] { typeof(DynamicEntity) };
            return types.Any(t => t == objectType);
        }
    }
}