using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        public IntralismBeatMap() {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismBeatMap"/> class.
        /// </summary>
        /// <param name="maniaBeatMap">Creates a <see cref="IntralismBeatMap"/> from a <see cref="ManiaBeatMap"/>.</param>
        public IntralismBeatMap(ManiaBeatMap maniaBeatMap)
        {
            this.Helper = new IntralismHelper(maniaBeatMap);

            this.Name = this.Helper.Name;
            this.Info = this.Helper.Info;
            this.LevelResources = this.Helper.LevelResources;
            this.MusicTime = this.Helper.MusicTime;
            this.Events = this.Helper.AllEvents;
            this.IconFile = this.Helper.IconFile;
        }

        /// <summary>
        ///     Gets <see cref="IntralismHelper"/>.
        /// </summary>
        [JsonIgnore]
        public IntralismHelper? Helper { get; }

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
        public int Lives { get; set; } = 50;

        /// <summary>
        ///     Gets or sets the maximum amount of lives in a beatmap.
        /// </summary>
        [JsonPropertyName("maxLives")]
        public int MaxLives { get; set; } = 50;

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
            File.WriteAllText(outputPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));

        /// <summary>
        ///     Reads json data an deserializes it into an <see cref="IntralismBeatMap"/>.
        /// </summary>
        /// <param name="path">The json file path.</param>
        /// <returns>A <see cref="IntralismBeatMap"/>.</returns>
        /// <exception cref="NoNullAllowedException">When deserialization returns null.</exception>
        public static IntralismBeatMap ReadFromJsonFile(string path)
        {
            string jsonData = File.ReadAllText(path);

            IntralismBeatMap? intralismBeatMap = JsonSerializer.Deserialize<IntralismBeatMap>(jsonData);

            if (intralismBeatMap is null)
            {
                throw new NoNullAllowedException($"{nameof(intralismBeatMap)} should not be null. Something went wrong.");
            }

            return intralismBeatMap;
        }
    }
}