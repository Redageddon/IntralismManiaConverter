namespace IntralismManiaConverter.Mania
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using IntralismManiaConverter.Enums;
    using IntralismManiaConverter.Intralism;
    using OsuParsers.Beatmaps.Objects;

    /// <summary>
    ///     A class to help mania deserialization.
    /// </summary>
    public class ManiaHelper
    {
        private readonly IntralismBeatMap intralismBeatMap;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManiaHelper"/> class.
        /// </summary>
        /// <param name="intralismBeatMap"> The intralism beatmap having data pulled from. </param>
        public ManiaHelper(IntralismBeatMap intralismBeatMap) =>
            this.intralismBeatMap = intralismBeatMap;

        /// <summary>
        ///     Gets all Events of type hitObject.
        /// </summary>
        /// <param name="spawnObjects"> All intralism events. </param>
        /// <returns> A collection of hitObjects. </returns>
        public static IEnumerable<HitObject> GetManiaHitObjects(IEnumerable<Event> spawnObjects) =>
            spawnObjects?.SelectMany(e => IntralismToManiaNote(e.Data[1], (int)TimeSpan.FromSeconds(e.Time).TotalMilliseconds));

        /// <summary>
        ///     Gets the name of the artist and defaults to "Intralism" if one isn't found.
        /// </summary>
        /// <returns> The artists name. </returns>
        public string GetArtist()
        {
            string[] names = this.intralismBeatMap.Name.Split('-');
            return names.Length > 1
                ? names[0]
                : "Intralism";
        }

        /// <summary>
        ///     Gets the name of the intralism beatmap.
        /// </summary>
        /// <returns> A string representing a name. </returns>
        public string GetTitle() =>
            this.intralismBeatMap.Name;

        /// <summary>
        ///     Gets the icon file of the intralism beatmap.
        /// </summary>
        /// <returns> A string representing the icon file path. </returns>
        public string GetBackgroundImage() =>
            this.intralismBeatMap.IconFile;

        /// <summary>
        ///     Gets the preview time of this beatmap.
        /// </summary>
        /// <returns> Half of the total length of the song. </returns>
        public int GetPreviewTime() =>
            (int)TimeSpan.FromSeconds(this.intralismBeatMap.MusicTime).TotalMilliseconds / 2;

        /// <summary>
        ///     Gets the filename of the audio.
        /// </summary>
        /// <returns> A string of the audio name. </returns>
        public string GetAudioFilename() =>
            this.intralismBeatMap.MusicFile;

        /// <summary>
        ///     Gets all events of type "SpawnObj".
        /// </summary>
        /// <returns> A collection of Spawn Objects. </returns>
        public IEnumerable<Event> GetSpawnObjects() =>
            this.intralismBeatMap.Events?.Where(e => e.Data[0] == EventType.SpawnObj.ToString());

        private static IEnumerable<HitCircle> IntralismToManiaNote(string data, int timing)
        {
            string rawData = data.Split(',')[0][1..^1];
            string[] notes = rawData.Split('-');
            IEnumerable<Position> positions = notes.Select(Enum.Parse<Position>);

            return positions.Select(position => GetManiaHitObject(position, timing));
        }

        private static HitCircle GetManiaHitObject(Position position, int timing) =>
            new HitCircle(new Vector2((int)position, 192), timing, 1, 0, new (), false, 0);
    }
}