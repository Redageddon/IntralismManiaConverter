using System.Collections.Generic;

namespace IntralismManiaConverter.Interface
{
    public interface IStoryboardable
    {
        /// <summary>
        ///     A collection of all image paths.
        /// </summary>
        public List<string> ImagePaths { get; }
    }
}