#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include "WifiConnect.h";
#include <string.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include <SoftwareSerial.h>
#include <NTPClient.h>
#include <WiFiUdp.h>


int D5 = 14, D6 = 12;
SoftwareSerial s(D6,D5);

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP);
// For UTC +2.00 : 2 * 60 * 60 : 7200
const long utcOffsetInSeconds = 7200;

//PIN 
#define TEMPERATURE_SENSOR_PIN 2
#define MOISTURE_SENSOR_PIN A0
#define RELAYS_PUMP_PIN 5


#define RELAYS_WATER_SWITCH_P0_PIN 4
#define RELAYS_WATER_SWITCH_P1_PIN 13

#define TEMPERATURE_INTERVAL_TIME_POST 60000
#define MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON 30000
#define MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF 60000

struct Board 
{ 
   bool registered; 
   int id; 
};

struct Moisture
{
  const char* port;
  int value;
};

struct Zone 
{
  int startMoisture;
  int stopMoisture;
  bool waterSwitch;
};

struct Schedule
{
  int temperatureMin;
  int temperatureMax;
  bool manual;
  char* startTime;
};

  int temperatureMin;
  int temperatureMax;
  bool manual;
  const char* startTime;
  
const String series = "AAAA";
bool registered; //if user registered system from app
Board board;

const String  api = "https://smart-garden.conveyor.cloud/api";
const String fingerPrint = "82:88:7C:B6:41:71:8B:04:67:A5:10:C2:34:40:24:04:78:A6:7E:55"; 

HTTPClient http;
//WiFiClient cli;

bool systemState;

float temperature;
unsigned long temperatureTimeTrigger;

Moisture moisture[2];
unsigned long moistureTimeTrigger;

Schedule schedule;

unsigned long currentTime = millis();

Zone zones[2];

 WifiConnect wifi = WifiConnect("DIGI_ce10a8","989d31ef");

 //WifiConnect wifi = WifiConnect("MERCUSYS_98EB","matei123");


void setup() {
  
  wifi.Connect();
  Serial.begin(9600);
  //serial with uno
  s.begin(9600);

  timeClient.begin();
  timeClient.setTimeOffset(utcOffsetInSeconds);

  pinMode(MOISTURE_SENSOR_PIN,INPUT);
  pinMode(TEMPERATURE_SENSOR_PIN ,INPUT);
  pinMode(RELAYS_PUMP_PIN,OUTPUT);
  pinMode(RELAYS_WATER_SWITCH_P0_PIN,OUTPUT);
  pinMode(RELAYS_WATER_SWITCH_P1_PIN,OUTPUT);

  temperatureTimeTrigger = 0;
  moistureTimeTrigger = millis();
  
  // registered = CheckForRegisteredBoard(series);
  board = GetBoardByBoardSeries("BBBB");
  Serial.print("Id:");
  Serial.println(board.id);
  Serial.print("Registered:");
  Serial.println(board.registered);



  digitalWrite(13,HIGH);
  digitalWrite(15,HIGH);
}


void loop() {
 
 // timeClient.update();
 // int currentTimeFromServer = ((timeClient.getHours() * 3600) + (timeClient.getMinutes()* 60));
    Serial.println(ESP.getFreeHeap());

//  int seconds = TransformTimeInSeconds(schedule.startTime);
    ReadMoisture();
//
 if (WiFi.status() == WL_CONNECTED) //&& board.registered)  //Check WiFi connection status
 {
  GetScheduleBySystem(board.id);
  Serial.println(schedule.temperatureMin);
  currentTime = millis();
  
  systemState = CheckForRemoteStateChanges(board.id);
  Serial.print("System state");
  Serial.println(systemState);
  
  GetZonesBySystemId(board.id);
  
  
  
  temperature = ReadTemperature();
  Serial.println(temperature);
  
  Serial.print("tempTrigger");
  Serial.println(temperatureTimeTrigger);
  if((currentTime - temperatureTimeTrigger >= TEMPERATURE_INTERVAL_TIME_POST))
     {
       PostSensorValue(board.id,"Temperature","D0",temperature);
       temperatureTimeTrigger = millis();
     }
   else
    {
     Serial.println("time not elapsed");
     }

  if(systemState == true)
   {
      digitalWrite(RELAYS_PUMP_PIN,HIGH);
      digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,zones[0].waterSwitch);
      digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,zones[1].waterSwitch); 
      Serial.print("Zone 1:");
      Serial.println(zones[1].waterSwitch);
   }
   else if(systemState == false)
   {
      digitalWrite(RELAYS_PUMP_PIN,LOW);
      digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
       digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,LOW); 
   }
   
   //check for moisture
  
  if(systemState == true)
   {
   for(int i=0;i<2;i++) {
       Serial.print("previouse-moisture");
       //Serial.println(previousMoisture);
      Serial.print("moistureTrigger");    
            if((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON))
            {
              PostSensorValue(board.id,"Moisture",moisture[i].port,moisture[i].value);
              if(i==1){
              moistureTimeTrigger = millis();
              }
            }
         else
           {
            Serial.println("moisture time not elapsed");
           }
     }
    }
       
       else if(systemState == false)
       {
        for(int i=0;i<2;i++) {
          if((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF))
            {
             PostSensorValue(board.id,"Moisture",moisture[i].port,moisture[i].value);
             if(i==1){
              moistureTimeTrigger = millis();
              }
           }
         else
          {
          Serial.println("moisture time not elapsed");
          }
         }
     }
  }

}


//function to post sensors value

