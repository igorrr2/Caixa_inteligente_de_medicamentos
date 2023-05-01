using Android.App;
using Android.OS;
using Android.Widget;
using System;
using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Protocol;

namespace CaixaInteligente
{
    [Activity(Label = "ComandosActivity")]
    internal class ComandosActivity : Activity
    {
        private IMqttClient mqttClient;
        private string clientId;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Comandos);
            Button btnDispararAlarme = FindViewById<Button>(Resource.Id.btnDispararAlarme);

            // Configurações do cliente MQTT
            var brokerHost = "test.mosquitto.org";
            var brokerPort = 1883;
            var mqttConfig = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerHost, brokerPort)
                .WithClientId(clientId)
                .Build();

            // Criação do cliente MQTT
            mqttClient = new MqttFactory().CreateMqttClient();
            mqttClient.ConnectAsync(mqttConfig).Wait();

            btnDispararAlarme.Click += OnAlarmButtonClicked;
        }
        private void OnAlarmButtonClicked(object sender, EventArgs e)
        {
            var topic = "TOPICO_SUBSCRIBE_TESTE_XAMARIN";
            var payload = "TESTE XAMARIN";

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            mqttClient.PublishAsync(message).Wait();
        }
    }
}