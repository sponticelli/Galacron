using System.Collections.Generic;
using System.Linq;
namespace Galacron
{
    public enum Sounds
    {
        UI_CLICK_01,
        UI_CLICK_02,
        UI_CLICK_03,
        UI_CLICK_04,
        UI_CLICK_05,

        SFX_EXPLOSION_01,
        SFX_EXPLOSION_02,
        SFX_EXPLOSION_03,
        SFX_EXPLOSION_04,
        SFX_EXPLOSION_05,
        SFX_EXPLOSION_06,
        SFX_HIT_01,
        SFX_HIT_02,
        SFX_HIT_03,
        SFX_HIT_04,
        SFX_HIT_05,
        SFX_HIT_06,
        SFX_SHOOT_01,
        SFX_SHOOT_02,
        SFX_SHOOT_03,
        SFX_SHOOT_04,
        SFX_SHOOT_05,
    }

    public static class SoundIds
    {
        public static class UI
        {
            public const string CLICK_01 = "62824717-c3ce-4843-9cbd-fd740b78ea3a";
            public const string CLICK_02 = "93fbe6ad-1a72-4abe-afea-4ccb41c59d71";
            public const string CLICK_03 = "9d86e077-7078-4ab6-bf80-450e2a6a44b0";
            public const string CLICK_04 = "4253baea-b841-4d11-ba88-8d1462e44423";
            public const string CLICK_05 = "a060544b-1bee-4658-b3fe-6e9556c991d1";
        }

        public static class SFX
        {
            public const string EXPLOSION_01 = "c921facb-7d13-469c-b7a8-53093ecb414e";
            public const string EXPLOSION_02 = "68aea87f-b826-46f7-b376-e951f8652ced";
            public const string EXPLOSION_03 = "884f8d52-0ded-49a6-9635-0864b7b20f16";
            public const string EXPLOSION_04 = "9e192d8a-f3ca-483b-ae55-93943398cc2b";
            public const string EXPLOSION_05 = "9b081be4-dbda-43b3-a7c0-5c362eef2992";
            public const string EXPLOSION_06 = "6fb3ce71-80f1-49ee-a14a-be06f438d6a3";
            public const string HIT_01 = "7e405487-3176-43ca-80c1-fb4e5991ba14";
            public const string HIT_02 = "98cceb83-2a29-4a56-b624-e66cf07a1cc0";
            public const string HIT_03 = "8567ca0d-9e6c-4e7b-8a4d-540ad814b0f4";
            public const string HIT_04 = "b783a462-8136-48dd-9b54-d5a8a95c4ff5";
            public const string HIT_05 = "4115ac7e-ca17-4305-bd5b-8a78a514528b";
            public const string HIT_06 = "c29e2cd7-e65b-4ae2-a70f-7b396f07b760";
            public const string SHOOT_01 = "787e10b6-340a-4a57-a66c-5e573a73f443";
            public const string SHOOT_02 = "15641dc5-2aa6-4d06-b2bc-b8496350c9d0";
            public const string SHOOT_03 = "61aa7d09-54b0-4b03-87c9-64e830006da4";
            public const string SHOOT_04 = "e8ea3aae-45e5-4447-a6c2-ab694218ebc3";
            public const string SHOOT_05 = "8956cbfb-9365-40bc-93af-7497a918bf94";
        }

    }

    public static class SoundIdConverter
    {
        private static readonly Dictionary<Sounds, string> _soundIdCache = new Dictionary<Sounds, string>();

        static SoundIdConverter()
        {
            InitializeCache();
        }

        public static string GetId(Sounds sound)
        {
            return _soundIdCache.GetValueOrDefault(sound);
        }

        private static void InitializeCache()
        {
            var constants = new Dictionary<string, string>();

            // Get nested type classes (UI, SFX, Environment)
            var typeClasses = typeof(SoundIds).GetNestedTypes();
            foreach (var typeClass in typeClasses)
            {
                // Get the SoundType name from the class name
                var typeName = typeClass.Name.ToUpperInvariant();

                // Get all constant fields in this type class
                var fields = typeClass.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly);

                // Add constants with type prefix to match enum names
                foreach (var field in fields)
                {
                    constants[$"{typeName}_{field.Name}"] = (string)field.GetValue(null);
                }
            }

            foreach (Sounds sound in System.Enum.GetValues(typeof(Sounds)))
            {
                string enumName = sound.ToString();
                if (constants.TryGetValue(enumName, out string id))
                {
                    _soundIdCache[sound] = id;
                }
            }
        }

        public static void RefreshCache()
        {
            _soundIdCache.Clear();
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
