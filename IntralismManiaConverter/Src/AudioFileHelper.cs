namespace IntralismManiaConverter
{
    using System.IO;
    using FFmpeg.NET;

    /// <summary>
    ///     The class responsible for saving an audio file.
    /// </summary>
    public static class AudioFileHelper
    {
        private static Engine ffmpeg = new Engine();
        private static string ffmpegPath;

        /// <summary>
        ///     Gets or sets the path to the ffmeg installation, default is "ffmpeg.exe".
        /// </summary>
        public static string FfmpegPath
        {
            get => ffmpegPath;
            set
            {
                ffmpeg = new Engine(value);
                ffmpegPath = value;
            }
        }

        /// <summary>
        ///     Saves an audio file.
        /// </summary>
        /// <param name="startPath"> The path where the audio is loaded. </param>
        /// <param name="endPath"> The path where the audio is saved. </param>
        public static void SaveAudio(string startPath, string endPath)
        {
            if (Path.GetExtension(startPath) != ".ogg")
            {
                endPath = Path.Combine(Path.GetDirectoryName(endPath) !, "music.ogg");

                MediaFile unused = ffmpeg.ConvertAsync(
                    new MediaFile(startPath),
                    new MediaFile(endPath)).Result;
            }
            else
            {
                File.Copy(startPath!, endPath!);
            }
        }
    }
}