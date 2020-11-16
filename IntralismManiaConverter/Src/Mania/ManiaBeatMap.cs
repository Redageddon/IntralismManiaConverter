using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Intralism;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Sections;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Beatmaps;
using OsuParsers.Storyboards;

namespace IntralismManiaConverter.Mania
{
    /// <summary>
    ///     The class representing mania data.
    /// </summary>
    public class ManiaBeatMap : Beatmap, ISavable, ILocatable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaBeatMap"/> class.
        ///     An empty ctor allowing for serialization.
        /// </summary>
        public ManiaBeatMap()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaBeatMap"/> class.
        /// </summary>
        /// <param name="path"> The path being loaded from. </param>
        public ManiaBeatMap(string path)
        {
            Beatmap data = BeatmapDecoder.Decode(path);
            this.Version = data.Version;
            this.GeneralSection = data.GeneralSection;
            this.EditorSection = data.EditorSection;
            this.MetadataSection = data.MetadataSection;
            this.DifficultySection = data.DifficultySection;
            this.EventsSection = data.EventsSection;
            this.TimingPoints = data.TimingPoints;
            this.HitObjects = data.HitObjects;
            this.Path = path;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaBeatMap"/> class.
        /// </summary>
        /// <param name="intralismBeatMap"> Creates a <see cref="ManiaBeatMap"/> from an <see cref="IntralismBeatMap"/>. </param>
        public ManiaBeatMap(IntralismBeatMap intralismBeatMap)
        {
            this.Version = 14;

            this.GeneralSection = new BeatmapGeneralSection
            {
                AudioFilename = intralismBeatMap.MusicFile,
                AudioLeadIn = 0,
                PreviewTime = (int)TimeSpan.FromSeconds(intralismBeatMap.MusicTime).TotalMilliseconds / 2,
                Countdown = false,
                SampleSet = SampleSet.Normal,
                StackLeniency = 0.7,
                Mode = Ruleset.Mania,
                LetterboxInBreaks = false,
                SpecialStyle = false,
                WidescreenStoryboard = false,
            };

            this.EditorSection = new BeatmapEditorSection
            {
                DistanceSpacing = 1,
                BeatDivisor = 4,
                GridSize = 4,
                TimelineZoom = 2.4f,
            };

            this.MetadataSection = new BeatmapMetadataSection
            {
                Title = intralismBeatMap.Name,
                TitleUnicode = intralismBeatMap.Name,
                Artist = intralismBeatMap.GetArtist(),
                ArtistUnicode = intralismBeatMap.GetArtist(),
                Creator = "IntralismToolCollection",
                Version = "IntralismConvert",
                Source = "Intralism",
                Tags = new string[0],
                BeatmapID = 0,
                BeatmapSetID = 0,
            };

            this.DifficultySection = new BeatmapDifficultySection
            {
                HPDrainRate = 5,
                CircleSize = 4,
                OverallDifficulty = 5,
                ApproachRate = 5,
                SliderMultiplier = 1.4,
                SliderTickRate = 1,
            };

            this.EventsSection = new BeatmapEventsSection
            {
                BackgroundImage = intralismBeatMap.IconFile,
                Storyboard = new Storyboard(),
            };

            this.TimingPoints = new List<TimingPoint>
            {
                new TimingPoint
                {
                    Offset = 0,
                    BeatLength = 500,
                    TimeSignature = TimeSignature.SimpleQuadruple,
                    SampleSet = SampleSet.Normal,
                    CustomSampleSet = 0,
                    Volume = 100,
                    Inherited = true,
                    Effects = 0,
                },
            };

            this.HitObjects = GetManiaHitObjects(intralismBeatMap.GetSpawnObjects()).ToList();
        }

        /// <inheritdoc/>
        public string Path { get; set; } = string.Empty;

        /// <inheritdoc/>
        public void SaveToFile(string outputPath) =>
            this.Write(outputPath);

        private static IEnumerable<HitObject> GetManiaHitObjects(IEnumerable<Event> spawnObjects)
        {
            List<HitCircle> hitObjects = new List<HitCircle>();

            foreach (Event e in spawnObjects)
            {
                hitObjects.AddRange(IntralismToManiaNote(e.Data[1], (int)TimeSpan.FromSeconds(e.Time).TotalMilliseconds));
            }

            return hitObjects;
        }

        private static IEnumerable<HitCircle> IntralismToManiaNote(string data, int timing)
        {
            string rawData = data.Split(',')[0][1..^1];
            string[] notes = rawData.Split('-');
            IEnumerable<Position> positions = notes.Select(e => (Position)Enum.Parse(typeof(Position), e));

            return positions.Select(position => GetManiaHitObject(position, timing));
        }

        private static HitCircle GetManiaHitObject(Position position, int timing) =>
            new HitCircle(new Vector2((int)position, 192), timing, 1, 0, new Extras(), false, 0);
    }
}