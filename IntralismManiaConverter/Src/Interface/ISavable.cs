namespace IntralismManiaConverter.Interface
{
    /// <summary>
    ///     The interface responsible for guaranteeing that a class can be saved.
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        ///     The method responsible for saving a classes data to a path.
        /// </summary>
        /// <param name="outputPath"> The path to be saved to. </param>
        public void SaveToFile(string outputPath);
    }
}