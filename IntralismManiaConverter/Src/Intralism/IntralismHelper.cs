using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntralismManiaConverter.Enums;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Mania;
using NAudio.Vorbis;
using NAudio.Wave;
using OsuParsers.Beatmaps.Objects;

namespace IntralismManiaConverter.Intralism
{
    /// <summary>
    ///     A class that helps with getting data from a mania beatmap.
    /// </summary>
    public class IntralismHelper : IStoryboardable
    {
        private readonly ManiaBeatMap maniaBeatMap;

        private readonly IntralismStoryboardHelper storyboardHelper;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntralismHelper"/> class.
        /// </summary>
        /// <param name="maniaBeatMap"> The beatmap that the helper class is reading from. </param>
        public IntralismHelper(ManiaBeatMap maniaBeatMap)
        {
            this.maniaBeatMap = maniaBeatMap;
            this.storyboardHelper = new IntralismStoryboardHelper(maniaBeatMap);
            this.ImagePaths = this.storyboardHelper.ImagePaths;
            this.IconFile = this.storyboardHelper.ImagePaths[0];

            this.AllEvents = this.GetAllEvents();
            this.LevelResources = this.ImagePaths.Select(path => new LevelResource(Path.GetFileName(path)));
        }

        /// <summary>
        ///     Gets the name of this beatmap.
        /// </summary>
        public string Name => this.maniaBeatMap.MetadataSection.ArtistUnicode + " - " + this.maniaBeatMap.MetadataSection.TitleUnicode;

        /// <summary>
        ///     Gets the info for intralism.
        /// </summary>
        public string Info =>
            $"Mania convert https://osu.ppy.sh/beatmapsets/{this.maniaBeatMap.MetadataSection.BeatmapSetID}/discussion/{this.maniaBeatMap.MetadataSection.BeatmapID} by {this.maniaBeatMap.MetadataSection.Creator}";

        /// <summary>
        ///     Gets the total length of a song in seconds.
        /// </summary>
        public double MusicTime
        {
            get
            {
                string path = Path.Combine(Path.GetDirectoryName(this.maniaBeatMap.Path)!,
                                           this.maniaBeatMap.GeneralSection.AudioFilename);

                string extension = Path.GetExtension(path);

                return extension switch
                {
                    ".mp3" => new Mp3FileReader(path).TotalTime.TotalSeconds,
                    ".wav" => new WaveFileReader(path).TotalTime.TotalSeconds,
                    ".ogg" => new VorbisWaveReader(path).TotalTime.TotalSeconds,
                    _      => 0,
                };
            }
        }

        /// <summary>
        ///     Gets the first storyboard background.
        /// </summary>
        public string IconFile { get; }

        /// <summary>
        ///     Gets every resource for intralism.
        /// </summary>
        public IEnumerable<LevelResource> LevelResources { get; }

        /// <summary>
        ///     Gets every event for intralism.
        /// </summary>
        public IEnumerable<Event> AllEvents { get; }

        /// <inheritdoc/>
        public List<string> ImagePaths { get; }

        private IEnumerable<Event> GetAllEvents()
        {
            IEnumerable<Event> hitObjectEvents = this.GetHitObjectEvents();
            IEnumerable<Event> storyboardEvents = this.storyboardHelper.StoryboardEvents;

            return storyboardEvents.Concat(hitObjectEvents!);
        }

        private IEnumerable<Event> GetHitObjectEvents() =>
            this.maniaBeatMap.HitObjects
                .Where(hitObject => Enum.IsDefined((Position)(int)hitObject.Position.X))
                .GroupBy(hitObject => hitObject.StartTime, ManiaToIntralismNote);

        private static Event ManiaToIntralismNote(int time, IEnumerable<HitObject> objects) =>
            new()
            {
                Time = TimeSpan.FromMilliseconds(time).TotalSeconds,
                Data = new[] { nameof(EventType.SpawnObj), $"[{string.Join('-', objects.Select(e => (Position)(int)e.Position.X)!)}]" },
            };
    }
}