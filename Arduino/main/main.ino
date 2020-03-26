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
  #define RELAYS_WATER_SWITCH_P1_PIN 15
  
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
    const char* name;
  };
  
  struct Schedule
  {
    int temperatureMin;
    int temperatureMax;
    const char* beginTime;
    const char* endTime;
  };
  
  struct SystemState
  {
    bool manual;
    bool working;
  };
  const char* FCMToken;
  
  
  const String  api = "https://smart-garden.conveyor.cloud/api";
  const String fingerPrint = "82:88:7C:B6:41:71:8B:04:67:A5:10:C2:34:40:24:04:78:A6:7E:55"; 
  
  const String series = "BBBB";
  bool registered; //if user registered system from app
  Board board;
  
  HTTPClient http;
  //WiFiClient cli;
  
  SystemState systemState;
  
  float temperature;
  unsigned long temperatureTimeTrigger;
  
  Moisture moisture[2];
  unsigned long moistureTimeTrigger;
  
  Schedule schedule;
  
  //unsigned long currentTime = millis();
  
  Zone zones[2];
  
  bool notificationSent[2];
  
  SystemState localSystemState;
  SystemState SystemStateFromDb;
  
   //WifiConnect wifi = WifiConnect("DIGI_ce10a8","989d31ef");
  
   WifiConnect wifi = WifiConnect("MERCUSYS_98EB","matei123");
  
  bool manualIrrigation;
  
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
    
    //registered = CheckForRegisteredBoard(series);
    board = GetBoardByBoardSeries(series);
    Serial.print("Id:");
    Serial.println(board.id);
    Serial.print("Registered:");
    Serial.println(board.registered);
  
    notificationSent[0] = false;
    notificationSent[1] = false;
  
    localSystemState.working = false;
    
  }
  
  
  void loop() {
    
   //ReadMoisture();
  
      moisture[0].port = "A0";
      moisture[0].value = 60;
      
      moisture[1].port = "A1";
      moisture[1].value = 70; 
   
  
   if (WiFi.status() == WL_CONNECTED) //&& board.registered)  //Check WiFi connection status
   {
    GetScheduleZonesState(board.id);
    timeClient.update();
   
    int currentTimeFromServer = ((timeClient.getHours() * 3600) + (timeClient.getMinutes()* 60));
    
    int currentTime = millis();
    
  
    Serial.print("System state working: ");
    Serial.println(SystemStateFromDb.working);
    Serial.print("System state manual: ");
    Serial.println(SystemStateFromDb.manual);
    Serial.print("System state local : ");
    Serial.println(localSystemState.working);
    
    
    temperature = ReadTemperature();
    
    
     //automation code
  
    if((currentTimeFromServer >= TransformTimeInSeconds(schedule.beginTime)) && (currentTimeFromServer <= TransformTimeInSeconds(schedule.endTime)) && localSystemState.working == false )
    {
      if(temperature >= schedule.temperatureMin && temperature <= schedule.temperatureMax)
      {
        int countSwitchOn = 0;
        if(moisture[0].value <= zones[0].startMoisture || moisture[0].value <= zones[0].stopMoisture )
        {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,zones[0].waterSwitch);
          if(zones[0].waterSwitch == true)
            countSwitchOn++;
        }
        if(moisture[1].value <= zones[1].startMoisture || moisture[1].value <= zones[1].stopMoisture )
        {
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,true);
          if(zones[1].waterSwitch == true)
            countSwitchOn++;
        }
        if(countSwitchOn > 0)
        {
          digitalWrite(RELAYS_PUMP_PIN,HIGH);
          localSystemState.working = true;
          notificationSent[0] = false;
          notificationSent[1] = false;
          SendNotification(FCMToken, "Irigarea automata a inceput");
          UpdateWorking(false,true,board.id);
        }
      }
      else if(temperature < schedule.temperatureMin || temperature > schedule.temperatureMax && localSystemState.working == true)
      {
        digitalWrite(RELAYS_PUMP_PIN,LOW);
        digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
        digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,LOW); 
        localSystemState.working = false;
        SendNotification(FCMToken, "Irigarea s-a oprit din cauza temperaturii nonconforme");
        UpdateWorking(false,false,board.id);
      }
    } else if(currentTimeFromServer > TransformTimeInSeconds(schedule.endTime) || currentTimeFromServer < TransformTimeInSeconds(schedule.beginTime) && localSystemState.working == true && SystemStateFromDb.manual == false)
    {
      digitalWrite(RELAYS_PUMP_PIN,LOW);
      digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
      digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,LOW); 
      localSystemState.working = false;
      SendNotification(FCMToken, "Irigarea a luat sfarsit.");
      UpdateWorking(false,false,board.id);
    } 
    else if(SystemStateFromDb.working == false && SystemStateFromDb.working == false && manualIrrigation == false && localSystemState.working == true)
   {
      digitalWrite(RELAYS_PUMP_PIN,LOW);
      digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
      digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,LOW); 
      manualIrrigation = false;
      localSystemState.working = false;
      SendNotification(FCMToken, "Irigarea automata a fost oprita manual");
   }
  
    //verificare daca s-a atins umiditatea si se inchid switcj urile
    if(localSystemState.working == true && systemState.manual == false)
    {
      int countSwitchOff = 0;
      if(moisture[0].value > zones[0].stopMoisture || zones[0].waterSwitch == false)
       {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
          countSwitchOff++;
        }
        if(moisture[1].value > zones[1].stopMoisture || zones[1].waterSwitch == false)
        {
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,false);
          countSwitchOff++;
        }
        if(countSwitchOff == 2)
        {
          digitalWrite(RELAYS_PUMP_PIN,LOW);
          localSystemState.working = false;
          SendNotification(FCMToken, "Irigarea a luat sfarsit");
          UpdateWorking(false,false,board.id);
        }
    }
  // end automation code
  
    //send temperature to db
  
   
    if((currentTime - temperatureTimeTrigger >= TEMPERATURE_INTERVAL_TIME_POST))
    {
         PostSensorValue(board.id,"Temperature","D0",temperature);
         temperatureTimeTrigger = millis();
    }
  
    //check for moisture and send it to db
    
    if(SystemStateFromDb.working == true)
     {
       for(int i=0;i<2;i++) { 
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
     else if(SystemStateFromDb.working == false)
     {
      for(int i=0;i<2;i++) {
        if((currentTime - moistureTimeTrigger >= MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF))
        {
          PostSensorValue(board.id,"Moisture",moisture[i].port,moisture[i].value);
          if(i==1){
          moistureTimeTrigger = millis();
          }
        }
      }
    }
  
    
    if(SystemStateFromDb.working == true && SystemStateFromDb.manual == true && localSystemState.working == false )
     {
        digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,zones[0].waterSwitch);
        digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,zones[1].waterSwitch); 
        digitalWrite(RELAYS_PUMP_PIN,HIGH);
        manualIrrigation = true;
        localSystemState.working = true;
        notificationSent[0] = false;
        notificationSent[1] = false;
        SendNotification(FCMToken, "Irigarea manuala a inceput");
     }
     else if(SystemStateFromDb.working == false && manualIrrigation == true && localSystemState.working == true)
     {
        digitalWrite(RELAYS_PUMP_PIN,LOW);
        digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
        digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,LOW); 
        manualIrrigation = false;
        SendNotification(FCMToken, "Irigarea manuala a luat sfarsit");
     }
      Serial.print("Time from server:");
      Serial.println(currentTimeFromServer);
      Serial.print("Time from app start:");
      Serial.println(TransformTimeInSeconds(schedule.beginTime));
      Serial.print("Time from app end:");
      Serial.println(schedule.endTime); 
      
      if(moisture[0].value < zones[0].startMoisture && localSystemState.working == false && notificationSent[0] == false)
      {
        notificationSent[0]=true;
        String text0 = "A scazut umiditatea in zona: ";
        String name0 = String(zones[0].name);
        String notifText0 = text0 + name0;
        Serial.print("Zone name t:");
        Serial.println(name0);
        SendNotification(FCMToken,notifText0);
      }
    
       if(moisture[1].value < zones[1].startMoisture && localSystemState.working == false && notificationSent[1]==false)
        {
          notificationSent[1]=true;
          String text1 = "A scazut umiditatea in zona: ";
          String name1 = String(zones[1].name);
          String notifText1 = text1 + name1;
          Serial.print("Zone name t:");
          Serial.println(notifText1);
          SendNotification(FCMToken,notifText1);
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
      Serial.print("Intra in Read moisture pe intrerupere");
      const size_t capacity = JSON_ARRAY_SIZE(3) + 3*JSON_OBJECT_SIZE(2) + 215;
      DynamicJsonBuffer jsonBuffer(capacity);
      
      JsonArray& root = jsonBuffer.parseArray(s);
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
    
      return sensors.getTempCByIndex(0);
    }
    
    Board GetBoardByBoardSeries(String series)
    {
      Board board;
      http.begin(api+"/boardsseries/"+series,fingerPrint); //Specify the URL
      int httpCode = http.GET();             
        if (httpCode > 0) { //Check for the returning code
     
          String payload = http.getString();
    
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
    
      void GetScheduleZonesState(int systemId)
      {
        http.begin(api+"/systems/"+systemId+"/arduino",fingerPrint); //Specify the URL
        int httpCode = http.GET();
            Serial.print("code:");    
            Serial.println(httpCode);     
          if (httpCode == 200) { //Check for the returning code
           
            String payload = http.getString();
            Serial.println("payload");
            Serial.println(payload);
            const size_t capacity = JSON_ARRAY_SIZE(1) + JSON_ARRAY_SIZE(2) + JSON_OBJECT_SIZE(2) + 2*JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(8)+816;
            DynamicJsonBuffer jsonBuffer(capacity);
            JsonObject& root = jsonBuffer.parseObject(payload);
    
            if(root.success()){
           
            JsonObject& schState = root["dataForArduino"][0];
           
            schedule.beginTime = schState["start"]; // "2019-12-02T21:51:00"
            schedule.endTime = schState["stop"]; // "2019-12-02T21:51:00"
            schedule.temperatureMin = (int)schState["temperatureMin"]; // 22
            schedule.temperatureMax = (int)schState["temperatureMax"]; // 32
            SystemStateFromDb.working = schState["working"];
            SystemStateFromDb.manual = schState["manual"];
            FCMToken = schState["fcmToken"];
    
            
            JsonObject& zonesMapped_0 = root["zonesMapped"][0];
            zones[0].startMoisture = zonesMapped_0["moistureStart"]; 
            zones[0].stopMoisture = zonesMapped_0["moistureStop"]; 
            zones[0].waterSwitch = zonesMapped_0["waterSwitch"]; 
            zones[0].name = zonesMapped_0["name"]; 
            JsonObject& zonesMapped_1 = root["zonesMapped"][1];
            zones[1].startMoisture = zonesMapped_1["moistureStart"]; 
            zones[1].stopMoisture = zonesMapped_1["moistureStop"];
            zones[1].waterSwitch = zonesMapped_1["waterSwitch"];
            zones[1].name = zonesMapped_1["name"]; 
           }
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
    
    
      void SendNotification(String token, String message)
      {
        //Serial.println("making POST request for sending notification");
        String contentType = "application/json";
        
        const int capacity = JSON_OBJECT_SIZE(2)+212;
        StaticJsonBuffer<capacity> JSONbuffer;
        JsonObject& JSONencoder = JSONbuffer.createObject();
        
        JSONencoder["body"] = message;
        JSONencoder["to"] = token;
        
        char JSONmessageBuffer[capacity];
        JSONencoder.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
          
        http.begin(api+"/push-notification",fingerPrint);      //Specify request destination
        http.addHeader("Content-Type", "application/json");  //Specify content-type header
      
        int httpCode = http.POST(JSONmessageBuffer);   //Send the request
        String payload = http.getString();                                        //Get the response payload
      
        Serial.println(httpCode);   //Print HTTP return code
        Serial.println(payload);    //Print request response payload
      
        http.end();  //Close connection
      }
    
      void UpdateWorking(bool manual, bool working,int systemId)
      {
        String contentType = "application/json";
        
        const int capacity = JSON_OBJECT_SIZE(2)+65;
        StaticJsonBuffer<capacity> JSONbuffer;
        JsonObject& JSONencoder = JSONbuffer.createObject();
        
        JSONencoder["working"] = working;
        JSONencoder["manual"] = manual;
        
        char JSONmessageBuffer[capacity];
        JSONencoder.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
          
        http.begin(api+"/systems/"+systemId+"/systemState",fingerPrint);      //Specify request destination
        http.addHeader("Content-Type", "application/json");  //Specify content-type header
      
        int httpCode = http.PUT(JSONmessageBuffer);   //Send the request
        String payload = http.getString();                                        //Get the response payload
      
        Serial.println(httpCode);   //Print HTTP return code
        Serial.println(payload);    //Print request response payload
      
        http.end();  //Close connection
      }
