#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <SPI.h>
#include "WifiConnect.h";
#include <string.h>
#include <OneWire.h>
#include <DallasTemperature.h>

//PIN 
#define TEMPERATURE_SENSOR_PIN 2
#define MOISTURE_SENSOR_PIN A0
#define RELAYS_PUMP_PIN 5

#define TEMPERATURE_INTERVAL_TIME_POST 5000
#define MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON 5000
#define MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF 10000

struct Board 
{ 
   bool registered; 
   int id; 
};

const String series = "AAAA";
bool registered; //if user registered system from app
Board board;

const String  api = "https://smart-garden.conveyor.cloud/api";
const String fingerPrint = "82:88:7C:B6:41:71:8B:04:67:A5:10:C2:34:40:24:04:78:A6:7E:55"; 

HTTPClient http;
WiFiClient cli;

float temperature;
float previousTemperature;

int moisture;
int previousMoisture;

bool systemState;

unsigned long temperatureTimeTrigger = millis();
unsigned long moistureTimeTrigger = millis();

unsigned long currentTime = millis();


WifiConnect wifi = WifiConnect("INTERNET","c6c202emov");

// WifiConnect wifi = WifiConnect("MERCUSYS_98EB","matei123");



void setup() {
  wifi.Connect();
  Serial.begin(9600);
  
  pinMode(MOISTURE_SENSOR_PIN,INPUT);
  pinMode(TEMPERATURE_SENSOR_PIN ,INPUT);
  pinMode(RELAYS_PUMP_PIN,OUTPUT);
  temperatureTimeTrigger = millis();
  // registered = CheckForRegisteredBoard(series);
  board = GetBoardByBoardSeries("AAAA");
  Serial.print("Id:");
  Serial.println(board.id);
  Serial.print("Registered:");
  Serial.println(board.registered);
}


void loop() {

  if (WiFi.status() == WL_CONNECTED && board.registered) { //Check WiFi connection status

   systemState = CheckForRemoteStateChanges(board.id);
   Serial.print("System state");
   Serial.println(systemState);
   temperature = ReadTemperature();
   moisture = ReadMoisture();
   
   currentTime = millis();
   
   Serial.print("previouse-temp");
   Serial.println(previousTemperature);
   Serial.print("tempTrigger");
   Serial.println(temperatureTimeTrigger);

   if(systemState == true)
   {
      digitalWrite(RELAYS_PUMP_PIN,HIGH); 
   }
   else if(systemState == false)
   {
      digitalWrite(RELAYS_PUMP_PIN,LOW);
   }

   if((currentTime - temperatureTimeTrigger >= TEMPERATURE_INTERVAL_TIME_POST) && 
      ((temperature >= previousTemperature + 1) || (temperature <= previousTemperature - 1)))
      {
        PostSensorValue(board.id,"Temperature",temperature);
      }
   else if(((currentTime - temperatureTimeTrigger >= TEMPERATURE_INTERVAL_TIME_POST) && 
      ((temperature <= previousTemperature + 1) || (temperature >= previousTemperature - 1))))
   {
        temperatureTimeTrigger = millis();
        Serial.println("Reinitialize temperature trigger");
   }
   else
     {
      Serial.println("Not big difference of temperature or time not elapsed");
     }
      
    //check for moisture
   if(systemState == true)
   {
     Serial.print("previouse-moisture");
     Serial.println(previousMoisture);
     Serial.print("moistureTrigger");
     Serial.println(moistureTimeTrigger);
     if((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON) && 
      ((moisture >= previousMoisture + 5) || (moisture <= previousMoisture - 5)))
      {
        PostSensorValue(board.id,"Moisture",moisture);
      }
   else if(((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON) && 
      ((moisture <= previousMoisture + 5) || (moisture >= previousMoisture - 5))))
   {
        moistureTimeTrigger = millis();
        Serial.println("Reinitialize moisture trigger");
   }
   else
     {
      Serial.println("Not big difference of moisture or time not elapsed");
     }
   }
   else if(systemState == false)
   {
    if((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF) && 
      ((moisture >= previousMoisture + 5) || (moisture <= previousMoisture - 5)))
      {
        PostSensorValue(board.id,"Moisture",moisture);
      }
   else if(((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF) && 
      ((moisture <= previousMoisture + 5) || (moisture >= previousMoisture - 5))))
   {
        moistureTimeTrigger = millis();
        Serial.println("Reinitialize moisture trigger");
   }
   else
     {
      Serial.println("Not big difference of moisture or time not elapsed");
     }
   }
   
    
     delay(30000);    //Send a request every 30 seconds
  }
}


