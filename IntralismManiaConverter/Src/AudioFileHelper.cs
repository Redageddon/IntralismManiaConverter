using System.IO;
using System.Threading.Tasks;
using FFmpeg.NET;

namespace IntralismManiaConverter
{
    /// <summary>
    ///     The class responsible for saving an audio file.
    /// </summary>
    public static class AudioFileHelper
    {
        private static Engine ffmpeg = new ();
        private static string ffmpegPath;

        /// <summary>
        ///     Gets or sets the path to the ffmeg installation, default is "ffmpeg.exe".
        /// </summary>
        public static string FfmpegPath
        {
            get => ffmpegPath;
            set
            {
                ffmpeg = new (value);
                ffmpegPath = value;
            }
        }

        /// <summary>
        ///     Saves an audio file.
        /// </summary>
        /// <param name="startPath"> The path where the audio is loaded. </param>
        /// <param name="endPath"> The path where the audio is saved. </param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public static async Task AsyncSaveAudio(string startPath, string endPath)
        {
            if (Path.GetExtension(startPath) != ".ogg")
            {
                endPath = Path.Combine(Path.GetDirectoryName(endPath)!, "music.ogg");

                await ffmpeg.ConvertAsync(
                    new (startPath),
                    new (endPath));
            }
            else
            {
                File.Copy(startPath!, endPath!, true);
            }
        }
    }
}