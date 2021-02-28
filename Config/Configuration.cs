using System.IO;
using System.Net.Http.Json;
using Newtonsoft.Json;
using osu.API.Client.Auth;
using snipeyourself.Utils;

namespace snipeyourself {
    public class Configuration {
        public ClientCredentialsGrant Config { get; }
        public Configuration(string filename) {
            if (File.Exists(filename)) {
                Config = JsonConvert.DeserializeObject<ClientCredentialsGrant>(File.ReadAllText(filename));
            }
            else {
                Config = TUICfg.ClientCfg();
                File.WriteAllText(filename,JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
        }
    }
}