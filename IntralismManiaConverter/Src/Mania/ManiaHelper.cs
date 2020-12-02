using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Intralism;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Storyboards;
using EventType = IntralismManiaConverter.Enums.EventType;

namespace IntralismManiaConverter.Mania
{
    /// <summary>
    ///     A class to help mania deserialization.
    /// </summary>
    public class ManiaHelper : IStoryboardable
    {
        private readonly IntralismBeatMap intralismBeatMap;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaHelper"/> class.
        /// </summary>
        /// <param name="intralismBeatMap"> The intralism beatmap having data pulled from. </param>
        public ManiaHelper(IntralismBeatMap intralismBeatMap)
        {
            this.intralismBeatMap = intralismBeatMap;
            ManiaStoryboardHelper storyboardHelper = new (intralismBeatMap);
            this.Storyboard = storyboardHelper.Storyboard;

            this.ImagePaths.Add(this.BackgroundImage);
            this.ImagePaths.AddRange(storyboardHelper.SpritePaths!);
        }

        /// <inheritdoc />
        public List<string> ImagePaths { get; } = new ();

        /// <summary>
        ///     Gets the name of the artist and defaults to "Intralism" if one isn't found.
        /// </summary>
        public string Artist
        {
            get
            {
                string[] names = this.intralismBeatMap.Name.Split('-');
                return names.Length > 1
                    ? names[0]
                    : "Intralism";
            }
        }

        /// <summary>
        ///     Gets the name of the intralism beatmap.
        /// </summary>
        public string Title => this.intralismBeatMap.Name;

        /// <summary>
        ///     Gets the icon file of the intralism beatmap.
        /// </summary>
        public string BackgroundImage => this.intralismBeatMap.IconFile;

        /// <summary>
        ///     Gets the preview time of this beatmap.
        /// </summary>
        public int PreviewTime => (int)TimeSpan.FromSeconds(this.intralismBeatMap.MusicTime).TotalMilliseconds / 2;

        /// <summary>
        ///     Gets the filename of the audio.
        /// </summary>
        /// <returns> A string of the audio name. </returns>
        public string AudioFilename => this.intralismBeatMap.MusicFile;

        /// <summary>
        ///     Gets a storyboard filled with intralism sprites.
        /// </summary>
        public Storyboard Storyboard { get; }

        /// <summary>
        ///     Gets all Events of type hitObject.
        /// </summary>
        /// <returns> A collection of hitObjects. </returns>
        public IEnumerable<HitObject> GetManiaHitObjects() =>
            this.SpawnObjects()?.SelectMany(@event => IntralismToManiaNote(@event, (int)TimeSpan.FromSeconds(@event.Time).TotalMilliseconds));

        private static IEnumerable<HitCircle> IntralismToManiaNote(Event data, int timing)
        {
            string rawData = data.GetTrimmedInnerData();
            string[] notes = rawData.Split('-');
            IEnumerable<Position> positions = notes.Select(Enum.Parse<Position>);

            return positions.Select(position => GetManiaHitObject(position, timing));
        }

        private static HitCircle GetManiaHitObject(Position position, int timing) =>
            new (new Vector2((int)position, 192), timing, 1, 0, new Extras(), false, 0);

        private IEnumerable<Event> SpawnObjects() =>
            this.intralismBeatMap.Events?.Where(e => e.IsEventOfType(EventType.SpawnObj));
    }
}