using System;
using System.Text.Json.Serialization;
using IntralismManiaConverter.Enums;

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

        /// <summary>
        ///     Gets if the event is a background event.
        /// </summary>
        /// <returns> True if the event is a background event, else its a foreground event. </returns>
        public bool IsBackgroundLayer() =>
            this.Data[1].Split(',')[1] == "0";

        /// <summary>
        ///     Gets the data inside of Data.
        /// </summary>
        /// <returns> A string of data. </returns>
        public string GetInnerData() =>
            this.Data[1].Split(',')[0];

        /// <summary>
        ///     Checks if this event is a specific event type.
        /// </summary>
        /// <param name="eventType"> The event type being checked. </param>
        /// <returns> True if this event is event type else false. </returns>
        public bool IsEventOfType(EventType eventType) =>
            this.Data[0] == eventType.ToString();

        /// <summary>
        ///     Gets data inside of Data.
        /// </summary>
        /// <returns> A trimmed sting of the data inside of Data. </returns>
        public string GetTrimmedInnerData() =>
            this.Data[1].Split(',')[0][1..^1];

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(this.Time)}: {this.Time}, {nameof(this.Data)}: [{string.Join(", ", this.Data!)}]";
    }
}