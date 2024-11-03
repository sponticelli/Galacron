using System.Collections.Generic;
using System.Linq;

namespace Galacron
{
    public enum MusicTracks
    {

        // MusicPlaylist Tracks
        MUSIC_GAME_LOOP_10,
        MUSIC_GAME_LOOP_11,
        MUSIC_GAME_LOOP_12,
        MUSIC_GAME_LOOP_13,
    }

    public static class PlaylistIds
    {
        public static class Music
        {
            public const string GAME_LOOP_10 = "music01";
            public const string GAME_LOOP_11 = "music02";
            public const string GAME_LOOP_12 = "music03";
            public const string GAME_LOOP_13 = "music04";
        }

        private static string GetPlaylistName(string enumValue)
        {
            int underscoreIndex = enumValue.IndexOf('_');
            return underscoreIndex > 0 ? enumValue.Substring(0, underscoreIndex) : enumValue;
        }

        private static string GetTrackName(string enumValue)
        {
            int underscoreIndex = enumValue.IndexOf('_');
            return underscoreIndex > 0 ? enumValue.Substring(underscoreIndex + 1) : enumValue;
        }
    }

    public static class MusicTrackConverter
    {
        private static readonly Dictionary<MusicTracks, string> _trackIdCache = new Dictionary<MusicTracks, string>();
        private static readonly Dictionary<string, MusicTracks> _reverseCache = new Dictionary<string, MusicTracks>();

        static MusicTrackConverter()
        {
            InitializeCache();
        }

        public static string GetId(MusicTracks track)
        {
            return _trackIdCache.GetValueOrDefault(track);
        }

        public static MusicTracks? GetTrack(string trackId)
        {
            return _reverseCache.GetValueOrDefault(trackId, default);
        }

        public static IEnumerable<MusicTracks> GetPlaylistTracks(string playlistName)
        {
            var prefix = playlistName.Replace("Playlist", "").ToUpperInvariant();
            return System.Enum.GetValues(typeof(MusicTracks))
                .Cast<MusicTracks>()
                .Where(t => t.ToString().StartsWith(prefix + "_"));
        }

        private static void InitializeCache()
        {
            var constants = new Dictionary<string, string>();

            // Get nested type classes (playlists)
            var playlistClasses = typeof(PlaylistIds).GetNestedTypes();
            foreach (var playlistClass in playlistClasses)
            {
                var playlistName = playlistClass.Name.ToUpperInvariant();

                // Get all constant fields in this playlist class
                var fields = playlistClass.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly);

                foreach (var field in fields)
                {
                    constants[$"{playlistName}_{field.Name}"] = (string)field.GetValue(null);
                }
            }

            foreach (MusicTracks track in System.Enum.GetValues(typeof(MusicTracks)))
            {
                string enumName = track.ToString();
                if (constants.TryGetValue(enumName, out string id))
                {
                    _trackIdCache[track] = id;
                    _reverseCache[id] = track;
                }
            }
        }

        public static void RefreshCache()
        {
            _trackIdCache.Clear();
            _reverseCache.Clear();
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
