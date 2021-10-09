using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Mlx90614;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace raspi_temperature
{
    public class RaspiTemperature
    {
        private static DeviceClient deviceClient;

        public RaspiTemperature()
        {
            var connectionString = Environment.GetEnvironmentVariable("RASPI_01_CS");
            if(String.IsNullOrEmpty(connectionString)) {
                throw new InvalidOperationException("Environment variable RASPI_01_CS is not set.");
            }
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            deviceClient.SetConnectionStatusChangesHandler((status, reason) =>
            {
                Console.WriteLine($"Connection status change registered - status={status}, reason={reason}.");
            });
            
        }

        public async Task Run()
        {
            var busId = 1;
            var deviceAddress = 0x48;
            var settings = new I2cConnectionSettings(busId, deviceAddress);

            using (var device = I2cDevice.Create(settings))
            {
                while(true) {
                    var analogVal = device.ReadByte();

                    var rawTemperature = Convert.ToDouble(analogVal);
                    var Vr = 5 * (rawTemperature) / 255;
                    var Rt = 10000 * Vr / (5 - Vr);
                    var temp = 1 / (((Math.Log(Rt / 10000)) / 3950) + (1 / (273.15 + 25)));
                    temp = temp - 273.15;
                    
                    Console.WriteLine("final temp: " + temp);

                    var temperatureData = new TemperatureData { Id = "1", TemperatureCelsius = temp};

                    

                    var messageString  = JsonConvert.SerializeObject(temperatureData);
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));

                    await deviceClient.SendEventAsync(message);
                    
                    Task.Delay(2000).Wait();
                }
            }
        }
    }
}
