using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CaixaInteligente
{
    public class MqttManager
    {
        private readonly IManagedMqttClient _mqttClient;

        public MqttManager(IManagedMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;


        }

        private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            // handle the received MQTT message here
            Console.WriteLine($"Received message on topic {eventArgs.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload)}");
        }

        public async Task SubscribeAsync(string topic)
        {
            await _mqttClient.SubscribeAsync(topic);
        }

        public async Task UnsubscribeAsync(string topic)
        {
            await _mqttClient.UnsubscribeAsync(topic);
        }
    }
}