
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using osu.API.Data.Enums;

namespace snipeyourself.Utils {
    public class OrderedScore {
        public OrderedScore(double rebase, string beatmapLink, string name, string difficulty, double acc) {
            Rebase = rebase;
            BeatmapLink = beatmapLink;
            Name = name;
            Difficulty = difficulty;
            Acc = acc;
        }

        public double Rebase { get; set; }
        public string BeatmapLink { get; set; }
        public string Name { get; set; }
        public string Difficulty { get; set; }
        public double Acc { get; set; }
        public uint MaxCombo { get; set; }
        public uint Combo { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ScoreGrade Grade { get; set; }

        public bool ShouldSerializeMaxCombo() {
            return MaxCombo != 0;
        }
        public bool ShouldSerializeCombo() {
            return Combo != 0;
        }
        public bool ShouldSerializeGrade() {
            return Grade <= ScoreGrade.B;
        }
    }
}