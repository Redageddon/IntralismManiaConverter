using System;
using System.Text.Json.Serialization;
using IntralismManiaConverter.Interface;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     An intralism level resource.
    /// </summary>
    [Serializable]
    public class LevelResource : ILocatable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LevelResource"/> class.
        ///     An empty ctor allowing for serialization.
        /// </summary>
        public LevelResource() {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="LevelResource"/> class.
        /// </summary>
        /// <param name="path"> The path/name to the resource. </param>
        public LevelResource(string path)
        {
            this.Name = path;
            this.Path = path;
            this.Type = "Sprite";
        }

        /// <summary>
        ///     Gets or sets the name of the resource.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the type of the resource.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the path to the resource.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    }
}