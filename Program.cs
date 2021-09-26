using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Mlx90614;

namespace raspi_temperature
{
    class Program
    {
        private const byte PCF8591 = (0x90 >> 1); // device address
        private const byte PCF8591_ADC_CH0 = 0x40; // thermistor
        private const byte PCF8591_ADC_CH1 = 0x41; // photo-voltaic cell
        private const byte PCF8591_ADC_CH2 = 0x42;
        private const byte PCF8591_ADC_CH3 = 0x41; // photentiometer
        private I2cDevice I2PCF8591;
        // private DispatcherTimer timer;

        static void Main(string[] args)
        {
            //string aqs = I2cDevice.GetDeviceSelector();

            var settings = new I2cConnectionSettings(1, 0x48);
            using (I2cDevice device = I2cDevice.Create(settings))
            {
                var analogVal = device.ReadByte();

                var rawTemperature = Convert.ToDouble(analogVal);
                var Vr = 5 * (rawTemperature) / 255;
                var Rt = 10000 * Vr / (5 - Vr);
                var temp = 1 / (((Math.Log(Rt / 10000)) / 3950) + (1 / (273.15 + 25)));
                temp = temp - 273.15;
                //
                //  Display the readings in the debug window and pause before repeating.
                //
                Console.WriteLine("final temp: " + temp);
            }
        }
    }
}
