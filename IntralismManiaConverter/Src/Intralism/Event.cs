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
        ///     Initializes a new instance of the <see cref="Event"/> class.
        ///     An empty ctor allowing for serialization.
        /// </summary>
        public Event()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="time"> The start time of this event. </param>
        /// <param name="name"> the name being used in an event data configuration. </param>
        public Event(double time, string name)
        {
            this.Time = time;
            this.Data = new[]
            {
                EventType.ShowSprite.ToString(),
                $"{name},0,False,0,0,0",
            };
        }

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