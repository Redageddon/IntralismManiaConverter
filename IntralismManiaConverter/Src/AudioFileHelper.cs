using System.IO;
using FFmpeg.NET;

namespace IntralismManiaConverter
{
    /// <summary>
    ///     The class responsible for saving an audio file.
    /// </summary>
    public static class AudioFileHelper
    {
        /// <summary>
        ///     The path to the ffmeg installation, default is "ffmpeg.exe"
        /// </summary>
        public static string FfmpegPath { get; set; } = "ffmpeg.exe";

        private static readonly Engine Ffmpeg = new Engine(FfmpegPath);

        /// <summary>
        ///     Saves an audio file.
        /// </summary>
        /// <param name="startPath"> The path where the audio is loaded. </param>
        /// <param name="endPath"> The path where the audio is saved. </param>
        public static void SaveAudio(string startPath, string endPath)
        {
            if (Path.GetExtension(startPath) != ".ogg")
            {
                endPath = Path.Combine(Path.GetDirectoryName(endPath), "music.ogg");

                MediaFile mediaFile = Ffmpeg.ConvertAsync(
                    new MediaFile(startPath),
                    new MediaFile(endPath)).Result;
            }
            else
            {
                File.Copy(startPath, endPath);
            }
        }
    }
}