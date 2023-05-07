#include <arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <ESP32Servo.h>  //Biblioteca utilizada

int buz=0;
const int buzzer = 23;
bool AlarmeAtivado = false;

////////////////////////////////////////////////////////////////////////////////////////////////////////
/* Definicoes para o MQTT */
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ADICIONAR_HORARIO "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ADICIONAR_HORARIO"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_REMOVER_HORARIO "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_REMOVER_HORARIO"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_STATUS_ALARME "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_STATUS_ALARME"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID"
#define ID_MQTT  "dc44fae4-a83f-4303-9e8c-44957228882c"     //id mqtt (para identificação de sessão)
//#define ID_MQTT  "mqttdash-ea4e25ea"
////////////////////////////////////////////////////////////////////////////////////////////////////////

const char* SSID     = "igor"; // SSID / nome da rede WI-FI que deseja se conectar
const char* PASSWORD = "123456ie"; // Senha da rede WI-FI que deseja se conectar

const char* BROKER_MQTT = "test.mosquitto.org";
int BROKER_PORT = 1883; // Porta do Broker MQTT

//Variáveis e objetos globais
WiFiClient espClient; // Cria o objeto espClient
PubSubClient MQTT(espClient); // Instancia o Cliente MQTT passando o objeto espClient


/* Prototypes */
void initWiFi(void);
void initMQTT(void);
void mqtt_callback(char* topic, byte* payload, unsigned int length);
void reconnectMQTT(void);
void reconnectWiFi(void);
void VerificaConexoesWiFIEMQTT(void);

/*
   Implementações
*/

/* Função: inicializa e conecta-se na rede WI-FI desejada
   Parâmetros: nenhum
   Retorno: nenhum
*/
void initWiFi(void)
{
  delay(10);
  Serial.println("------Conexao WI-FI------");
  Serial.print("Conectando-se na rede: ");
  Serial.println(SSID);
  Serial.println("Aguarde");

  reconnectWiFi();
}


/* Função: inicializa parâmetros de conexão MQTT(endereço do
           broker, porta e seta função de callback)
   Parâmetros: nenhum
   Retorno: nenhum
*/
void initMQTT(void)
{
  MQTT.setServer(BROKER_MQTT, BROKER_PORT);   //informa qual broker e porta deve ser conectado
  MQTT.setCallback(mqtt_callback);            //atribui função de callback (função chamada quando qualquer informação de um dos tópicos subescritos chega)
}

/* Função: função de callback
           esta função é chamada toda vez que uma informação de
           um dos tópicos subescritos chega)
   Parâmetros: nenhum
   Retorno: nenhum
*/

void mqtt_callback(char* topic, byte* payload, unsigned int length)
{
  String msg;
  /* obtem a string do payload recebido */
  for (int i = 0; i < length; i++)
  {
    char c = (char)payload[i];
    msg += c;
  }

  Serial.print("Chegou a seguinte string via MQTT: ");
  Serial.println(msg);
  Serial.print("topico: ");
  Serial.println(topic);

  /* toma ação dependendo da string recebida */
   if(msg == "STATUS ALARME"){
    //se alarme está ativado
     if(AlarmeAtivado == true)
     MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Ativado");
     //se alarme está desativado
     else
     MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Desativado");
  }
  else if(msg == "A"){
    //se alarme está ativado
    if(AlarmeAtivado == true){
    noTone(buzzer);
    AlarmeAtivado = false;
    MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Desativado");
    }
    //se alarme está desativado
    else{
    tone(buzzer,2000);
    AlarmeAtivado = true;
    MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Ativado");
    }    
  }
  
  }
/* Função: reconecta-se ao broker MQTT (caso ainda não esteja conectado ou em caso de a conexão cair)
           em caso de sucesso na conexão ou reconexão, o subscribe dos tópicos é refeito.
   Parâmetros: nenhum
   Retorno: nenhum
*/
void reconnectMQTT(void)
{
  while (!MQTT.connected())
  {
    Serial.print("* Tentando se conectar ao Broker MQTT: ");
    Serial.println(BROKER_MQTT);
    if (MQTT.connect(ID_MQTT))
    {
      Serial.println("Conectado com sucesso ao broker MQTT!");
      MQTT.subscribe(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ADICIONAR_HORARIO);
      MQTT.subscribe(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_REMOVER_HORARIO);
      MQTT.subscribe(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_STATUS_ALARME);
      MQTT.subscribe(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP);
    }
    else
    {
      Serial.println("Falha ao reconectar no broker.");
      Serial.println("Havera nova tentatica de conexao em 2s");
      delay(2000);
    }
  }  
}

/* Função: verifica o estado das conexões WiFI e ao broker MQTT.
           Em caso de desconexão (qualquer uma das duas), a conexão
           é refeita.
   Parâmetros: nenhum
   Retorno: nenhum
*/
void VerificaConexoesWiFIEMQTT(void)
{
  if (!MQTT.connected())
    reconnectMQTT(); //se não há conexão com o Broker, a conexão é refeita

  reconnectWiFi(); //se não há conexão com o WiFI, a conexão é refeita
}

/* Função: reconecta-se ao WiFi
   Parâmetros: nenhum
   Retorno: nenhum
*/
void reconnectWiFi(void)
{
  //se já está conectado a rede WI-FI, nada é feito.
  //Caso contrário, são efetuadas tentativas de conexão
  if (WiFi.status() == WL_CONNECTED)
    return;

  WiFi.begin(SSID, PASSWORD); // Conecta na rede WI-FI

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(100);
    Serial.print(".");
  }

  Serial.println();
  Serial.print("Conectado com sucesso na rede ");
  Serial.print(SSID);
  Serial.println("\nIP obtido: ");
  Serial.println(WiFi.localIP());
}


void setup() {
  Serial.begin(9600); //Enviar e receber dados em 9600 baud
  delay(1000);
  Serial.println("Disciplina IoT2: acesso a nuvem via ESP32");
  delay(1000);
  randomSeed(analogRead(0));
  pinMode(buzzer,OUTPUT);
  /* Inicializa a conexao wi-fi */
  initWiFi();

  /* Inicializa a conexao ao broker MQTT */
  initMQTT();
}

// the loop function runs over and over again forever
void loop() {

  string horarioAtual = horarioAtual;
  
 
  //se deu horário de tomar remédio aciona alarme e alarme está desativado
  //if(AlarmeAtivado == false && horario[i]==horarioAtual)//acrescentar verificação do horário
    //MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP, "A");
  //se alarme está ativado e detectou que usuário pegou o remédio, desativa o alarme
  //if(AlarmeAtivado == true )
    //MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP, "A");
/* garante funcionamento das conexões WiFi e ao broker MQTT */
  VerificaConexoesWiFIEMQTT();
  /* keep-alive da comunicação com broker MQTT */
  MQTT.loop();
  delay(100);
}
