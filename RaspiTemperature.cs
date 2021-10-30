using System;
using System.Device.I2c;
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
            deviceClient = GetAzureIotHubClient();
        }

        private DeviceClient GetAzureIotHubClient()
        {
            var connectionString = GetConnectionString();
            var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            deviceClient.SetConnectionStatusChangesHandler((status, reason) =>
            {
                Console.WriteLine($"Connection status change registered - status={status}, reason={reason}.");
            });

            return deviceClient;
        }

        private string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("RASPI_01_CS");
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Environment variable RASPI_01_CS is not set.");
            }

            return connectionString;
        }

        public async Task Run()
        {
            var busId = 1;
            var deviceAddress = 0x48;
            var settings = new I2cConnectionSettings(busId, deviceAddress);

            using (var device = I2cDevice.Create(settings))
            {
                while (true)
                {
                    var temperatureCelsius = ParseToCelsius(device.ReadByte());
                    Console.WriteLine($"Current temperature: {temperatureCelsius} C");

                    var msg = CreateMessage(temperatureCelsius);
                    await deviceClient.SendEventAsync(msg);

                    await Task.Delay(2000);
                }
            }
        }

        private Message CreateMessage(double temperatureCelsius)
        {
            var temperatureData = new TemperatureData { Id = Guid.NewGuid().ToString(), DeviceId = "1", TemperatureCelsius = temperatureCelsius };
            var messageString = JsonConvert.SerializeObject(temperatureData);
            return new Message(Encoding.ASCII.GetBytes(messageString));
        }

        private double ParseToCelsius(byte analogicTemperature)
        {
            var rawTemperature = Convert.ToDouble(analogicTemperature);
            var ratedVoltage = 5 * (rawTemperature) / 255;
            var resistence = 10000 * ratedVoltage / (5 - ratedVoltage);
            var kelvinTemperature = 1 / (((Math.Log(resistence / 10000)) / 3950) + (1 / (273.15 + 25)));
            var celciusTemperature = kelvinTemperature - 273.15;

            return celciusTemperature;
        }
    }
}
