using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ks.Json.Standard.Converters
{
    /// <summary> VersionクラスのDeserializeをサポート(既存のDeserializeではバージョンが欠けている時にエラーが出るため) </summary> 
    internal class KsVersionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = value as Version;
            if (v == null)
                return;

            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Version.Major));
            writer.WriteValue(v.Major);
            writer.WritePropertyName(nameof(Version.Minor));
            writer.WriteValue(v.Minor);
            writer.WritePropertyName(nameof(Version.Build));
            writer.WriteValue(v.Build);
            writer.WritePropertyName(nameof(Version.Revision));
            writer.WriteValue(v.Revision);
            writer.WritePropertyName(nameof(Version.MajorRevision));
            writer.WriteValue(v.MajorRevision);
            writer.WritePropertyName(nameof(Version.MinorRevision));
            writer.WriteValue(v.MinorRevision);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            //TokenTypeは、StartObject → PropertyName → Interger →（繰り返し）→ EndObjectの順に来る
            //{"Major":1,"Minor":2,"Build":3,"Revision":4,"MajorRevision":0,"MinorRevision":4}
            if (reader.TokenType == JsonToken.StartObject)
            {
                var dictionary = new Dictionary<string, object>();
                string key = "";
                while (reader.TokenType != JsonToken.EndObject)
                {
                    reader.Read();

                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value != null)
                        {
                            key = (string)reader.Value;
                            dictionary.Add(key, null);
                        }
                        continue;
                    }
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        dictionary[key] = reader.Value;
                        continue;
                    }
                }
                return ToVersion(dictionary);
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
            => objectType == typeof(Version);

        private Version ToVersion(IDictionary<string, object> dictionary)
        {
            var major = GetInt(dictionary, nameof(Version.Major));
            var minor = GetInt(dictionary, nameof(Version.Minor));
            var build = GetInt(dictionary, nameof(Version.Build));
            var revision = GetInt(dictionary, nameof(Version.Revision));
            if (major < 0 || minor < 0)
                return new Version();
            if (build < 0)
                return new Version(major, minor);
            if (revision < 0)
                return new Version(major, minor, build);
            return new Version(major, minor, build, revision);
        }

        private int GetInt(IDictionary<string, object> dictionary, string name)
        {
            if (dictionary.TryGetValue(name, out var value) && value != null)
            {
                try
                {
                    return Convert.ToInt32(dictionary[name]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(KsVersionConverter)}.{nameof(GetInt)} Error {ex.Message}");
                }
            }
            return -1;
        }

        private static bool IsInvalid(int? value)
            => !value.HasValue || value.Value < 0;
    }
}
