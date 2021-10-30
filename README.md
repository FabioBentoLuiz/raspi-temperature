# Introduction

This is an .net Core experiment with the [Sunfounder Analog Temperature Sensor](https://docs.sunfounder.com/projects/sensorkit-v2-pi/en/latest/lesson_18.html) and my Raspberry Pi 3 B+.

I've decided to go some steps further and:

- send the read temperatures to [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/);
- route the messages from Azure IoT Hub to an Azure Function. The function implementation is in the folder [azure-function](/azure-function);
- persist the messages in a [Cosmos DB (API for MongoDB)](https://docs.microsoft.com/en-us/azure/cosmos-db/mongodb/mongodb-introduction);
- configure [Monitoring and Alerts](https://docs.microsoft.com/en-us/azure/iot-hub/tutorial-use-metrics-and-diags).

## Dependencies

```
dotnet add package Iot.Device.Bindings
dotnet add package Microsoft.Azure.Devices.Client
dotnet add package System.Threading
```

## Tips

This command is usefull to find the device addresses of the temperature sensor attached on the Raspberry Pi. In my case it is `0x48`.

```
i2cdetect 1
```

I've used the [vscode Remote Development](https://code.visualstudio.com/docs/remote/ssh) using SSH, so I could connect from my notebook to the Raspberry Pi then develop and debug there directly.

```
ssh pi@raspberrypi.local
```

## How to run this project

If you want to try it only locally with your Raspberry Pi, you should comment or remove the connection with Azure.

```
// var msg = CreateMessage(temperatureCelsius);
// await deviceClient.SendEventAsync(msg);
```

Otherwise, if you have your Azure environment configured you could set the env `RASPI_01_CS` with your Azure IoT Hub connection string.

```
dotnet run Program
```
