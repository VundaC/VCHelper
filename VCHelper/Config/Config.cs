using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Config
{
    public class Config
    {
        public static Config Instance { get; set; }

        [JsonProperty("pixabay_key")]
        public string PixabayKey { get; set; }

        public static void Load()
        {
            var assembly = typeof(Config).Assembly;

            var resource = assembly.GetManifestResourceNames();
            using var mStream = assembly.GetManifestResourceStream("Config.Config.json");
            using var reader = new StreamReader(mStream);
            var json = reader.ReadToEnd();
            Instance = JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
