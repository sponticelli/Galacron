using System.Collections.Generic;
using System.Linq;

namespace Galacron
{
    public enum Pools
    {
        ENEMYWASP = 0,
        FIREFLYBULLET = 1,
        FX_EXPLOSION = 2,
        PATHDIVE1 = 3,
        PATHDIVE2 = 4,
        PATHINLEFT = 5,
        PATHINLEFT2 = 6,
        PATHINRIGHT = 7,
        PATHINRIGHT2 = 8,
        PLAYERBULLET = 9,
    }

    public static class PoolIds
    {
        public static class Poolingserviceconfig
        {
            public const string ENEMYWASP = "EnemyWasp";
            public const string FIREFLYBULLET = "FireflyBullet";
            public const string FX_EXPLOSION = "FX_Explosion";
            public const string PATHDIVE1 = "PathDive1";
            public const string PATHDIVE2 = "PathDive2";
            public const string PATHINLEFT = "PathInLeft";
            public const string PATHINLEFT2 = "PathInLeft2";
            public const string PATHINRIGHT = "PathInRight";
            public const string PATHINRIGHT2 = "PathInRight2";
            public const string PLAYERBULLET = "PlayerBullet";
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
            var constants = new Dictionary<string, string>();

            // Get nested type classes (configs)
            var configClasses = typeof(PoolIds).GetNestedTypes();
            foreach (var configClass in configClasses)
            {
                // Get all constant fields in this config class
                var fields = configClass.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly);

                foreach (var field in fields)
                {
                    constants[field.Name] = (string)field.GetValue(null);
                }
            }

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
