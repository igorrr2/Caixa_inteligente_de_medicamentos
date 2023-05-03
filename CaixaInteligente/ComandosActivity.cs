using Android.App;
using Android.OS;
using Android.Widget;
using System;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client;

namespace CaixaInteligente
{
    [Activity(Label = "ComandosActivity")]
    internal class ComandosActivity : Activity
    {
        private IMqttClient mqttClient2;
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
            mqttClient2 = new MqttFactory().CreateMqttClient();
            mqttClient2.ConnectAsync(mqttConfig).Wait();

            btnDispararAlarme.Click += OnAlarmButtonClicked;
            
            
            // Configurações do cliente MQTT

            var mqttClientOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("test.mosquitto.org", 1883)
            .Build())
            .Build();

            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            var mqttManager = new MqttManager(mqttClient);

            // connect to the MQTT broker
            mqttClient.StartAsync(mqttClientOptions);

            // subscribe to the desired topic
            var topic = "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID";
            mqttManager.SubscribeAsync(topic);
            
        }
        private void OnAlarmButtonClicked(object sender, EventArgs e)
        {
            var topic = "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP";
            var payload = "TESTE XAMARIN";

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            mqttClient2.PublishAsync(message).Wait();
        }
    }
}