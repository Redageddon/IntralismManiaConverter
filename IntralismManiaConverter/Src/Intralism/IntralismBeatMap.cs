using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Mania;

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
            IntralismBeatMap data = JsonSerializer.Deserialize<IntralismBeatMap>(File.ReadAllText(path!));
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
            this.Events = data.Events;
            this.Path = path;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismBeatMap"/> class.
        /// </summary>
        /// <param name="maniaBeatMap"> Creates a <see cref="IntralismBeatMap"/> from a <see cref="ManiaBeatMap"/>. </param>
        public IntralismBeatMap(ManiaBeatMap maniaBeatMap)
        {
            this.Helper = new (maniaBeatMap);

            this.Name = this.Helper.Name;
            this.Info = this.Helper.Info;
            this.LevelResources = this.Helper.GetLevelResources();
            this.MusicTime = this.Helper.MusicTime;
            this.IconFile = this.Helper.IconFile;
            this.Events = this.Helper.GetAllEvents();
        }

        /// <summary>
        ///     Gets the intralism file helper class.
        /// </summary>
        [JsonIgnore]
        public IntralismHelper Helper { get; }

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
        public IEnumerable<LevelResource> LevelResources { get; set; } = Array.Empty<LevelResource>();

        /// <summary>
        ///     Gets or sets the hand count.
        /// </summary>
        [JsonPropertyName("handCount")]
        public int HandCount { get; set; } = 1;

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
        ///     Gets or sets all of the beatmap events.
        /// </summary>
        [JsonPropertyName("events")]
        public IEnumerable<Event> Events { get; set; } = Array.Empty<Event>();

        /// <inheritdoc/>
        [JsonIgnore]
        public string Path { get; set; } = string.Empty;

        /// <inheritdoc/>
        public void SaveToFile(string outputPath) =>
            File.WriteAllText(outputPath!, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}