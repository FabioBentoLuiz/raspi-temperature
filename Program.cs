using System;
using System.Threading.Tasks;

namespace raspi_temperature
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rt = new RaspiTemperature();
            await rt.Run();
            Console.WriteLine("Press [ENTER] to termitate...");
            Console.ReadLine();
        }
    }
}
