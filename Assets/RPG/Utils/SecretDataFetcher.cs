using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Utils {
    public class SecretDataFetcher {
        public static JProperty FetchProperty(string property) {
            string path = Application.persistentDataPath + "secret_data.json";
            if (!File.Exists(path)) {
                return null;
            }
            
            using (var textReader = File.OpenText(path)) {
                using (var reader = new JsonTextReader(textReader)) {
                    reader.FloatParseHandling = FloatParseHandling.Double;
                    var jObject = JObject.Load(reader);
                    return jObject.Property(property);
                }
            }
        }
    }
}
