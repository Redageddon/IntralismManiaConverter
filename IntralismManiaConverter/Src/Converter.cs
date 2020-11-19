using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IntralismManiaConverter.Interface;
using IntralismManiaConverter.Intralism;
using IntralismManiaConverter.Mania;

namespace IntralismManiaConverter
{
    /// <summary>
    ///     The class responsible for converting mania and intralism beatmaps back and fourth.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        ///     Reads mania data and saves it as an intralism beatmap.
        /// </summary>
        /// <param name="pathToBeatmapFile"> The path to a osu mania ".osu" file. </param>
        /// <param name="outputFolder"> The path to the intralism beatmap output. </param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public static async Task AsyncConvertManiaToIntralism(string pathToBeatmapFile, string outputFolder)
        {
            ManiaBeatMap maniaBeatMap = new (pathToBeatmapFile);
            IntralismBeatMap intralismBeatMap = new (maniaBeatMap);

            string audioFileName = maniaBeatMap.GeneralSection.AudioFilename;
            await SaveFiles(pathToBeatmapFile, outputFolder, audioFileName, intralismBeatMap.Helper, intralismBeatMap);
        }

        /// <summary>
        ///     Reads intralism data and saves it as a mania beatmap.
        /// </summary>
        /// <param name="pathToBeatmapFile"> The path to a intralism "config.txt" file. </param>
        /// <param name="outputFolder"> The path to the osu mania beatmap output. </param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public static async Task AsyncConvertIntralismToMania(string pathToBeatmapFile, string outputFolder)
        {
            IntralismBeatMap intralismBeatMap = new (pathToBeatmapFile);
            ManiaBeatMap maniaBeatMap = new (intralismBeatMap);

            string audioFileName = intralismBeatMap.MusicFile;
            await SaveFiles(pathToBeatmapFile, outputFolder, audioFileName, maniaBeatMap.Helper, maniaBeatMap);
        }

        private static async Task SaveFiles(string pathToBeatmapFile, string outputFolder, string audioFilename, IStoryboardable backgroundFileNames, ISavable savable)
        {
            string rootPath = Path.GetDirectoryName(pathToBeatmapFile);

            await AsyncSaveAudio(rootPath, outputFolder, audioFilename);
            SaveImages(rootPath, outputFolder, backgroundFileNames.ImagePaths);
            SaveConfig(outputFolder, savable);
        }

        private static async Task AsyncSaveAudio(string rootPath, string outputFolder, string audioFileName)
        {
            string startingAudioPath = Path.Combine(rootPath!, audioFileName!);
            string endingAudioPath = Path.Combine(outputFolder!, audioFileName);
            await AudioFileHelper.AsyncSaveAudio(startingAudioPath, endingAudioPath);
        }

        private static void SaveConfig(string outputFolder, ISavable savable)
        {
            string outputFileName = savable is ManiaBeatMap
                ? "config.osu"
                : "config.txt";
            string configEndPath = Path.Combine(outputFolder!, outputFileName);
            savable.SaveToFile(configEndPath);
        }

        private static void SaveImages(string rootPath, string outputFolder, IEnumerable<string> backgroundFileNames)
        {
            foreach (string backgroundFileName in backgroundFileNames)
            {
                string startingBackgroundPath = Path.Combine(rootPath!, backgroundFileName!);
                string endingBackgroundPath = Path.Combine(outputFolder!, Path.GetFileName(backgroundFileName));
                File.Copy(startingBackgroundPath, endingBackgroundPath, true);
            }
        }
    }
}