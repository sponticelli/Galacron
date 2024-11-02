using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;

namespace Galacron
{
    /// <summary>
    /// Converts enum values to their corresponding string IDs using reflection and caching.
    /// </summary>
    public static class SoundIdConverter
    {
        // Cache for storing the mapping between enum values and their corresponding string IDs
        private static readonly Dictionary<Sounds, string> _soundIdCache = new Dictionary<Sounds, string>();
        
        // Static constructor to initialize the cache
        static SoundIdConverter()
        {
            InitializeCache();
        }

        /// <summary>
        /// Gets the sound ID corresponding to the given sound enum value.
        /// </summary>
        /// <param name="sound">The sound enum value to convert.</param>
        /// <returns>The corresponding sound ID string, or null if not found.</returns>
        public static string GetId(Sounds sound)
        {
            return _soundIdCache.GetValueOrDefault(sound);
        }

        /// <summary>
        /// Initializes the cache by matching enum values with their corresponding constant fields.
        /// </summary>
        private static void InitializeCache()
        {
            var soundType = typeof(SoundIds.Sound);
            var constants = soundType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                   .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                                   .ToDictionary(fi => fi.Name, fi => (string)fi.GetValue(null));

            foreach (Sounds sound in Enum.GetValues(typeof(Sounds)))
            {
                string enumName = sound.ToString();
                if (constants.TryGetValue(enumName, out string id))
                {
                    _soundIdCache[sound] = id;
                }
            }
        }

        /// <summary>
        /// Forces a refresh of the cache. Useful if sound IDs are modified at runtime.
        /// </summary>
        public static void RefreshCache()
        {
            _soundIdCache.Clear();
            InitializeCache();
        }
        
        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            RefreshCache();
        }
    }
}