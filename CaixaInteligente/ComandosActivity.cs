using Android.App;
using Android.OS;
using Android.Widget;
using System;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client;
using Android.Content;
using Android.Preferences;

namespace CaixaInteligente
{
    [Activity(Label = "ComandosActivity")]
    public class ComandosActivity : Activity
    {
        private IMqttClient mqttClient2;
        private string clientId;
        private static TextView _statusAlarmeTextView;
        private bool alarmeAtivado;



        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Comandos);
            Button btnDispararAlarme = FindViewById<Button>(Resource.Id.btnDispararAlarme);
            _statusAlarmeTextView = FindViewById<TextView>(Resource.Id.statusAlarmeTextView);
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
            var mqttManager = new MqttManager(mqttClient, new ComandosActivity());

            // connect to the MQTT broker
            mqttClient.StartAsync(mqttClientOptions);

            // subscribe to the desired topic
            var topic = "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID";
            mqttManager.SubscribeAsync(topic);

            var topic2 = "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP";
            var payload = "STATUS ALARME";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic2)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            mqttClient2.PublishAsync(message).Wait();

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
        public void AtualizaStatusAlarme(string status, bool alarmeAtivado)
        {

            if (_statusAlarmeTextView != null)
            {
                _statusAlarmeTextView.Text = status;
                if (alarmeAtivado)
                    _statusAlarmeTextView.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                else
                    _statusAlarmeTextView.SetBackgroundColor(Android.Graphics.Color.Red);
                _statusAlarmeTextView.RequestLayout();

            }
        }
        protected override void OnResume()
        {
            base.OnResume();

            // Carrega o status do alarme
            alarmeAtivado = LoadAlarmeStatus();

            // Atualiza o TextView com o status atual do alarme
            _statusAlarmeTextView.Text = alarmeAtivado ? "Alarme Ativado" : "Alarme Desativado";
        }
        protected override void OnPause()
        {
            base.OnPause();

            // Salva o status do alarme
            SaveAlarmeStatus();
        }
        private void SaveAlarmeStatus()
        {
            // Salva o status do alarme usando as preferências compartilhadas do aplicativo
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            if (_statusAlarmeTextView.Text == "Alarme Ativado")
                editor.PutBoolean("alarmeAtivado", true);
            else
                editor.PutBoolean("alarmeAtivado", false);
            editor.Commit();
        }

        private bool LoadAlarmeStatus()
        {
            // Carrega o status do alarme das preferências compartilhadas do aplicativo
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            return prefs.GetBoolean("alarmeAtivado", false);
        }
    }
}