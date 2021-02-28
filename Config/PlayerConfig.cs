using System.IO;
using Newtonsoft.Json;
using osu.API.Client.Auth;
using snipeyourself.Utils;

namespace snipeyourself {
    public class PlayerConfig {
        public PlayerInfo Player { get; set; }

        public PlayerConfig(string filename) {
            if (File.Exists(filename)) {
                Player = JsonConvert.DeserializeObject<PlayerInfo>(File.ReadAllText(filename));
            }
            else {
                Player = TUICfg.PlayerCfg();
                File.WriteAllText(filename,JsonConvert.SerializeObject(Player, Formatting.Indented));
            }
        }
    }
}