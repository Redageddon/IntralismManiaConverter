namespace IntralismManiaConverter.Enums
{
    using System;

    /// <summary>
    ///     The enum that represents mania to intralism note conversions.
    /// </summary>
    [Flags]
    public enum Position
    {
        /// <summary>
        ///     The mania note representing Intralism's Left note
        /// </summary>
        Left = 64,

        /// <summary>
        ///     The mania note representing Intralism's Up note
        /// </summary>
        Up = 192,

        /// <summary>
        ///     The mania note representing Intralism's Down note
        /// </summary>
        Down = 320,

        /// <summary>
        ///     The mania note representing Intralism's Right note
        /// </summary>
        Right = 448,
    }
}