using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Mlx90614;

namespace raspi_temperature
{
    class Program
    {
        static void Main(string[] args)
        {
            var busId = 1;
            var deviceAddress = 0x48;
            var settings = new I2cConnectionSettings(busId, deviceAddress);

            using (var device = I2cDevice.Create(settings))
            {
                var analogVal = device.ReadByte();

                var rawTemperature = Convert.ToDouble(analogVal);
                var Vr = 5 * (rawTemperature) / 255;
                var Rt = 10000 * Vr / (5 - Vr);
                var temp = 1 / (((Math.Log(Rt / 10000)) / 3950) + (1 / (273.15 + 25)));
                temp = temp - 273.15;
                
                Console.WriteLine("final temp: " + temp);
            }
        }
    }
}
