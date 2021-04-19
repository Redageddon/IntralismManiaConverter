using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Mania;
using MoreLinq.Extensions;
using OsuParsers.Beatmaps.Sections;
using OsuParsers.Storyboards.Commands;
using OsuParsers.Storyboards.Interfaces;
using OsuParsers.Storyboards.Objects;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     A class responsible for helping with mania to intralism storyboard conversions.
    /// </summary>
    public class IntralismStoryboardHelper
    {
        private readonly ManiaBeatMap maniaBeatMap;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismStoryboardHelper"/> class.
        /// </summary>
        /// <param name="maniaBeatMap"> A beatmap having data pulled from. </param>
        public IntralismStoryboardHelper(ManiaBeatMap maniaBeatMap)
        {
            this.maniaBeatMap = maniaBeatMap;
            this.FillStoryboardAndImagePaths();
        }

        /// <summary>
        ///     Gets all storyboard events.
        /// </summary>
        public List<Event> StoryboardEvents { get; } = new();

        /// <summary>
        ///     Gets all storyboard image paths.
        /// </summary>
        public List<string> ImagePaths { get; } = new();

        private void FillStoryboardAndImagePaths()
        {
            BeatmapEventsSection eventsSection = this.maniaBeatMap.EventsSection;

            this.StoryboardEvents.Add(ManiaToIntralismEvent(0, eventsSection.BackgroundImage));
            this.ImagePaths.Add(eventsSection.BackgroundImage);

            foreach (StoryboardSprite sprite in GetStoryboardSprites(eventsSection.Storyboard.BackgroundLayer)
                .Concat(GetStoryboardSprites(eventsSection.Storyboard.ForegroundLayer)))
            {
                this.ImagePaths.Add(sprite.FilePath);
                Command command = sprite.Commands.Commands[0];
                this.StoryboardEvents.Add(ManiaToIntralismEvent(command.StartTime, sprite.FilePath, duration: command.EndTime - command.StartTime));
            }
        }

        private static IEnumerable<StoryboardSprite> GetStoryboardSprites(IEnumerable<IStoryboardObject> storyboardObjects) =>
            storyboardObjects
                .DistinctBy(storyboardObject => storyboardObject.FilePath)
                .Select(storyboardObject => storyboardObject as StoryboardSprite)
                .Where(sprite => sprite.Commands.Commands.Count != 0);

        private static Event ManiaToIntralismEvent(int time,
                                                   string path,
                                                   int spritePosition = 0,
                                                   bool keepAspectRatio = true,
                                                   float duration = 0,
                                                   float fadeInDuration = 0,
                                                   float fadeOutDuration = 0) =>
            new()
            {
                Time = TimeSpan.FromMilliseconds(time).TotalSeconds,
                Data = new[]
                {
                    nameof(EventType.ShowSprite),
                    $"{Path.GetFileName(path)},{spritePosition},{keepAspectRatio},{duration},{fadeInDuration},{fadeOutDuration}",
                },
            };
    }
}