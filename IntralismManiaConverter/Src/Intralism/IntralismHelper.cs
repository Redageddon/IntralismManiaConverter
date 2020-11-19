using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Mania;
using MoreLinq;
using NAudio.Wave;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Storyboards.Objects;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     A class that helps with getting data from a mania beatmap.
    /// </summary>
    public class IntralismHelper
    {
        private readonly ManiaBeatMap maniaBeatMap;
        private readonly IEnumerable<Event> events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismHelper"/> class.
        /// </summary>
        /// <param name="maniaBeatMap"> The beatmap that the helper class is reading from. </param>
        public IntralismHelper(ManiaBeatMap maniaBeatMap)
        {
            this.maniaBeatMap = maniaBeatMap;
            this.events = this.GetStoryboardEvents();
        }

        /// <summary>
        ///     Gets the name of this beatmap.
        /// </summary>
        public string Name => this.maniaBeatMap.MetadataSection.ArtistUnicode + " - " + this.maniaBeatMap.MetadataSection.TitleUnicode;

        /// <summary>
        ///     Gets the info for intralism.
        /// </summary>
        public string Info => $"Mania convert https://osu.ppy.sh/beatmapsets/{this.maniaBeatMap.MetadataSection.BeatmapSetID}/discussion/{this.maniaBeatMap.MetadataSection.BeatmapID} by {this.maniaBeatMap.MetadataSection.Creator}";

        /// <summary>
        ///     Gets the total length of a song in seconds.
        /// </summary>
        public double MusicTime => new Mp3FileReader(Path.Combine(Path.GetDirectoryName(this.maniaBeatMap.Path)!, this.maniaBeatMap.GeneralSection.AudioFilename!)).TotalTime.TotalSeconds;

        /// <summary>
        ///     Gets the first storyboard background.
        /// </summary>
        public string IconFile => this.GetStoryboardPaths()?.First();

        /// <summary>
        ///     Gets every resource for intralism.
        /// </summary>
        /// <returns> A collection of resources. </returns>
        public IEnumerable<LevelResource> GetLevelResources() =>
            this.GetStoryboardPaths()?.Select(path => new LevelResource(Path.GetFileName(path)));

        /// <summary>
        ///     Gets every event for intralism.
        /// </summary>
        /// <returns> A collection of events. </returns>
        public IEnumerable<Event> GetAllEvents()
        {
            IEnumerable<Event> hitObjectEvents = this.GetHitObjectEvents(this.maniaBeatMap.HitObjects);

            return this.events?.Concat(hitObjectEvents!);
        }

        /// <summary>
        ///     Gets all paths from a mania background storyboard.
        /// </summary>
        /// <returns> A collection of paths. </returns>
        public IEnumerable<string> GetStoryboardPaths()
        {
            yield return this.maniaBeatMap.EventsSection.BackgroundImage;

            foreach (StoryboardSprite sprite in this.GetBackgroundStoryboards())
            {
                yield return sprite.FilePath;
            }
        }

        private IEnumerable<Event> GetStoryboardEvents()
        {
            yield return new (0, this.maniaBeatMap.EventsSection.BackgroundImage);

            foreach (StoryboardSprite sprite in this.GetBackgroundStoryboards())
            {
                yield return new (
                    TimeSpan.FromMilliseconds(sprite.Commands.Commands[0].StartTime).TotalSeconds,
                    Path.GetFileName(sprite.FilePath));
            }
        }

        private IEnumerable<Event> GetHitObjectEvents(IEnumerable<HitObject> hitObjects) =>
            hitObjects?.Where(hitObject => Enum.IsDefined(typeof(Position), (int)hitObject.Position.X))
                      .GroupBy(hitObject => hitObject.StartTime, this.ManiaToIntralismNote);

        private Event ManiaToIntralismNote(int time, IEnumerable<HitObject> objects) =>
            new ()
            {
                Time = TimeSpan.FromMilliseconds(time).TotalSeconds,
                Data = new[]
                {
                    EventType.SpawnObj.ToString(),
                    $"[{string.Join('-', objects?.Select(e => (Position)(int)e.Position.X)!)}]",
                },
            };

        private IEnumerable<StoryboardSprite> GetBackgroundStoryboards() =>
            this.maniaBeatMap
                .EventsSection
                .Storyboard
                .BackgroundLayer
                .DistinctBy(storyboardObject => storyboardObject.FilePath)?
                .Select(storyboardObject => storyboardObject as StoryboardSprite);
    }
}