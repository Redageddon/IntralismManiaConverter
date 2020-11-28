using System;
using System.IO;
using EventType = IntralismManiaConverter.Enums.EventType;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     A class responsible for helping with mania to intralism storyboard conversions.
    /// </summary>
    public class IntralismStoryboardHelper
    {
        public static Event ManiaToIntralismStoryboard(
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
                    nameof(EventType.SpawnObj),
                    $"{Path.GetFileName(path)},{spritePosition},{keepAspectRatio},{duration},{fadeInDuration},{fadeOutDuration}",
                },
            };
    }
}