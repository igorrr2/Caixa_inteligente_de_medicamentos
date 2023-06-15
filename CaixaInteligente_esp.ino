#include <arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <ESP32Servo.h>  //Biblioteca utilizada
#include <EEPROM.h>
#include "time.h"

#define SOUND_SPEED 0.034
#define CM_TO_INCH 0.393701

int quantidade = 8;
int registradorQuantiadade = 0;
int buz=0;
const int buzzer = 13;
bool AlarmeAtivado = false;
bool match = false;
int address = 0; // Endereço de memória para armazenar os dados

long duration;
float distanceCm;
float distanceInch;

const int trigPin = 5;
const int echoPin = 18;

const char* ntpServer = "pool.ntp.org";
const long  gmtOffset_sec = 0;

//nao esqueca de ajustar o fuso
const int   daylightOffset_sec = -3600*3;

////////////////////////////////////////////////////////////////////////////////////////////////////////
/* Definicoes para o MQTT */
#define TOPICO_PUBLISH_CAIXA_INTELIGENTE_ADICIONAR_HORARIO "TOPICO_PUBLISH_CAIXA_INTELIGENTE_ADICIONAR_HORARIO"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ADICIONAR_HORARIO "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ADICIONAR_HORARIO"
#define TOPICO_PUBLISH_CAIXA_INTELIGENTE_REMOVER_HORARIO "TOPICO_PUBLISH_CAIXA_INTELIGENTE_REMOVER_HORARIO"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_REMOVER_HORARIO "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_REMOVER_HORARIO"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_STATUS_ALARME "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_STATUS_ALARME"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP"
#define TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID "TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID"
#define ID_MQTT  "049ddd24-f13a-45a2-8e1d-bf77455fe3b3"     //id mqtt (para identificação de sessão)
//#define ID_MQTT  "mqttdash-ea4e25ea"
////////////////////////////////////////////////////////////////////////////////////////////////////////

const char* SSID     = "NOME_DA_REDE"; // SSID / nome da rede WI-FI que deseja se conectar
const char* PASSWORD = "SENHA_DA_REDE"; // Senha da rede WI-FI que deseja se conectar

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
    tone(buzzer,1000);
    AlarmeAtivado = true;
    MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Ativado");
    }    
  }
  //METODO DE ADIÇÃO DE HORARIO
  else if(msg[0] == 'H'){
    
    String valor; 
    int data = 0; // Dado a ser armazenado

    //Separa a string
    for(int i =1; msg[i] != '\0'; i++){
      char c = msg[i];
      valor += c;
    }

    //Converte string para int
    data = valor.toInt();

    Serial.println(data);

    //grava a string
    EEPROM.writeInt(quantidade, data); // Grava o dado na memória flash
    quantidade += 8;
    EEPROM.writeInt(registradorQuantiadade, quantidade);
    EEPROM.commit();

    Serial.println(quantidade);

    //Parte que ler os dados
    int retrievedData = EEPROM.readInt(quantidade-8); // Lê o dado armazenado na memória flash   
    char temp[16];
    itoa(retrievedData, temp, 10);
    MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ADICIONAR_HORARIO, temp);
  }
  //METODO DE EXCLUSÃO DE HORARIO
  else if(msg[0] =='R'){

    String valor; 
    int data = 0; // Dado a ser armazenado

    //Separa a string
    for(int i =1; msg[i] != '\0'; i++){
      char c = msg[i];
      valor += c;
    }

    //Converte string para int
    data = valor.toInt();

    int numeroRegistros = EEPROM.readInt(registradorQuantiadade);
    //Excluir o valor da memoria
    for(int i=8;i<numeroRegistros;i+=8){
      int retrievedData = EEPROM.readInt(i); // Lê o dado armazenado na memória flash   

      if(retrievedData == data){
        char temp[16];
        itoa(retrievedData, temp, 10);
        MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_REMOVER_HORARIO, temp);
        EEPROM.writeInt(i, 0); // Grava o dado na memória flash //trabalha com octual????
        EEPROM.commit();
      }
    }

  }
  else if(msg[0]=='G'){

    int numeroRegistros = EEPROM.readInt(registradorQuantiadade);
      for(int i=8;i<numeroRegistros; i+=8){
            int retrievedData = EEPROM.readInt(i); // Lê o dado armazenado na memória flash 
            if(retrievedData != 0){
            char temp[16];
            itoa(retrievedData, temp, 10);
            MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ESP, temp);  
            }
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

  configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);

  Serial.println();
  Serial.print("Conectado com sucesso na rede ");
  Serial.print(SSID);
  Serial.println("\nIP obtido: ");
  Serial.println(WiFi.localIP());

  configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);
  printLocalTime();
}


void printLocalTime()
{
  struct tm timeinfo;
  if(!getLocalTime(&timeinfo)){
    Serial.println("Falha ao obter a hora");
    return;
  }
  Serial.println(&timeinfo, "%H%M");
}


String getLocalTime()
{
  struct tm timeinfo;
  if(!getLocalTime(&timeinfo)){
    Serial.println("Falha ao obter a hora");
    return "";
  }
  String tempo;
  char horaminuto[5];
  strftime(horaminuto,5, "%H%M", &timeinfo);

  for(int i =0; horaminuto[i] != '\0'; i++){
  char c = horaminuto[i];
  tempo += c;
  }
  return tempo;
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

  EEPROM.begin(512); // Inicializa a EEPROM com tamanho de 512 bytes

  pinMode(trigPin, OUTPUT); // Sets the trigPin as an Output
  pinMode(echoPin, INPUT); // Sets the echoPin as an Input

}

// the loop function runs over and over again forever
void loop() {

  // Clears the trigPin
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  // Sets the trigPin on HIGH state for 10 micro seconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  
  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH);
  
  // Calculate the distance
  distanceCm = duration * SOUND_SPEED/2;

  Serial.print("Distance (cm): ");
  Serial.println(distanceCm);

  String tempo;
  tempo = getLocalTime();
  Serial.println(tempo);
  int TempoInt = tempo.toInt();
  Serial.println(TempoInt);

  int numeroRegistros = EEPROM.readInt(registradorQuantiadade);

  //Faz uma busca na memoria para verificar se algum dos horarios adicionados é igual ao horario atual e se for, é ativado o alarme
  for(int i=8;i<numeroRegistros; i+=8){
    
    int retrievedData = EEPROM.readInt(i); // Lê o dado armazenado na memória flash 
    if(retrievedData != 0 && TempoInt == retrievedData && AlarmeAtivado == false){

      EEPROM.writeInt(i, 0); // Grava o dado na memória flash 
      EEPROM.commit();
      tone(buzzer,1000);
      AlarmeAtivado = true;
      MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Ativado");
    }


  }

  //Se o alrme estiver ativo e o medicamento for retirado da caixa, o alarme é desligado
  if(AlarmeAtivado == true){
    if(distanceCm > 5){
      noTone(buzzer);
      AlarmeAtivado = false;
      MQTT.publish(TOPICO_SUBSCRIBE_CAIXA_INTELIGENTE_ANDROID, "Alarme Desativado");
    }
  }
  
/* garante funcionamento das conexões WiFi e ao broker MQTT */
  VerificaConexoesWiFIEMQTT();
  /* keep-alive da comunicação com broker MQTT */
  MQTT.loop();
  delay(100);
}