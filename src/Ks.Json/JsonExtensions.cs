using Ks.Json.Standard.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ks.Json
{
    public static class JsonExtensions
    {
        private static readonly KsVersionConverter _versionConverter;

        static JsonExtensions()
        {
            _versionConverter = new KsVersionConverter();
        }

        public static T ToObject<T>(this string json)
            => JsonConvert.DeserializeObject<T>(json, _versionConverter);

        public static string ToJson(this object obj)
            => JsonConvert.SerializeObject(obj, _versionConverter);

        public static string ToJson(this object obj, bool ignoreNullProperty)
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = ignoreNullProperty ? NullValueHandling.Ignore : NullValueHandling.Include,
                Converters = new List<JsonConverter> { _versionConverter }
            };
            return JsonConvert.SerializeObject(obj, setting);
        }
    }
}
