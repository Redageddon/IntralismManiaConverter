namespace IntralismManiaConverter
{
    using System.IO;
    using IntralismManiaConverter.Interface;
    using IntralismManiaConverter.Intralism;
    using IntralismManiaConverter.Mania;

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
        public static void ConvertManiaToIntralism(string pathToBeatmapFile, string outputFolder)
        {
            ManiaBeatMap maniaBeatMap = new ManiaBeatMap(pathToBeatmapFile);
            IntralismBeatMap intralismBeatMap = new IntralismBeatMap(maniaBeatMap);

            string rootPath = Path.GetDirectoryName(pathToBeatmapFile);

            SaveAudio(rootPath, outputFolder, maniaBeatMap.GeneralSection.AudioFilename);
            SaveBackground(rootPath, outputFolder, maniaBeatMap.EventsSection.BackgroundImage);
            SaveConfig(outputFolder, intralismBeatMap);
        }

        /// <summary>
        ///     Reads intralism data and saves it as a mania beatmap.
        /// </summary>
        /// <param name="pathToBeatmapFile"> The path to a intralism "config.txt" file. </param>
        /// <param name="outputFolder"> The path to the osu mania beatmap output. </param>
        public static void ConvertIntralismToMania(string pathToBeatmapFile, string outputFolder)
        {
            IntralismBeatMap intralismBeatMap = new IntralismBeatMap(pathToBeatmapFile);
            ManiaBeatMap maniaBeatMap = new ManiaBeatMap(intralismBeatMap);

            string rootPath = Path.GetDirectoryName(pathToBeatmapFile);

            SaveBackground(rootPath, outputFolder, intralismBeatMap.IconFile);
            SaveConfig(outputFolder, maniaBeatMap, true);
            SaveAudio(rootPath, outputFolder, intralismBeatMap.MusicFile);
        }

        private static void SaveConfig(string outputFolder, ISavable savable, bool isMania = false)
        {
            string outputFileName = isMania
                ? "config.osu"
                : "config.txt";
            string configEndPath = Path.Combine(outputFolder!, outputFileName);
            savable.SaveToFile(configEndPath);
        }

        private static void SaveAudio(string rootPath, string outputFolder, string audioFileName)
        {
            string startingAudioPath = Path.Combine(rootPath!, audioFileName!);
            string endingAudioPath = Path.Combine(outputFolder!, audioFileName);
            AudioFileHelper.SaveAudio(startingAudioPath, endingAudioPath);
        }

        private static void SaveBackground(string rootPath, string outputFolder, string backgroundFileName)
        {
            string startingBackgroundPath = Path.Combine(rootPath!, backgroundFileName!);
            string endingBackgroundPath = Path.Combine(outputFolder!, backgroundFileName);
            File.Copy(startingBackgroundPath, endingBackgroundPath);
        }
    }
}