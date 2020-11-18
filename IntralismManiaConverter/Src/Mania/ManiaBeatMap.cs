using System;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Intralism;
using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Beatmaps;

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
            this.Helper = new ManiaHelper(intralismBeatMap);
            this.Version = 14;

            this.GeneralSection = new ()
            {
                AudioFilename = this.Helper.AudioFilename,
                AudioLeadIn = 0,
                PreviewTime = this.Helper.PreviewTime,
                Countdown = false,
                SampleSet = SampleSet.Normal,
                StackLeniency = 0.7,
                Mode = Ruleset.Mania,
                LetterboxInBreaks = false,
                SpecialStyle = false,
                WidescreenStoryboard = false,
            };

            this.EditorSection = new ()
            {
                DistanceSpacing = 1,
                BeatDivisor = 4,
                GridSize = 4,
                TimelineZoom = 2.4f,
            };

            this.MetadataSection = new ()
            {
                Title = this.Helper.Title,
                TitleUnicode = this.Helper.Title,
                Artist = this.Helper.Artist,
                ArtistUnicode = this.Helper.Artist,
                Creator = "IntralismToolCollection",
                Version = "IntralismConvert",
                Source = "Intralism",
                Tags = Array.Empty<string>(),
                BeatmapID = 0,
                BeatmapSetID = 0,
            };

            this.DifficultySection = new ()
            {
                HPDrainRate = 5,
                CircleSize = 4,
                OverallDifficulty = 5,
                ApproachRate = 5,
                SliderMultiplier = 1.4,
                SliderTickRate = 1,
            };

            this.EventsSection = new ()
            {
                BackgroundImage = this.Helper.BackgroundImage,
                Storyboard = new (),
            };

            this.TimingPoints = new ()
            {
                new ()
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

            this.HitObjects.AddRange(ManiaHelper.GetManiaHitObjects(this.Helper.SpawnObjects)!);
        }

        /// <summary>
        ///     Gets the helper class for mania.
        /// </summary>
        public ManiaHelper Helper { get; }

        /// <inheritdoc/>
        public string Path { get; set; } = string.Empty;

        /// <inheritdoc/>
        public void SaveToFile(string outputPath) =>
            this.Write(outputPath);
    }
}