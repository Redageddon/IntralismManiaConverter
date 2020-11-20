using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Mania;
using MoreLinq;
using NAudio.Wave;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Sections;
using OsuParsers.Storyboards.Commands;
using OsuParsers.Storyboards.Interfaces;
using OsuParsers.Storyboards.Objects;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     A class that helps with getting data from a mania beatmap.
    /// </summary>
    public class IntralismHelper : IStoryboardable
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
            this.events = this.GetAllEvents();
        }

        /// <inheritdoc />
        public List<string> ImagePaths { get; } = new ();

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
        public string IconFile => this.ImagePaths[0];

        /// <summary>
        ///     Gets every resource for intralism.
        /// </summary>
        /// <returns> A collection of resources. </returns>
        public IEnumerable<LevelResource> GetLevelResources() =>
            this.ImagePaths?.Select(path => new LevelResource(Path.GetFileName(path)));

        /// <summary>
        ///     Gets every event for intralism.
        /// </summary>
        /// <returns> A collection of events. </returns>
        public IEnumerable<Event> GetAllEvents()
        {
            IEnumerable<Event> hitObjectEvents = GetHitObjectEvents(this.maniaBeatMap.HitObjects);
            IEnumerable<Event> storyboardEvents = this.GetStoryboardEvents();

            return storyboardEvents?.Concat(hitObjectEvents!);
        }

        private static Event ManiaToIntralismNote(int time, IEnumerable<HitObject> objects) =>
            new ()
            {
                Time = TimeSpan.FromMilliseconds(time).TotalSeconds,
                Data = new[]
                {
                    EventType.SpawnObj.ToString(),
                    $"[{string.Join('-', objects?.Select(e => (Position)(int)e.Position.X)!)}]",
                },
            };

        private static Event ManiaToIntralismStoryboard(
            int time,
            string path,
            int spritePosition = 0,
            bool keepAspectRatio = true,
            float duration = 0,
            float fadeInDuration = 0,
            float fadeOutDuration = 0) =>
            new ()
            {
                Time = TimeSpan.FromMilliseconds(time).TotalSeconds,
                Data = new[]
                {
                    EventType.ShowSprite.ToString(),
                    $"{Path.GetFileName(path)},{spritePosition},{keepAspectRatio},{duration},{fadeInDuration},{fadeOutDuration}",
                },
            };

        private static IEnumerable<Event> GetHitObjectEvents(IEnumerable<HitObject> hitObjects) =>
            hitObjects?.Where(hitObject => Enum.IsDefined(typeof(Position), (int)hitObject.Position.X))
                      .GroupBy(hitObject => hitObject.StartTime, ManiaToIntralismNote);

        private static IEnumerable<StoryboardSprite> GetStoryboardSprites(IEnumerable<IStoryboardObject> storyboardObjects) =>
            storyboardObjects
                .DistinctBy(storyboardObject => storyboardObject.FilePath)?
                .Select(storyboardObject => storyboardObject as StoryboardSprite)
                .Where(sprite => sprite.Commands.Commands.Count != 0);

        private IEnumerable<Event> GetStoryboardEvents()
        {
            BeatmapEventsSection eventsSection = this.maniaBeatMap.EventsSection;

            yield return ManiaToIntralismStoryboard(0, eventsSection.BackgroundImage);
            this.ImagePaths.Add(eventsSection.BackgroundImage);

            foreach (StoryboardSprite sprite in GetStoryboardSprites(eventsSection.Storyboard.BackgroundLayer)?.Concat(GetStoryboardSprites(eventsSection.Storyboard.ForegroundLayer)))
            {
                this.ImagePaths.Add(sprite.FilePath);
                Command command = sprite.Commands.Commands[0];
                yield return ManiaToIntralismStoryboard(command.StartTime, sprite.FilePath, duration: command.EndTime - command.StartTime);
            }
        }
    }
}