void PostSensorValue(int systemId,String type, String port, float value)
{
  Serial.println("making POST request");
  String contentType = "application/json";

  const int capacity = JSON_OBJECT_SIZE(1) + 33;
  StaticJsonBuffer<capacity> JSONbuffer;
  JsonObject& JSONencoder = JSONbuffer.createObject();
  
  JSONencoder["Value"] = value;

  char JSONmessageBuffer[capacity];
  JSONencoder.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
  
  http.begin(api+"/systems/"+systemId+"/measurements/"+port,fingerPrint);      //Specify request destination
    http.addHeader("Content-Type", "application/json");  //Specify content-type header
 
    int httpCode = http.POST(JSONmessageBuffer);   //Send the request
    String payload = http.getString();                                        //Get the response payload
 
    Serial.println(httpCode);   //Print HTTP return code
    Serial.println(payload);    //Print request response payload

    http.end();  //Close connection
    
}

void ReadMoisture() {
  
  //timer0_write(ESP.getCycleCount() + 40000000L);
  const size_t capacity = JSON_ARRAY_SIZE(3) + 3*JSON_OBJECT_SIZE(2) + 215;
  DynamicJsonBuffer jsonBuffer(capacity);
  
  JsonArray& root = jsonBuffer.parseArray(s);
  Serial.println("citireeeee");

  Serial.println(moisture[0].port);
 Serial.println(moisture[0].value);
   if (root.success()){
    moisture[0].port = "A0";
    moisture[0].value = root[0]["v"];
    
    moisture[1].port = "A1";
    moisture[1].value = root[1]["v"]; 
   Serial.println("JSON received and parsed");
   root.prettyPrintTo(Serial);
   Serial.println("---------------------xxxxx--------------------");

  }
 
}

float ReadTemperature()
{
  // Setup a oneWire instance to communicate with any OneWire device
  OneWire oneWire(TEMPERATURE_SENSOR_PIN);  

  // Pass oneWire reference to DallasTemperature library
  DallasTemperature sensors(&oneWire);

  sensors.requestTemperatures(); 

  //print the temperature in Celsius
  //Serial.print("Temperature: ");
 // Serial.print(sensors.getTempCByIndex(0));
 // Serial.println("C");
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
      board.registered = (bool)root["registered"]; 
      board.id = (int)root["irigationSystemId"];
      int a = (int)root["irigationSystemId"];
      Serial.println(board.id);
    }
    else 
    {
      Serial.println("Error on HTTP request");
    }
    http.end(); //Free the resources
    
    return board;
}

void GetZonesBySystemId(int systemId)
{
  http.begin(api+"/systems/"+systemId+"/zones/arduino",fingerPrint); //Specify the URL
  int httpCode = http.GET();             
    if (httpCode > 0) { //Check for the returning code
 
      String payload = http.getString();
      Serial.println(httpCode);
      Serial.println(payload);
      const size_t capacity = JSON_ARRAY_SIZE(2) + 2*JSON_OBJECT_SIZE(3) + 120;
      DynamicJsonBuffer jsonBuffer(capacity);
           
      JsonArray& root = jsonBuffer.parseArray(payload);
      
      JsonObject& root_0 = root[0];
      zones[0].startMoisture = root_0["moistureStart"]; 
      zones[0].stopMoisture = root_0["moistureStop"]; 
      zones[0].waterSwitch = root_0["waterSwitch"]; 
      
      JsonObject& root_1 = root[1];
      zones[1].startMoisture = root_1["moistureStart"]; 
      zones[1].stopMoisture = root_1["moistureStop"];
      zones[1].waterSwitch = root_1["waterSwitch"];
    }
    else 
    {
      Serial.println("Error on HTTP request");
    }
    http.end(); //Free the resources
}
void GetScheduleBySystem(int systemId)
  {
    http.begin(api+"/systems/"+systemId+"/schedule",fingerPrint); //Specify the URL
    int httpCode = http.GET();
        Serial.print("code:");    
        Serial.println(httpCode);     
      if (httpCode == 200) { //Check for the returning code
        String payload = http.getString();
        Serial.println(payload);
  
        const size_t capacity = JSON_OBJECT_SIZE(7) + 238;
        DynamicJsonBuffer jsonBuffer(capacity);
             
        JsonObject& root = jsonBuffer.parseObject(payload);
  

        Serial.println("temperatureMin");
       Serial.println((int)root["temperatureMin"]);
        Serial.println("temperatureMax");
        Serial.println((int)root["temperatureMax"]);
        Serial.println("Manual");
        Serial.println((bool)root["manual"]);
        
        startTime = root["start"]; // "2019-12-02T21:51:00"
        temperatureMin = (int)root["temperatureMin"]; // 22
        temperatureMax = (int)root["temperatureMax"]; // 32
        manual = (bool)root["manual"]; // true
       }
      
      else 
      {
        Serial.println("Error on HTTP request");
      }
      http.end(); //Free the resources
  }
  int TransformTimeInSeconds(String date)
  {
     //Extract date
     int splitT = date.indexOf("T");
      // Extract time

    String  timeFromDate = date.substring(splitT+1, date.lastIndexOf(":" +1));
  
    int firstColon = timeFromDate.indexOf(":");
    int secondColon = timeFromDate.lastIndexOf(":");
    
    // get the substrings for hour, minute second:
    String hourString = timeFromDate.substring(0, firstColon);
    String minString = timeFromDate.substring(firstColon + 1, secondColon);
    String secString = timeFromDate.substring(secondColon + 1);

    int hours = hourString.toInt();
    int minutes = minString.toInt();
    int seconds = minString.toInt();
    
    int totalSeconds = (hours * 3600) + (minutes * 60);
   // Serial.println(totalSeconds);
    return totalSeconds;
  }
