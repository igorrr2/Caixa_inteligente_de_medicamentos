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
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;


namespace CaixaInteligente
{
    public class MqttManager
    {
        private readonly IManagedMqttClient _mqttClient;
        ComandosActivity _statusAlarme;

        public MqttManager(IManagedMqttClient mqttClient, ComandosActivity statusAlarme)
        {
            _mqttClient = mqttClient;
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
            _statusAlarme = statusAlarme;

        }

        private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            // handle the received MQTT message here
            Console.WriteLine($"Received message on topic {eventArgs.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload)}");
            string mensagem = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
            try
            {
                if (mensagem == "Alarme Desativado")
                    _statusAlarme.RunOnUiThread(() =>
                    {
                        _statusAlarme.AtualizaStatusAlarme(mensagem, false);
                    });
                else
                    _statusAlarme.RunOnUiThread(() =>
                    {
                        _statusAlarme.AtualizaStatusAlarme(mensagem, true);
                    });
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
               
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