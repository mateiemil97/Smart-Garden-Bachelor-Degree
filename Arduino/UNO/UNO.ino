#include <SoftwareSerial.h>
#include <ArduinoJson.h>
SoftwareSerial s(5,6);
#define MOISTURE_SENSOR_PIN_A0 A0
#define MOISTURE_SENSOR_PIN_A1 A1

  int moistureA0;
  int moistureA1;

  int moisturePertange_A0;
  int moisturePertange_A1;

  int map_low =686;
  int map_high =350;

void setup() {
  s.begin(9600);
  Serial.begin(9600);
  pinMode(MOISTURE_SENSOR_PIN_A0,INPUT);
  pinMode(MOISTURE_SENSOR_PIN_A1,INPUT);
}

void loop() {
  ReadMoisture();
  SendMoistureSerial();
}

void ReadMoisture()
{
  moistureA0 = analogRead(MOISTURE_SENSOR_PIN_A0);
  moistureA1 = analogRead(MOISTURE_SENSOR_PIN_A1);

  moisturePertange_A0 = map(moistureA0,map_low,map_high,0,100);
  moisturePertange_A1 = map(moistureA1,map_low,map_high,0,100);

  Serial.println(moisturePertange_A0);
  Serial.println(moisturePertange_A1);
}

void SendMoistureSerial()
{
  const size_t capacity = JSON_ARRAY_SIZE(3) + 3*JSON_OBJECT_SIZE(2)+51;
  DynamicJsonBuffer jsonBuffer(capacity);
  
  JsonArray& root = jsonBuffer.createArray();
  
  JsonObject& root_0 = root.createNestedObject();
  root_0["v"] = moisturePertange_A0;
  
  JsonObject& root_1 = root.createNestedObject();
  root_1["v"] = moisturePertange_A1;
    
  root.printTo(Serial);
  root.printTo(s);  
}
