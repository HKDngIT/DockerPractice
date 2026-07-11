using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MqttServer>(sp =>
{
    var factory = new MqttServerFactory();
    var options = new MqttServerOptionsBuilder()
        .WithDefaultEndpoint()      // Port 1883
        .Build();
    return factory.CreateMqttServer(options);
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// MQTT Broker starten und beim App-Stop wieder beenden
var mqttServer = app.Services.GetRequiredService<MqttServer>();

// Log wenn eine Nachricht gepublished wird
mqttServer.InterceptingPublishAsync += e =>
{
    app.Logger.LogInformation(
        "MQTT Nachricht empfangen: Topic={Topic}, Payload={Payload}, Client={ClientId}",
        e.ApplicationMessage.Topic,
        e.ApplicationMessage.ConvertPayloadToString(),
        e.ClientId);
    return Task.CompletedTask;
};

mqttServer.ValidatingConnectionAsync += e =>
{
    if (e.ClientId != "ValidClientId")
    {
        e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
    }

    if (e.UserName != "ValidUser")
    {
        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
    }

    if (e.Password != "SecretPassword")
    {
        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
    }

    return Task.CompletedTask;
};

await mqttServer.StartAsync();
app.Logger.LogInformation("MQTT Broker gestartet auf Port 1883");

app.Lifetime.ApplicationStopping.Register(() =>
{
    mqttServer.StopAsync().GetAwaiter().GetResult();
    mqttServer.Dispose();
});

app.Run();
