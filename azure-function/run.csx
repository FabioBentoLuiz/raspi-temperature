using System;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using MongoDB;
using MongoDB.Driver;
using System.Security.Authentication;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



public static void Run(string myIoTHubMessage, ILogger log)
{
    log.LogInformation($"C# IoT Hub trigger function processed a message: {myIoTHubMessage}");
    var raw_obj = JObject.Parse(myIoTHubMessage);
    var id = (string)raw_obj["Id"];
    var deviceId = (string)raw_obj["DeviceId"];
    var celsius = (string)raw_obj["TemperatureCelsius"];
    Cosmos cosmos = new Cosmos(id, deviceId, Convert.ToDouble(celsius));
    cosmos.pushData();
}

public class Cosmos
{
    string id = "";
    string deviceId = "";
    double celsius = 0;
    public Cosmos(string id, string deviceId, double celsius)
    {
        this.id = id;
        this.deviceId = deviceId;
        this.celsius = celsius;
    }
    public void pushData()
    {
        MainAsync().Wait();
    }
    public async Task MainAsync()
    {
        string connectionString = @"<set your connection string here>";
        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        var mongoClient = new MongoClient(settings);
        IMongoDatabase db = mongoClient.GetDatabase("iot");
        var icollection = db.GetCollection<TemperatureData>("messages");
        var document = new TemperatureData()
        {
            Id = id,
            DeviceId = deviceId,
            TemperatureCelsius = celsius
        };
        await icollection.InsertOneAsync(document);
    }
}

public class TemperatureData
{
    public string Id { get; set; }
    public string DeviceId { get; set; }
    public double TemperatureCelsius { get; set; }
}