using System;
using System.Collections.Generic;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Intralism;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Sections;
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
        ///     Initializes a new instance of the <see cref="ManiaBeatMap" /> class.
        ///     An empty ctor allowing for serialization.
        /// </summary>
        public ManiaBeatMap()
        {
            this.Version = 14;

            this.GeneralSection = new BeatmapGeneralSection
            {
                AudioLeadIn = 0,
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
                Creator = "IntralismToolCollection",
                Version = "IntralismConvert",
                Source = "Intralism",
                Tags = Array.Empty<string>(),
                BeatmapID = 0,
                BeatmapSetID = 0,
            };

            this.DifficultySection = new BeatmapDifficultySection
            {
                HPDrainRate = 8,
                CircleSize = 4,
                OverallDifficulty = 8,
                ApproachRate = 5,
                SliderMultiplier = 1.4,
                SliderTickRate = 1,
            };

            this.EventsSection = new BeatmapEventsSection();

            this.TimingPoints = new List<TimingPoint>
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
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaBeatMap" /> class.
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
        ///     Initializes a new instance of the <see cref="ManiaBeatMap" /> class.
        /// </summary>
        /// <param name="intralismBeatMap"> Creates a <see cref="ManiaBeatMap" /> from an <see cref="IntralismBeatMap" />. </param>
        public ManiaBeatMap(IntralismBeatMap intralismBeatMap)
            : this()
        {
            this.Helper = new ManiaHelper(intralismBeatMap);

            this.GeneralSection.AudioFilename = this.Helper.AudioFilename;
            this.GeneralSection.PreviewTime = this.Helper.PreviewTime;
            this.MetadataSection.Title = this.Helper.Title;
            this.MetadataSection.TitleUnicode = this.Helper.Title;
            this.MetadataSection.Artist = this.Helper.Artist;
            this.MetadataSection.ArtistUnicode = this.Helper.Artist;
            this.EventsSection.BackgroundImage = this.Helper.BackgroundImage;
            this.EventsSection.Storyboard = this.Helper.Storyboard;
            this.HitObjects.AddRange(this.Helper.GetManiaHitObjects()!);
        }

        /// <summary>
        ///     Gets the helper class for mania.
        /// </summary>
        public ManiaHelper Helper { get; }

        /// <inheritdoc />
        public string Path { get; set; } = string.Empty;

        /// <inheritdoc />
        public void SaveToFile(string outputPath) => this.Write(outputPath);
    }
}