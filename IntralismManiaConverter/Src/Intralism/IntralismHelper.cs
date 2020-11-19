using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Mania;
using MoreLinq;
using NAudio.Wave;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Storyboards.Commands;
using OsuParsers.Storyboards.Interfaces;
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

            foreach (StoryboardSprite sprite in this.GetStoryboardSprites(this.maniaBeatMap.EventsSection.Storyboard.BackgroundLayer))
            {
                yield return sprite.FilePath;
            }
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
            SpritePosition spritePosition = SpritePosition.Background,
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
                    $"{Path.GetFileName(path)},{(int)spritePosition},{keepAspectRatio},{duration},{fadeInDuration},{fadeOutDuration}",
                },
            };

        private IEnumerable<Event> GetStoryboardEvents()
        {
            yield return ManiaToIntralismStoryboard(0, this.maniaBeatMap.EventsSection.BackgroundImage);

            foreach (StoryboardSprite sprite in this.GetStoryboardSprites(this.maniaBeatMap.EventsSection.Storyboard.BackgroundLayer))
            {
                Command command = sprite.Commands.Commands[0];
                yield return ManiaToIntralismStoryboard(command.StartTime, sprite.FilePath, duration: command.StartFloat - command.EndTime);
            }

            foreach (StoryboardSprite sprite in this.GetStoryboardSprites(this.maniaBeatMap.EventsSection.Storyboard.ForegroundLayer))
            {
                Command command = sprite.Commands.Commands[0];
                yield return ManiaToIntralismStoryboard(command.StartTime, sprite.FilePath, duration: command.StartFloat - command.EndTime, spritePosition: SpritePosition.Foreground);
            }
        }

        private IEnumerable<Event> GetHitObjectEvents(IEnumerable<HitObject> hitObjects) =>
            hitObjects?.Where(hitObject => Enum.IsDefined(typeof(Position), (int)hitObject.Position.X))
                      .GroupBy(hitObject => hitObject.StartTime, ManiaToIntralismNote);

        private IEnumerable<StoryboardSprite> GetStoryboardSprites(IEnumerable<IStoryboardObject> storyboardObjects) =>
            storyboardObjects
                .DistinctBy(storyboardObject => storyboardObject.FilePath)?
                .Select(storyboardObject => storyboardObject as StoryboardSprite);
    }
}