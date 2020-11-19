using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Intralism;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Enums.Storyboards;
using OsuParsers.Storyboards;
using OsuParsers.Storyboards.Objects;
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
        public ManiaHelper(IntralismBeatMap intralismBeatMap) =>
            this.intralismBeatMap = intralismBeatMap;

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
        ///     Gets all events of type "SpawnObj".
        /// </summary>
        /// <returns> A collection of Spawn Objects. </returns>
        public IEnumerable<Event> SpawnObjects =>
            this.intralismBeatMap.Events?.Where(e => e.Data[0] == EventType.SpawnObj.ToString());

        /// <summary>
        ///     Gets all Events of type hitObject.
        /// </summary>
        /// <param name="spawnObjects"> All intralism events. </param>
        /// <returns> A collection of hitObjects. </returns>
        public static IEnumerable<HitObject> GetManiaHitObjects(IEnumerable<Event> spawnObjects) =>
            spawnObjects?.SelectMany(e => IntralismToManiaNote(e.Data[1], (int)TimeSpan.FromSeconds(e.Time).TotalMilliseconds));

        /// <summary>
        ///     Gets a storyboard filled with intralism sprites.
        /// </summary>
        /// <returns> A filled storyboard. </returns>
        public Storyboard GetStoryboard()
        {
            Storyboard storyboard = new ();
            IEnumerable<Event> spriteEvents = this.GetShowSpriteEvents();

            foreach (Event spriteEvent in spriteEvents)
            {
                StoryboardSprite sprite = this.IntralismToManiaStoryboard(spriteEvent);
                this.ImagePaths.Add(sprite.FilePath);

                switch (spriteEvent.Data[1].Split(',')[1])
                {
                    case "0": storyboard.BackgroundLayer.Add(sprite);
                        break;
                    case "1": storyboard.ForegroundLayer.Add(sprite);
                        break;
                }
            }

            return storyboard;
        }

        private static IEnumerable<HitCircle> IntralismToManiaNote(string data, int timing)
        {
            string rawData = data.Split(',')[0][1..^1];
            string[] notes = rawData.Split('-');
            IEnumerable<Position> positions = notes.Select(Enum.Parse<Position>);

            return positions.Select(position => GetManiaHitObject(position, timing));
        }

        private static HitCircle GetManiaHitObject(Position position, int timing) =>
            new (new Vector2((int)position, 192), timing, 1, 0, new Extras(), false, 0);

        private StoryboardSprite IntralismToManiaStoryboard(Event sprite)
        {
            LevelResource matchingResource = this.intralismBeatMap.LevelResources?.First(e => e.Name == sprite.Data[1].Split(',')[0]);
            string mapEnd = this.intralismBeatMap.Events?.First(e => e.Data[0] == EventType.MapEnd.ToString()).Data[1];
            mapEnd = string.IsNullOrEmpty(mapEnd)
                ? "1000"
                : mapEnd;
            double endTimeInSeconds = double.Parse(mapEnd);

            int startTime = (int)TimeSpan.FromSeconds(sprite.Time).TotalMilliseconds;
            int endTime = (int)TimeSpan.FromSeconds(endTimeInSeconds).TotalMilliseconds;

            StoryboardSprite storyboardSprite = new (Origins.Centre, matchingResource.Path, 0, 0);
            storyboardSprite.Commands.Commands.Add(new (Easing.None, startTime, endTime - startTime, Color.White, Color.White));

            return storyboardSprite;
        }

        private IEnumerable<Event> GetShowSpriteEvents() =>
            this.intralismBeatMap.Events?.Where(e => e.Data[0] == EventType.ShowSprite.ToString());
    }
}