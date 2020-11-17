namespace IntralismManiaConverter.Intralism
{
    using System;
    using System.Text.Json.Serialization;
    using IntralismManiaConverter.Enums;

    /// <summary>
    ///     An intralism event.
    /// </summary>
    [Serializable]
    public class Event
    {
        /// <summary>
        ///     Gets or sets the time that this event starts.
        /// </summary>
        [JsonPropertyName("time")]
        public double Time { get; set; }

        /// <summary>
        ///     Gets or sets the data of this event.
        /// </summary>
        [JsonPropertyName("data")]
        public string[] Data { get; set; } = Array.Empty<string>();

        /// <summary>
        ///     Gets strings representative of the types data.
        /// </summary>
        /// <param name="type"> The Data's type. </param>
        /// <param name="value"> The Data's value. </param>
        /// <returns> Two strings of the the type and value. </returns>
        public static string[] GetDataStrings(EventType type, string value)
        {
            string[] output =
            {
                type.ToString(),
                value,
            };

            return output;
        }
    }
}