using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using osu.API.Client;

namespace snipeyourself.Utils {
    public class MapCache {
        private LiteDatabase Db => new LiteDatabase($"FileName={Directory.GetCurrentDirectory()}/mapcache.db; Connection=Shared;");
        public ILiteCollection<CachedMap> CACHE => Db.GetCollection<CachedMap>("maps");
        public async Task PopulateCacheAsync(IEnumerable<uint> mapIds, osuClient client) {
            foreach (var mapId in mapIds.Where(mapId => !CACHE.Exists(x => x.Id == mapId))) {
                var map = await client.GetBeatmapRequest(mapId);
                if (map.MaxCombo.HasValue) {
                    CachedMap mapx = new();
                    mapx.Combo = map.MaxCombo.Value;
                    mapx.Id = map.Id;
                    CACHE.Insert(mapx);
                }
                else {
                    Console.WriteLine($"ERROR ON MAPID  #{mapId}, Request didn't return max combo.");
                }
            }
        }
    }
}