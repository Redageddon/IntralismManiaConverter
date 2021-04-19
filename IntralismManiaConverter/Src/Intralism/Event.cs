using System;
using System.Text.Json.Serialization;
using IntralismManiaConverter.Enums;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     An Intralism event.
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
        ///     Gets the data inside of Data.
        /// </summary>
        /// <returns>A <see cref="string"/> of data.</returns>
        public string GetInnerData() => this.Data[1].Split(',')[0];

        /// <summary>
        ///     Gets data inside of Data.
        /// </summary>
        /// <returns>A trimmed sting of the data inside of Data.</returns>
        public string GetInnerDataTrimmed() => this.GetInnerData()[1..^1];

        /// <summary>
        ///     Gets if the event is a background event.
        /// </summary>
        /// <returns><see langword="true"/> if the event is a background event, else wise <see langword="false"/>.</returns>
        public bool IsBackgroundLayer() => this.Data[1].Split(',')[1] == "0";

        /// <summary>
        ///     Checks if this <see cref="Event"/> is a specific <see cref="EventType"/>.
        /// </summary>
        /// <param name="eventType">An <see cref="EventType"/>.</param>
        /// <returns><see langword="true"/> if this event is event type, else wise <see langword="false"/>.</returns>
        public bool IsEventOfType(EventType eventType) => this.Data[0] == eventType.ToString();

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(this.Time)}: {this.Time}, {nameof(this.Data)}: [{string.Join(", ", this.Data!)}]";
    }
}