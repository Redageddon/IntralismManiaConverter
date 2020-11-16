using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Mania;
using NAudio.Wave;
using Newtonsoft.Json;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Sections;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     The class representing intralism data.
    /// </summary>
    [Serializable]
    public class IntralismBeatMap : ISavable, ILocatable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismBeatMap"/> class.
        ///     An empty ctor allowing for serialization.
        /// </summary>
        public IntralismBeatMap()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismBeatMap"/> class.
        /// </summary>
        /// <param name="path"> The path being loaded from. </param>
        public IntralismBeatMap(string path)
        {
            IntralismBeatMap data = JsonConvert.DeserializeObject<IntralismBeatMap>(File.ReadAllText(path));
            this.ConfigVersion = data.ConfigVersion;
            this.Name = data.Name;
            this.LevelResources = data.LevelResources;
            this.HandCount = data.HandCount;
            this.Speed = data.Speed;
            this.Lives = data.Lives;
            this.MaxLives = data.MaxLives;
            this.MusicFile = data.MusicFile;
            this.MusicTime = data.MusicTime;
            this.IconFile = data.IconFile;
            this.EnvironmentType = data.EnvironmentType;
            this.Hidden = data.Hidden;
            this.Events = data.Events;
            this.E = data.E;
            this.Path = path;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismBeatMap"/> class.
        /// </summary>
        /// <param name="maniaBeatMap"> Creates a <see cref="IntralismBeatMap"/> from a <see cref="ManiaBeatMap"/>. </param>
        public IntralismBeatMap(ManiaBeatMap maniaBeatMap)
        {
            BeatmapMetadataSection metaData = maniaBeatMap.MetadataSection;

            this.Name = metaData.ArtistUnicode + " - " + metaData.TitleUnicode;
            this.MusicTime = GetMusicTime(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(maniaBeatMap.Path), maniaBeatMap.GeneralSection.AudioFilename));
            this.IconFile = maniaBeatMap.EventsSection.BackgroundImage;
            this.Events = GetEvents(maniaBeatMap.HitObjects);

            this.Info =
                $"Mania convert https://osu.ppy.sh/beatmapsets/{metaData.BeatmapSetID}/discussion/{metaData.BeatmapID} by {metaData.Creator}";
        }

        /// <summary>
        ///     Gets or sets the config version.
        /// </summary>
        [JsonPropertyName("configVersion")]
        public int ConfigVersion { get; set; } = 2;

        /// <summary>
        ///     Gets or sets the name of the beatmap.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the beatmap info.
        /// </summary>
        [JsonPropertyName("info")]
        public string Info { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the beatmap level resources.
        /// </summary>
        [JsonPropertyName("levelResources")]
        public LevelResource[] LevelResources { get; set; } = new LevelResource[0];

        /// <summary>
        ///     Gets or sets the beatmap tags..
        /// </summary>
        [JsonPropertyName("tags")]
        public IEnumerable<string> Tags { get; set; } = new string[0];

        /// <summary>
        ///     Gets or sets the hand count.
        /// </summary>
        [JsonPropertyName("handCount")]
        public int HandCount { get; set; } = 1;

        /// <summary>
        ///     Gets or sets a more info link.
        /// </summary>
        [JsonPropertyName("moreInfoURL")]
        public string MoreInfoUrl { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the speed of the beatmap.
        /// </summary>
        [JsonPropertyName("speed")]
        public double Speed { get; set; } = 25;

        /// <summary>
        ///     Gets or sets the amount of lives in the beatmap.
        /// </summary>
        [JsonPropertyName("lives")]
        public int Lives { get; set; } = 10;

        /// <summary>
        ///     Gets or sets the maximum amount of lives in a beatmap.
        /// </summary>
        [JsonPropertyName("maxLives")]
        public int MaxLives { get; set; } = 10;

        /// <summary>
        ///     Gets or sets the music file name.
        /// </summary>
        [JsonPropertyName("musicFile")]
        public string MusicFile { get; set; } = "music.ogg";

        /// <summary>
        ///     Gets or sets the music's length in time.
        /// </summary>
        [JsonPropertyName("musicTime")]
        public double MusicTime { get; set; }

        /// <summary>
        ///     Gets or sets the icon file path.
        /// </summary>
        [JsonPropertyName("iconFile")]
        public string IconFile { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the environment type.
        /// </summary>
        [JsonPropertyName("environmentType")]
        public int EnvironmentType { get; set; }

        /// <summary>
        ///     Gets or sets the conditions to unlock the beatmap.
        /// </summary>
        [JsonPropertyName("unlockConditions")]
        public IEnumerable<object> UnlockConditions { get; set; } = new object[0];

        /// <summary>
        ///     Gets or sets a value indicating whether the beat map is hidden or not.
        /// </summary>
        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        ///     Gets or sets the checkpoints of the map.
        /// </summary>
        [JsonPropertyName("checkpoints")]
        public IEnumerable<object> Checkpoints { get; set; } = new object[0];

        /// <summary>
        ///     Gets or sets all of the beatmap events.
        /// </summary>
        [JsonPropertyName("events")]
        public IEnumerable<Event> Events { get; set; } = new Event[0];

        /// <summary>
        ///     Gets or sets E.
        /// </summary>
        [JsonPropertyName("e")]
        public string E { get; set; } = string.Empty;

        /// <inheritdoc/>
        [Newtonsoft.Json.JsonIgnore]
        public string Path { get; set; } = string.Empty;

        /// <inheritdoc/>
        public void SaveToFile(string outputPath) =>
            File.WriteAllText(outputPath, JsonConvert.SerializeObject(this, Formatting.Indented));

        /// <summary>
        ///     Gets the name of the artist and defaults to "Intralism" if one isn't found.
        /// </summary>
        /// <returns> The artists name. </returns>
        public string GetArtist()
        {
            string output = "Intralism";
            string[] names = this.Name.Split('-');

            if (names.Length > 1)
            {
                output = names[0];
            }

            return output;
        }

        /// <summary>
        ///     Gets all events of type "SpawnObj".
        /// </summary>
        /// <returns> A collection of Spawn Objects. </returns>
        public IEnumerable<Event> GetSpawnObjects() =>
            this.Events.Where(e => e.Data[0] == "SpawnObj");

        private static double GetMusicTime(string audioPath)
        {
            Mp3FileReader reader = new Mp3FileReader(audioPath);
            return reader.TotalTime.TotalSeconds;
        }

        private static IEnumerable<Event> GetEvents(IEnumerable<HitObject> hitObjects) =>
            hitObjects.Where(h => Enum.IsDefined(typeof(Position), (int)h.Position.X))
                      .GroupBy(
                          s => s.StartTime,
                          (i, objects) =>
                              new Event
                              {
                                  Time = TimeSpan.FromMilliseconds(i).TotalSeconds,
                                  Data = new List<string>
                                  {
                                      "SpawnObj",
                                      $"[{string.Join('-', objects.Select(e => (Position)(int)e.Position.X))}],0",
                                  },
                              });
    }
}