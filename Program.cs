using System;
using System.Device.I2c;
using System.Threading;

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

            var settings = new I2cConnectionSettings(1, 0x90);
            I2cDevice myDevice = I2cDevice.Create(settings);
            byte[] data = { 0x01, 0xFF }; // Or byte[] data = { 0x10, 0x01, 0xFF }; both fails
            myDevice.Write(data);

            var buffer = new byte[128];
            myDevice.Read(buffer);
            Console.WriteLine(buffer);
        }
    }
}
