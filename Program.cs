using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.API.Client;
using osu.API.Data.Enums;
using snipeyourself.Utils;

namespace snipeyourself
{
    class Program
    {
        static async Task Main(string[] args) {
            Configuration c = new($"{Directory.GetCurrentDirectory()}/client.json");
            PlayerConfig playerConfig = new($"{Directory.GetCurrentDirectory()}/player.json");
            using osuClient client = new osuClient(c.Config);
            MapCache cache = new();
            
            await client.ConnectAsync();
            var player = await client.GetUserRequest(playerConfig.Player.Id);
            Console.WriteLine($"Getting data for {player.Username}");
            Console.WriteLine($"Current Status : Rank #{player.RankHistory.Data.Last()} (#{player.Statistics.Rank.Country} {player.Country.Code}) | Avg ACC : {player.Statistics.HitAccuracy:F2}%");
            var threshold = Math.Floor(player.Statistics.HitAccuracy);
            Console.WriteLine($"Using floor(avg acc) as cutoff point for accuracy. X = {threshold}%");
            Console.WriteLine($"Getting top 100 scores...");
            var top50 = await client.GetUserScoresRequest(playerConfig.Player.Id, ScoreType.BEST,limit:50);
            var _51to100 = await client.GetUserScoresRequest(playerConfig.Player.Id, ScoreType.BEST, limit:50, offset:50);
            var top100 = top50.Concat(_51to100).ToList();
            Console.WriteLine($"Got top 100. Filtering data...");
            var lowerThenTheshold = top100.Where(x => x.Accuracy < threshold / 100).ToList();
            Console.WriteLine("Populating the cache...");
            await cache.PopulateCacheAsync(lowerThenTheshold.Select(x => x.Beatmap.Id).ToList(), client);
            Console.WriteLine("Done.");
            Console.WriteLine($"From 100 scores : {lowerThenTheshold.Count} were below {threshold}% of accuracy");
            var aRank = lowerThenTheshold.Where(x => x.Grade == ScoreGrade.A).ToList();
            var bOrLower = lowerThenTheshold.Where(x => x.Grade <= ScoreGrade.B).ToList();
            var sRank = lowerThenTheshold.Where(x => x.Grade == ScoreGrade.S || x.Grade == ScoreGrade.SH).ToList();
            Console.WriteLine($"From those, based on rank gathered.");
            Console.WriteLine($"{sRank.Count} were S/S+ Rank.");
            Console.WriteLine($"{aRank.Count} were A Rank.");
            Console.WriteLine($"{bOrLower.Count} were B or Lower.");
            var orderedS = sRank.OrderRanks(threshold,cache.CACHE);
            var orderedA = aRank.OrderRanks(threshold,cache.CACHE);
            var orderedBMinus = bOrLower.OrderRanks(threshold,cache.CACHE);
            SaveScores(orderedS,orderedA, orderedBMinus);
            Console.WriteLine($"Scores exported to scores/rank.json ordered on Rebase v1");
            Console.WriteLine($"LOWER IS BETTER. The smaller the number the easier it should be to replicate said score.");
        }

        private static void SaveScores(List<OrderedScore> orderedS, List<OrderedScore> orderedA, List<OrderedScore> orderedBMinus) {
            AssertCleanFolder($"{Directory.GetCurrentDirectory()}/scores");
            File.WriteAllText($"{Directory.GetCurrentDirectory()}/scores/s.json", JsonConvert.SerializeObject(orderedS, Formatting.Indented));
            File.WriteAllText($"{Directory.GetCurrentDirectory()}/scores/a.json", JsonConvert.SerializeObject(orderedA, Formatting.Indented));
            File.WriteAllText($"{Directory.GetCurrentDirectory()}/scores/b_minus.json", JsonConvert.SerializeObject(orderedBMinus, Formatting.Indented));
        }

        /// <summary>
        /// Tis rm -rf. Use with caution, this shit has no safety whatsoever.
        /// </summary>
        /// <param name="folderDir"></param>
        public static void RecursiveDelete(string folderDir) {
            DirectoryInfo folder = new(folderDir);
            foreach(var subfolder in folder.GetDirectories()) {
                RecursiveDelete(subfolder.FullName);
            }

            foreach(var file in folder.GetFiles()) {
                file.Delete();
            }

            folder.Delete();
        }

        public static void AssertCleanFolder(string folderDir) {
            DirectoryInfo folder = new(folderDir);

            if (!folder.Exists) {
                folder.Create();
                return;
            }

            RecursiveDelete(folderDir);
            folder.Create();
        }
    }
}
