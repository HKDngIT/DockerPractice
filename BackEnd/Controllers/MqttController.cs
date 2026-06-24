using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Server;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MqttController(MqttServer mqttServer) : ControllerBase
    {
        [HttpPost("publish")]
        public async Task<IActionResult> Publish(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)   
                .Build();

            await mqttServer.InjectApplicationMessage(
                new InjectedMqttApplicationMessage(message) { SenderClientId = "API" });

            return Ok();
        }
    }
}
