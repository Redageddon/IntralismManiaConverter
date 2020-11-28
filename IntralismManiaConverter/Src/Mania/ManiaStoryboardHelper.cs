using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using IntralismManiaConverter.Intralism;
using NVorbis;
using OsuParsers.Enums.Storyboards;
using OsuParsers.Storyboards;
using OsuParsers.Storyboards.Objects;
using EventType = IntralismManiaConverter.Enums.EventType;

namespace IntralismManiaConverter.Mania
{
    /// <summary>
    ///     A class that helps with intralism to mania storyboard converting.
    /// </summary>
    public class ManiaStoryboardHelper
    {
        private readonly IntralismBeatMap intralismBeatMap;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaStoryboardHelper"/> class.
        /// </summary>
        /// <param name="intralismBeatMap"> An instance to be used. </param>
        public ManiaStoryboardHelper(IntralismBeatMap intralismBeatMap)
        {
            this.intralismBeatMap = intralismBeatMap;
            this.FillStoryboardAndSpritePaths();
        }

        /// <summary>
        ///     Gets a collection of all intralism sprite paths.
        /// </summary>
        public List<string> SpritePaths { get; } = new ();

        /// <summary>
        ///     Gets a completed mania storyboard.
        /// </summary>
        public Storyboard Storyboard { get; } = new ();

        private void FillStoryboardAndSpritePaths()
        {
            foreach (Event intralismSpriteEvent in this.GetShowSpriteEvents())
            {
                StoryboardSprite sprite = this.IntralismToManiaSprite(intralismSpriteEvent);
                this.SpritePaths.Add(sprite.FilePath);

                if (intralismSpriteEvent.IsBackgroundLayer())
                {
                    this.Storyboard.BackgroundLayer.Add(sprite);
                }
                else
                {
                    this.Storyboard.ForegroundLayer.Add(sprite);
                }
            }
        }

        private StoryboardSprite IntralismToManiaSprite(Event sprite)
        {
            LevelResource matchingResource = this.GetMatchingResource(sprite);
            int startTime = (int)TimeSpan.FromSeconds(sprite.Time).TotalMilliseconds;
            int endTime = this.GetMapEndTime();

            StoryboardSprite storyboardSprite = new (Origins.Centre, matchingResource.Path, 0, 0);
            storyboardSprite.Commands.Commands.Add(new (Easing.None, startTime, endTime, Color.White, Color.White));

            return storyboardSprite;
        }

        private int GetMapEndTime()
        {
            string mapEndTime = this.GetMapEndEvent().Data[1];
            mapEndTime = string.IsNullOrEmpty(mapEndTime)
                ? new VorbisReader(Path.Combine(Path.GetDirectoryName(this.intralismBeatMap.Path)!, this.intralismBeatMap.MusicFile!)).TotalTime.TotalSeconds.ToString()
                : mapEndTime;
            double endTimeInSeconds = double.Parse(mapEndTime);
            int endTimeInMilliseconds = (int)TimeSpan.FromSeconds(endTimeInSeconds).TotalMilliseconds;

            return endTimeInMilliseconds;
        }

        private IEnumerable<Event> GetShowSpriteEvents() =>
            this.intralismBeatMap.Events?.Where(e => e.IsEventOfType(EventType.ShowSprite));

        private Event GetMapEndEvent() =>
            this.intralismBeatMap.Events?.First(e => e.IsEventOfType(EventType.MapEnd));

        private LevelResource GetMatchingResource(Event @event)
        {
            string eventType = @event.GetInnerData();
            return this.intralismBeatMap.LevelResources?.First(e => e.Name == eventType);
        }
    }
}