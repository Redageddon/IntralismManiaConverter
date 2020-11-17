using System;
using System.Linq;

namespace IntralismManiaConverter
{
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main()
        {
            await Converter.AsyncConvertManiaToIntralism(
                @"C:\Users\Rubiksmaster02\Downloads\151720 ginkiha - EOS\ginkiha - EOS (alacat) [RLC's Another].osu",
                "Output");
        }
    }
}