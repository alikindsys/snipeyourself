using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using osu.API.Data;
using osu.API.Data.Enums;

namespace snipeyourself.Utils {
    public static class RebaseV1 {
        public static List<OrderedScore> OrderRanks(this IEnumerable<Score> scores, double threshold , ILiteCollection<CachedMap> CACHE) {
            return scores.OrderBy(x => GetRebaseV1(x, threshold, CACHE)).Select(
                x => {
                    var ord = new OrderedScore(GetRebaseV1(x, threshold, CACHE),
                        x.Beatmap.Url,
                        $"{x.Beatmapset.Artist} - {x.Beatmapset.Title}",
                        x.Beatmap.Version,
                        x.Accuracy);

                    ord.Grade = x.Grade;

                    if (GetCombo(x.Beatmap.Id, CACHE) != (uint) x.MaxCombo) {
                        ord.MaxCombo = GetCombo(x.Beatmap.Id, CACHE);
                        ord.Combo = (uint) x.MaxCombo;
                    }
                    
                    return ord;
                }).ToList();
        }

        public static double GetRebaseV1(this Score score, double threshold, ILiteCollection<CachedMap> CACHE) {
            var mapCombo =  GetCombo(score.Beatmap.Id, CACHE);
            var comboPercent = (double) score.MaxCombo / mapCombo;
            var misscount = score.Statistics.CountMiss;
            var count50 = score.Statistics.Count50;
            var count100 = score.Statistics.Count100;
            var count300 = score.Statistics.Count300;
            var badAcc = misscount + (count50 * 8) + (count100 * 2)  * ((double)count300/ mapCombo);
            var accDiff = threshold - (score.Accuracy*100);
            return (accDiff * 1.2) * comboPercent / badAcc;
        }

        private static uint GetCombo(uint beatmapId, ILiteCollection<CachedMap> CACHE) {
            if (!CACHE.Exists(x => x.Id == beatmapId)) return 1;

            return CACHE.FindOne(x => x.Id == beatmapId).Combo;
        }
    }
}