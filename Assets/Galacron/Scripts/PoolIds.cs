using System.Collections.Generic;
using System.Linq;

namespace Galacron
{
    public enum Pools
    {
        ENEMYWASP,
        FIREFLYBULLET,
        PATHINLEFT,
        PATHINLEFT2,
        PATHINRIGHT,
        PATHINRIGHT2,
        PLAYERBULLET,
        PATHDIVE1,
        PATHDIVE2
    }

    public static class PoolIds
    {
        public const string ENEMYWASP = "EnemyWasp";
        public const string FIREFLYBULLET = "FireflyBullet";
        public const string PATHINLEFT = "PathInLeft";
        public const string PATHINLEFT2 = "PathInLeft2";
        public const string PATHINRIGHT = "PathInRight";
        public const string PATHINRIGHT2 = "PathInRight2";
        public const string PLAYERBULLET = "PlayerBullet";
        public const string PATHDIVE1 = "PathDive1";
        public const string PATHDIVE2 = "PathDive2";

        private static readonly Dictionary<string, string> _prefabNames = new Dictionary<string, string>
        {
            { "EnemyWasp", "EnemyWasp" },
            { "FireflyBullet", "FireflyBullet" },
            { "PathInLeft", "PathInLeft" },
            { "PathInLeft2", "PathInLeft2" },
            { "PathInRight", "PathInRight" },
            { "PathInRight2", "PathInRight2" },
            { "PlayerBullet", "PlayerBullet" },
            { "PathDive1", "PathDive1" },
            { "PathDive2", "PathDive2" }
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