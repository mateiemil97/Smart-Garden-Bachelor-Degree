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

#define TEMPERATURE_INTERVAL_TIME_POST 5000

const String  api = "https://192.168.1.2:45457/api";
const String fingerPrint = "EF:4F:02:3E:EB:0B:F9:7A:F1:93:7C:70:3F:94:5F:52:24:26:34:12";

HTTPClient http;
WiFiClient cli;

float temperature;
float previousTemperature;

int moisture;
int previousMoisture;

bool systemState = true;

unsigned long temperatureTimeNow = millis();
unsigned long temperatureTimeTrigger = millis();

unsigned long currentTime = millis();


//WifiConnect wifi = WifiConnect("INTERNET","c6c202emov");

WifiConnect wifi = WifiConnect("MERCUSYS_98EB","matei123");



void setup() {
  wifi.Connect();
  Serial.begin(9600);
  
  pinMode(MOISTURE_SENSOR_PIN,INPUT);
  pinMode(TEMPERATURE_SENSOR_PIN ,INPUT);

  temperatureTimeTrigger = millis();
  
}


void loop() {

  if (WiFi.status() == WL_CONNECTED) { //Check WiFi connection status

    //PostSensorValue(1, "Moisture", 80);
//    moisture = ReadMoisture();
//    Serial.print("bla bla bla ");
//    Serial.println(moisture);

   temperature = ReadTemperature();

   currentTime = millis();
   Serial.print("previouse-temp");
   Serial.println(previousTemperature);
   Serial.print("tempTrigger");
   Serial.println(temperatureTimeTrigger);

   if((currentTime - temperatureTimeTrigger >= TEMPERATURE_INTERVAL_TIME_POST) && 
      ((temperature >= previousTemperature + 0.20) || (temperature <= previousTemperature - 0.20)))
      {
        PostSensorValue(1,"Temperature",temperature);
      }
   else if(((currentTime - temperatureTimeTrigger >= TEMPERATURE_INTERVAL_TIME_POST) && 
      ((temperature <= previousTemperature + 0.20) || (temperature >= previousTemperature - 0.20))))
   {
        temperatureTimeTrigger = millis();
        Serial.println("hit");
   }
   else
     {
      Serial.println("Not big difference of temperature or time not elapsed");
     }
      

//   if(systemState == true)
//   {
//     
//   }
//   else 
//   {
//    
//   }
   
    
     delay(3000);    //Send a request every 30 seconds
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
  Serial.println(JSONmessageBuffer);

  
  http.begin(api+"/systems/"+systemId+"/sensors",fingerPrint);      //Specify request destination
    http.addHeader("Content-Type", "application/json");  //Specify content-type header
 
    int httpCode = http.POST(JSONmessageBuffer);   //Send the request
    String payload = http.getString();                                        //Get the response payload
 
    Serial.println(httpCode);   //Print HTTP return code
    Serial.println(payload);    //Print request response payload

    previousTemperature = value;

    temperatureTimeTrigger = millis();
    
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
