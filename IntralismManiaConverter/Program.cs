using System.Threading.Tasks;

namespace IntralismManiaConverter
{
    public class Program
    {
        public static async Task Main()
        {
            await Converter.AsyncConvertManiaToIntralism(@"C:\Users\Rubiksmaster02\AppData\Local\osu!\Songs\914328 ClariS - SHIORI (TV Size)\ClariS - SHIORI (TV Size) (Daeng Simatta) [Normal].osu", "output");
        }
    }
}