//function to post sensors value

void PostSensorValue(int systemId, String type, float value)
{
  Serial.println("making POST request");
  String contentType = "application/json";

  const int capacity = JSON_OBJECT_SIZE(3) + 95;
  StaticJsonBuffer<capacity> JSONbuffer;
  JsonObject& JSONencoder = JSONbuffer.createObject();
  
  JSONencoder["SystemId"] = systemId;
  JSONencoder["Type"] = type;
  JSONencoder["Value"] = value;

  char JSONmessageBuffer[capacity];
  JSONencoder.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
  
  http.begin(api+"/systems/"+systemId+"/sensors",fingerPrint);      //Specify request destination
    http.addHeader("Content-Type", "application/json");  //Specify content-type header
 
    int httpCode = http.POST(JSONmessageBuffer);   //Send the request
    String payload = http.getString();                                        //Get the response payload
 
    Serial.println(httpCode);   //Print HTTP return code
    Serial.println(payload);    //Print request response payload

    if(type == "Temperature")
    {
      previousTemperature = value;
      temperatureTimeTrigger = millis();
    }
    else if(type == "Moisture")
    {
      previousMoisture = value;
      moistureTimeTrigger = millis();
    }
    http.end();  //Close connection
    
}

int ReadMoisture()
{
  int moisture = analogRead(MOISTURE_SENSOR_PIN);
  Serial.print("Moisture");
  Serial.println(moisture);

  int moisturePercentage = map(moisture,0,1023,250,0);
  Serial.print(moisturePercentage);
  Serial.println("%");
  return moisturePercentage;
}

float ReadTemperature()
{
  // Setup a oneWire instance to communicate with any OneWire device
  OneWire oneWire(TEMPERATURE_SENSOR_PIN);  

  // Pass oneWire reference to DallasTemperature library
  DallasTemperature sensors(&oneWire);

  sensors.requestTemperatures(); 

  //print the temperature in Celsius
  Serial.print("Temperature: ");
  Serial.print(sensors.getTempCByIndex(0));
  Serial.println("C");
  return sensors.getTempCByIndex(0);
}

bool CheckForRemoteStateChanges(int systemId)
{
  bool working = 0;
  
  http.begin(api+"/systems/"+systemId+"/currentState",fingerPrint); //Specify the URL
  int httpCode = http.GET();             
    if (httpCode > 0) { //Check for the returning code
 
        String payload = http.getString();
        Serial.println(httpCode);
        

      const int capacity = JSON_OBJECT_SIZE(3) + 103;;
      StaticJsonBuffer<capacity> JSONbuffer;
      JsonObject& root = JSONbuffer.parseObject(payload);
      
      // Parameters
      working = root["working"]; 
      // Serial.println(working);
    }
    else 
    {
      Serial.println("Error on HTTP request");
    }
    http.end(); //Free the resources
    
    return working;
}
//bool CheckForRegisteredBoard(String series)
//{
//  bool registered;
//  http.begin(api+"/boardsseries/"+series,fingerPrint); //Specify the URL
//  int httpCode = http.GET();             
//    if (httpCode > 0) { //Check for the returning code
// 
//        String payload = http.getString();
//        Serial.println(httpCode);
//        
//
//      const int capacity = JSON_OBJECT_SIZE(3)+93;
//      StaticJsonBuffer<capacity> JSONbuffer;
//      JsonObject& root = JSONbuffer.parseObject(payload);
//      
//      // Parameters
//      registered = root["registered"]; 
//      // Serial.println(working);
//    }
//    else 
//    {
//      Serial.println("Error on HTTP request");
//    }
//    http.end(); //Free the resources
//    
//    return registered;
//}

Board GetBoardByBoardSeries(String series)
{
  Board board;
  http.begin(api+"/boardsseries/"+series,fingerPrint); //Specify the URL
  int httpCode = http.GET();             
    if (httpCode > 0) { //Check for the returning code
 
        String payload = http.getString();
        Serial.println(httpCode);
        

      const int capacity = JSON_OBJECT_SIZE(2)+61;
      StaticJsonBuffer<capacity> JSONbuffer;
      JsonObject& root = JSONbuffer.parseObject(payload);
      Serial.println(payload);
      // Parameters
      board.registered = root["registered"]; 
      board.id = root["irigationSystemId"];
      int a = root["irigationSystemId"];
      Serial.println(board.id);
    }
    else 
    {
      Serial.println("Error on HTTP request");
    }
    http.end(); //Free the resources
          Serial.println(board.id);
    return board;
}
