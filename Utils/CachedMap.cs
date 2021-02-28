using LiteDB;

namespace snipeyourself.Utils {
    public class CachedMap {
        [BsonId] public ulong Id { get; set; }
        public uint Combo { get; set; }
    }
}