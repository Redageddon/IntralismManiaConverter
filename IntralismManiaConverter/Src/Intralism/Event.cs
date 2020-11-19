using System;
using System.Text.Json.Serialization;

namespace IntralismManiaConverter.Intralism
{
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

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(this.Time)}: {this.Time}, {nameof(this.Data)}: [{string.Join(", ", this.Data!)}]";
    }
}