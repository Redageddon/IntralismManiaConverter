using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using IntralismManiaConverter.Enums;
using OsuParsers.Beatmaps.Objects;

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
            this.Data = GetDataStrings(EventType.ShowSprite, $"{name},0,False,0,0,0");
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="time"> The start time of this event. </param>
        /// <param name="hitObjects"> A collection being used in an event data configuration. </param>
        public Event(double time, IEnumerable<HitObject> hitObjects)
        {
            this.Time = time;
            this.Data = GetDataStrings(EventType.SpawnObj, $"[{string.Join('-', hitObjects?.Select(e => (Position)(int)e.Position.X)!)}]");
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

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(this.Time)}: {this.Time}, {nameof(this.Data)}: [{string.Join(", ", this.Data!)}]";
    }
}