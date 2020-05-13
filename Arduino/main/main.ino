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
  #include <asyncHTTPrequest.h>
  #include <Ticker.h>
  
  int D5 = 14, D6 = 12;
  SoftwareSerial s(D6,D5);
  
  WiFiUDP ntpUDP;
  NTPClient timeClient(ntpUDP);
  // For UTC +3.00 : 3 * 60 * 60 : 10800
  const long utcOffsetInSeconds = 10800;
  
  //PIN 
  #define TEMPERATURE_SENSOR_PIN 2
  #define MOISTURE_SENSOR_PIN A0
  
  #define RELAYS_PUMP_PIN 5
  #define RELAYS_WATER_SWITCH_P0_PIN 4
  #define RELAYS_WATER_SWITCH_P1_PIN 15
  
  #define TEMPERATURE_INTERVAL_TIME_POST 60000
  #define MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON 120000
  #define MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF 120000
  
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
    bool automationMode;
  };
  const char* FCMToken;
  
  
  const String  api = "http://192.168.1.6:45457/api";
  const String fingerPrint = "82:88:7C:B6:41:71:8B:04:67:A5:10:C2:34:40:24:04:78:A6:7E:55"; 
  
  const String series = "BBBB";
  bool registered; //if user registered system from app
  Board board;
  
  HTTPClient http;
  //WiFiClient cli;

  asyncHTTPrequest request;
  Ticker ticker;
  String dataToParseFromAPI;
  
  
  SystemState systemState;
  
  float temperature;
  unsigned long temperatureTimeTrigger;
  
  Moisture moisture[2];
  unsigned long moistureTimeTrigger;
  
  Schedule schedule;
  
  //unsigned long currentTime = millis();
  
  Zone zones[2];
  
  bool notificationSent[2];
  bool notificationMoistureMaxSent[2];
  
  SystemState localSystemState;
  SystemState SystemStateFromDb;
  
   WifiConnect wifi = WifiConnect("DIGI_ce10a8","989d31ef");
  
   //WifiConnect wifi = WifiConnect("MERCUSYS_98EB","matei123");
  
  bool manualIrrigation;

  void sendRequest() {
    if (request.readyState() == 0 || request.readyState() == 4) {
      request.open("GET", "http://192.168.1.6:45457/api/systems/1013/arduino");
      request.send();
    }
  }


  void requestCB(void* optParm, asyncHTTPrequest* request, int readyState) {
    if (readyState == 4) {
//        Serial.println("intrerupere");       
//       Serial.println(request->responseText());       
       Serial.println(request->responseText());
        Serial.println();
        request->setDebug(false);
    }
  }

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

    notificationMoistureMaxSent[0] = false;
    notificationMoistureMaxSent[1] = false;
  
    localSystemState.working = false;
    request.setDebug(true);
    request.onReadyStateChange(requestCB);
    ticker.attach(2, sendRequest);
  }
  
  
  void loop() {

    moisture[0].port = "A0";
    moisture[0].value = 50;

    moisture[1].port = "A1";
    moisture[1].value = 40;  
  // ReadMoisture();  
  
   if (WiFi.status() == WL_CONNECTED) //&& board.registered)  //Check WiFi connection status
   {
    GetScheduleZonesState(dataToParseFromAPI);
    timeClient.update();
   
    int currentTimeFromServer = ((timeClient.getHours() * 3600) + (timeClient.getMinutes()* 60));
    
    int currentTime = millis();
    
  
    Serial.print("System state working: ");
    Serial.println(SystemStateFromDb.working);
    Serial.print("System state manual: ");
    Serial.println(SystemStateFromDb.manual);
    Serial.print("System state local : ");
    Serial.println(localSystemState.working);
    
    
    //temperature = ReadTemperature();
    temperature = 20;
    bool temperatureNotification;
    
     //automation code
     int startTime = TransformTimeInSeconds(schedule.beginTime);
     int stopTime = TransformTimeInSeconds(schedule.endTime);
     
    if((currentTimeFromServer >= startTime) && (currentTimeFromServer <= stopTime) && localSystemState.working == false && SystemStateFromDb.automationMode == true)
    {
      if(temperature >= schedule.temperatureMin && temperature <= schedule.temperatureMax)
      {
        Serial.println("data");
        Serial.println("primul temp");
        int countSwitchOn = 0;
        if(moisture[0].value <= zones[0].startMoisture || moisture[0].value <= zones[0].stopMoisture )
        {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,!zones[0].waterSwitch);
          if(zones[0].waterSwitch == true)
            countSwitchOn++;
        }
        if(moisture[1].value <= zones[1].startMoisture || moisture[1].value <= zones[1].stopMoisture )
        {
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,!zones[1].waterSwitch);
          if(zones[1].waterSwitch == true)
            countSwitchOn++;
        }
        if(countSwitchOn > 0)
        {
          digitalWrite(RELAYS_PUMP_PIN,LOW);
          localSystemState.working = true;
          notificationSent[0] = false;
          notificationSent[1] = false;
         // SendNotification(FCMToken, "Irigarea automata a inceput");
          temperatureNotification = false;
          UpdateWorking(false,true,SystemStateFromDb.automationMode,board.id);
        }
      }
      else if(temperature < schedule.temperatureMin || temperature > schedule.temperatureMax && localSystemState.working == true && SystemStateFromDb.manual == false && SystemStateFromDb.automationMode == true)
      {
        Serial.println("intra temperatura");
        digitalWrite(RELAYS_PUMP_PIN,HIGH);
        digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,HIGH);
        digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,HIGH); 
        localSystemState.working = false;
        temperatureNotification = true;
        if(temperatureNotification == false)
        {
          //SendNotification(FCMToken, "Irigarea automata s-a oprit din cauza temperaturii nonconforme");
        }
        UpdateWorking(false,false,SystemStateFromDb.automationMode,board.id);
      }
    } 
    else if(currentTimeFromServer > stopTime || currentTimeFromServer < startTime && localSystemState.working == true && SystemStateFromDb.manual == false && SystemStateFromDb.automationMode == true)
    {
      Serial.println("intra timp");
      digitalWrite(RELAYS_PUMP_PIN,HIGH);
      digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,HIGH);
      digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,HIGH); 
      localSystemState.working = false;
    //  SendNotification(FCMToken, "Irigarea automata s-a incheiat din cauza programului.");
      UpdateWorking(false,false,SystemStateFromDb.automationMode,board.id);
    } 
    else if(SystemStateFromDb.working == false && SystemStateFromDb.manual == false && manualIrrigation == false && localSystemState.working == true)
   {
    Serial.println("intra ultimul");
      digitalWrite(RELAYS_PUMP_PIN,HIGH);
      digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,HIGH);
      digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,HIGH); 
      manualIrrigation = false;
      localSystemState.working = false;
    //  SendNotification(FCMToken, "Irigarea a fost oprita");
   }
  
    //verificare daca s-a atins umiditatea si se inchid switcj urile
    if(localSystemState.working == true && systemState.manual == false && SystemStateFromDb.automationMode == true)
    {
      int countSwitchOff = 0;
      if(moisture[0].value > zones[0].stopMoisture || zones[0].waterSwitch == false)
       {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,HIGH);
          countSwitchOff++;
        }
        if(moisture[1].value > zones[1].stopMoisture || zones[1].waterSwitch == false)
        {
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,HIGH);
          countSwitchOff++;
        }
        if(countSwitchOff == 2)
        {
          digitalWrite(RELAYS_PUMP_PIN,HIGH);
          localSystemState.working = false;
        //  SendNotification(FCMToken, "Irigarea s-a incheiat. Umiditatea este in parametrii alesi.");
          UpdateWorking(false,false,SystemStateFromDb.automationMode,board.id);
        }
    }
   
  // end automation code
  
   
  
    
    if(SystemStateFromDb.working == true && SystemStateFromDb.manual == true && localSystemState.working == false )
     {
      Serial.println("intra in");
        digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,!zones[0].waterSwitch);
        digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,!zones[1].waterSwitch); 
        digitalWrite(RELAYS_PUMP_PIN,LOW);
        manualIrrigation = true;
        localSystemState.working = true;
        notificationSent[0] = false;
        notificationSent[1] = false;
       // SendNotification(FCMToken, "Irigarea manuala a inceput");
     }
     else if(SystemStateFromDb.working == false && manualIrrigation == true && localSystemState.working == true)
     {
        digitalWrite(RELAYS_PUMP_PIN,HIGH);
        digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,HIGH);
        digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,HIGH); 
        manualIrrigation = false;
        notificationMoistureMaxSent[0] = false;
        notificationMoistureMaxSent[1] = false;
       // SendNotification(FCMToken, "Irigarea s-a incheiat");
     }
     else if(SystemStateFromDb.working == true && manualIrrigation == true && localSystemState.working == true && SystemStateFromDb.automationMode == false)
     {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,!zones[0].waterSwitch);
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,!zones[1].waterSwitch);        

          if(zones[0].waterSwitch == false && zones[1].waterSwitch == false)
          {
            digitalWrite(RELAYS_PUMP_PIN,HIGH);
            localSystemState.working = false;
           // SendNotification(FCMToken, "Irigarea s-a incheiat. Niciun robinet deschis.");
            UpdateWorking(false,false,SystemStateFromDb.automationMode,board.id);  
            delay(1000);
          }
    }
    
     //end manual irrigation
      Serial.print("Time from server:");
      Serial.println(currentTimeFromServer);
      Serial.print("Time from app start:");
      Serial.println(startTime);
      Serial.print("Time from app end:");
      Serial.println(stopTime); 

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

    //Send notidication
      
      if(moisture[0].value < zones[0].startMoisture && localSystemState.working == false && notificationSent[0] == false)
      {
        notificationSent[0]=true;
        String text0 = "A scazut umiditatea in zona: ";
        String name0 = String(zones[0].name);
        String notifText0 = text0 + name0;
        Serial.print("Zone name :");
        Serial.println(notifText0);
        //SendNotification(FCMToken,notifText0);
      }     
    
       if(moisture[1].value < zones[1].startMoisture && localSystemState.working == false && notificationSent[1]==false)
        {
          notificationSent[1]=true;
          String text1 = "A scazut umiditatea in zona: ";
          String name1 = String(zones[1].name);
          String notifText1 = text1 + name1;
          Serial.print("Zone name:");
          Serial.println(notifText1);
          //SendNotification(FCMToken,notifText1);
        }

        if(moisture[0].value > zones[0].stopMoisture && localSystemState.working == true && SystemStateFromDb.working == true && SystemStateFromDb.manual == true  && notificationMoistureMaxSent[0] == false)
      {
        notificationMoistureMaxSent[0]=true;
        String text0 = "S-a atins umiditatea dorita in zona: ";
        String name0 = String(zones[0].name);
        String notifText0 = text0 + name0;
        Serial.print("Zone name :");
        Serial.println(notifText0);
        //SendNotification(FCMToken,notifText0);
      }
    
       if(moisture[1].value > zones[1].stopMoisture && localSystemState.working == true && SystemStateFromDb.working == true && SystemStateFromDb.manual == true  && notificationMoistureMaxSent[1] == false)
        {
          notificationMoistureMaxSent[1]=true;
          String text1 = "S-a atins umiditatea dorita in zona: ";
          String name1 = String(zones[1].name);
          String notifText1 = text1 + name1;
          Serial.print("Zone name:");
          Serial.println(notifText1);
          //SendNotification(FCMToken,notifText1);
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
      
      http.begin(api+"/systems/"+systemId+"/measurements/"+port);      //Specify request destination
        http.addHeader("Content-Type", "application/json");  //Specify content-type header
     
        int httpCode = http.POST(JSONmessageBuffer);   //Send the request
        String payload = http.getString();                                        //Get the response payload
     
        Serial.println(httpCode);   //Print HTTP return code
        Serial.println(payload);    //Print request response payload
    
        http.end();  //Close connection
        
    }
    
    void ReadMoisture() {
      Serial.print("Intra in Read moisture");
      const size_t capacity = JSON_ARRAY_SIZE(3) + 3*JSON_OBJECT_SIZE(2) + 215;
      DynamicJsonBuffer jsonBuffer(capacity);
      
      JsonArray& root = jsonBuffer.parseArray(s);
       if (root.success()){
        moisture[0].port = "A0";
        moisture[0].value = root[0]["v"];
        
        moisture[1].port = "A1";
        moisture[1].value = root[1]["v"]; 
       Serial.println("JSON received and parsed");
       Serial.print("A0:");
       Serial.println(moisture[0].value);
       Serial.print("A1:");
       Serial.println(moisture[1].value);
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
      http.begin(api+"/boardsseries/"+series); //Specify the URL
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
    
      void GetScheduleZonesState(String dataToParse)
      {
            if(dataToParse) {
            String payload = dataToParse;
            Serial.println("payload");
           // Serial.println(payload);
            const size_t capacity = JSON_ARRAY_SIZE(1) + JSON_ARRAY_SIZE(2) + JSON_OBJECT_SIZE(2) + 2*JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(8) + 824;
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
            SystemStateFromDb.automationMode = schState["automationMode"];
            
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
            Serial.println("Error on parsing");
          }
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
        Serial.println("making POST request for sending notification");
        String contentType = "application/json";
        
        const int capacity = JSON_OBJECT_SIZE(2)+212;
        StaticJsonBuffer<capacity> JSONbuffer;
        JsonObject& JSONencoder = JSONbuffer.createObject();
        
        JSONencoder["body"] = message;
        JSONencoder["to"] = token;
        
        char JSONmessageBuffer[capacity];
        JSONencoder.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
          
        http.begin(api+"/push-notification");      //Specify request destination
        http.addHeader("Content-Type", "application/json");  //Specify content-type header
      
        int httpCode = http.POST(JSONmessageBuffer);   //Send the request
        String payload = http.getString();                                        //Get the response payload
      
        Serial.println(httpCode);   //Print HTTP return code
        Serial.println(payload);    //Print request response payload
      
        http.end();  //Close connection
      }
    
      void UpdateWorking(bool manual, bool working,bool automation,int systemId)
      {
        String contentType = "application/json";
        
        const int capacity = JSON_OBJECT_SIZE(3)+88;
        StaticJsonBuffer<capacity> JSONbuffer;
        JsonObject& JSONencoder = JSONbuffer.createObject();
        
        JSONencoder["working"] = working;
        JSONencoder["manual"] = manual;
        JSONencoder["automationMode"] = automation;
        
        char JSONmessageBuffer[capacity];
        JSONencoder.prettyPrintTo(JSONmessageBuffer, sizeof(JSONmessageBuffer));
          
        http.begin(api+"/systems/"+systemId+"/systemState");      //Specify request destination
        http.addHeader("Content-Type", "application/json");  //Specify content-type header
      
        int httpCode = http.PUT(JSONmessageBuffer);   //Send the request
        String payload = http.getString();                                        //Get the response payload
      
        Serial.println(httpCode);   //Print HTTP return code
        Serial.println(payload);    //Print request response payload
      
        http.end();  //Close connection
      }
