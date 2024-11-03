using System.Collections.Generic;
using System.Linq;

namespace Galacron
{
    public enum Pools
    {
        PLAYERBULLET,
    }

    public static class PoolIds
    {
        public const string PLAYERBULLET = "PlayerBullet";

        private static readonly Dictionary<string, string> _prefabNames = new Dictionary<string, string>
        {
            { "PlayerBullet", "PlayerBullet" },
        };

        public static string GetPrefabName(string poolId)
        {
            return _prefabNames.TryGetValue(poolId, out var name) ? name : null;
        }
    }

    public static class PoolIdConverter
    {
        private static readonly Dictionary<Pools, string> _poolIdCache = new Dictionary<Pools, string>();

        static PoolIdConverter()
        {
            InitializeCache();
        }

        public static string GetId(Pools pool)
        {
            return _poolIdCache.GetValueOrDefault(pool);
        }

        private static void InitializeCache()
        {
            var fields = typeof(PoolIds).GetFields();
            var constants = fields.ToDictionary(
                f => f.Name,
                f => (string)f.GetValue(null)
            );

            foreach (Pools pool in System.Enum.GetValues(typeof(Pools)))
            {
                string enumName = pool.ToString();
                if (constants.TryGetValue(enumName, out string id))
                {
                    _poolIdCache[pool] = id;
                }
            }
        }

        public static void RefreshCache()
        {
            _poolIdCache.Clear();
            InitializeCache();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            RefreshCache();
        }
#endif
    }
}
