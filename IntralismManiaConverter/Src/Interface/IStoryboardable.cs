using System.Collections.Generic;

namespace IntralismManiaConverter.Interface
{
    /// <summary>
    ///     Allows a helper type to store all savable image paths.
    /// </summary>
    public interface IStoryboardable
    {
        /// <summary>
        ///     Gets a collection of all image paths.
        /// </summary>
        public List<string> ImagePaths { get; }
    }
}