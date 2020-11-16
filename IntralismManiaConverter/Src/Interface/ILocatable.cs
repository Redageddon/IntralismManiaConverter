namespace IntralismManiaConverter.Interface
{
    /// <summary>
    ///     The interface that guarantees a class has a locatable path.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        ///     Gets or sets a path that can be referenced.
        /// </summary>
        public string Path { get; set; }
    }
}