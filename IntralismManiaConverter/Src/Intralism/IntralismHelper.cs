namespace IntralismManiaConverter.Intralism
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using IntralismManiaConverter.Enums;
    using IntralismManiaConverter.Mania;
    using NAudio.Wave;
    using OsuParsers.Beatmaps.Objects;
    using OsuParsers.Beatmaps.Sections;
    using OsuParsers.Storyboards.Interfaces;
    using OsuParsers.Storyboards.Objects;

    /// <summary>
    ///     A class that helps with getting data from a mania beatmap.
    /// </summary>
    public class IntralismHelper
    {
        private readonly ManiaBeatMap maniaBeatMap;
        private readonly List<Event> events = new List<Event>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismHelper"/> class.
        /// </summary>
        /// <param name="maniaBeatMap"> The beatmap that the helper class is reading from. </param>
        public IntralismHelper(ManiaBeatMap maniaBeatMap)
        {
            this.maniaBeatMap = maniaBeatMap;
            this.SetStoryboardEvents();
        }

        /// <summary>
        ///     Gets the info for intralism.
        /// </summary>
        /// <returns> A string of info. </returns>
        public string GetInfo()
        {
            BeatmapMetadataSection metaData = this.maniaBeatMap.MetadataSection;
            return $"Mania convert https://osu.ppy.sh/beatmapsets/{metaData.BeatmapSetID}/discussion/{metaData.BeatmapID} by {metaData.Creator}";
        }

        /// <summary>
        ///     Gets the name of this beatmap.
        /// </summary>
        /// <returns> A string with the artist and title. </returns>
        public string GetName() =>
            this.maniaBeatMap.MetadataSection.ArtistUnicode + " - " + this.maniaBeatMap.MetadataSection.TitleUnicode;

        /// <summary>
        ///     Gets the first storyboard background.
        /// </summary>
        /// <returns> A path to the first background image. </returns>
        public string GetIconFile() =>
            this.events[0].Data[1];

        /// <summary>
        ///     Gets the total length of a song in seconds.
        /// </summary>
        /// <returns> Time in seconds. </returns>
        public double GetMusicTime()
        {
            Mp3FileReader reader = new (Path.Combine(Path.GetDirectoryName(this.maniaBeatMap.Path)!, this.maniaBeatMap.GeneralSection.AudioFilename!));

            return reader.TotalTime.TotalSeconds;
        }

        /// <summary>
        ///     Gets all paths from a mania background storyboard.
        /// </summary>
        /// <returns> A collection of paths. </returns>
        public IEnumerable<string> GetStoryboardPaths()
        {
            yield return this.maniaBeatMap.EventsSection.BackgroundImage;

            foreach (IStoryboardObject storyboardObject in this.GetBackgroundStoryboards())
            {
                yield return storyboardObject.FilePath;
            }
        }

        /// <summary>
        ///     Gets every event for intralism.
        /// </summary>
        /// <returns> A collection of events. </returns>
        public IEnumerable<Event> GetAllEvents()
        {
            IEnumerable<Event> hitObjectEvents = GetHitObjectEvents(this.maniaBeatMap.HitObjects);

            return this.events?.Concat(hitObjectEvents!);
        }

        /// <summary>
        ///     Gets every resource for intralism.
        /// </summary>
        /// <returns> A collection of resources. </returns>
        public IEnumerable<LevelResource> GetLevelResources() =>
            this.GetStoryboardPaths()?.Select(path => new LevelResource(Path.GetFileName(path)));

        private static IEnumerable<Event> GetHitObjectEvents(IEnumerable<HitObject> hitObjects) =>
            hitObjects?.Where(h => Enum.IsDefined(typeof(Position), (int)h.Position.X))
                      .GroupBy(
                          s => s.StartTime,
                          (i, objects) =>
                              new Event
                              {
                                  Time = TimeSpan.FromMilliseconds(i).TotalSeconds,
                                  Data = Event.GetDataStrings(EventType.SpawnObj, $"[{string.Join('-', objects?.Select(e => (Position)(int)e.Position.X) !)}]"),
                              });

        private void SetStoryboardEvents()
        {
            this.events.Add(new (0, this.maniaBeatMap.EventsSection.BackgroundImage));

            this.events.AddRange(
                this.GetBackgroundStoryboards()?.Select(
                    s => new Event(
                        TimeSpan.FromMilliseconds(((StoryboardSprite)s).Commands.Commands[0].StartTime).TotalSeconds,
                        Path.GetFileName(s.FilePath)))!);
        }

        private IEnumerable<IStoryboardObject> GetBackgroundStoryboards() =>
            this.maniaBeatMap.EventsSection
                .Storyboard
                .BackgroundLayer?
                .GroupBy(e => e.FilePath, (_, f) => f?.First());
    }
}