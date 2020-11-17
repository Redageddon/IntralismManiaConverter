namespace IntralismManiaConverter.Intralism
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    ///     An intralism level resource.
    /// </summary>
    [Serializable]
    public class LevelResource
    {
        /// <summary>
        ///     Gets or sets the name of the resource.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the type of the resource.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets the path to the resource.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}