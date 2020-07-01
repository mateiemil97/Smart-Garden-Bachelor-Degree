  #include <ArduinoJson.h>
  #include <ESP8266WiFi.h>
  #include <DNSServer.h>
  #include <WiFiManager.h>
  #include <ESP8266WebServer.h>
  #include <ESP8266HTTPClient.h>
  #include <string.h>
  #include <Adafruit_Sensor.h>
  #include <DHT.h>
  #include <SoftwareSerial.h>
  #include <NTPClient.h>
  #include <WiFiUdp.h>
  #include <asyncHTTPrequest.h>
  #include <Ticker.h>
  #include <FirebaseArduino.h>
  #include <LiquidCrystal_I2C.h>

  void ICACHE_RAM_ATTR TurnOnOffIrrigation();

  
  int D5 = 14, D6 = 12;
  SoftwareSerial s(D6,D5);
  
  WiFiUDP ntpUDP;
  NTPClient timeClient(ntpUDP);
  // For UTC +3.00 : 3 * 60 * 60 : 10800
  const long utcOffsetInSeconds = 10800;
  
  //PIN 
  #define TEMPHUM_SENSOR_PIN 2     // Digital pin connected to the DHT sensor
  #define DHTTYPE    DHT11  
  
  #define RELAYS_PUMP_PIN 0
  #define RELAYS_WATER_SWITCH_P0_PIN 13
  #define RELAYS_WATER_SWITCH_P1_PIN 16

  #define BUTTON_IRRIGATION_SWITCH_PIN 15
  
  #define TEMPHUM_INTERVAL_TIME_POST 600000
  #define MOISTURE_INTERVAL_TIME_POST_SYSTEM_ON 120000
  #define MOISTURE_INTERVAL_TIME_POST_SYSTEM_OFF 120000
  #define FIREBASE_INTERVAL_TIME_POST_SYSTEM 1000
  #define DISPLAY_INTERVAL_TIME_CHANGE_PAGE 5000
  
  #define FIREBASE_HOST "smart-drop-2eda9.firebaseio.com"
  #define FIREBASE_AUTH "g0J8wyFbZC1zVgxLR5z5jBHEcLpypnuyPIhR0xXp"
  
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
  
  
  const String  api = "http://192.168.1.7:80/api";
  const String apiGetData = "http://192.168.1.7:80/api/systems/1013/arduino";
  const String series = "BBBB";
  bool registered; //if user registered system from app
  Board board;

  bool intreruptionModeOn;

  LiquidCrystal_I2C lcd(0x27, 16, 2);
  
  int page_counter;
  long previousDisplayMillis;
  HTTPClient http;
  //WiFiClient cli;

  asyncHTTPrequest request;
  Ticker ticker;
  String dataToParseFromAPI;
  
  
  SystemState systemState;

  DHT dht(TEMPHUM_SENSOR_PIN, DHTTYPE);
  float humidity;
  float temperature;
  unsigned long temperatureTimeTrigger;
  
  Moisture moisture[2];
  unsigned long moistureTimeTrigger;
  
  Schedule schedule;

  
  
  unsigned long currentTime = millis();

  int irrigationState = LOW;
  int stateButton;
  int previousIrrigationState = LOW;
  
  Zone zones[2];
  
  bool notificationSent[2];
  bool notificationMoistureMaxSent[2];
  
  SystemState localSystemState;
  SystemState SystemStateFromDb;
    
  bool manualIrrigation;
  bool a;
  void sendRequest() {
    if (request.readyState() == 0 || request.readyState() == 4) {
      request.open("GET", "http://192.168.1.7:80/api/systems/1013/arduino");
      request.send();
    }
  }


  void requestCB(void* optParm, asyncHTTPrequest* request, int readyState) {
    if (readyState == 4) {
       dataToParseFromAPI = request->responseText();
      //Serial.println(request->responseText());       
    }
  }

  String daysOfWeek[7] = {"Duminica","Luni","Marti","Miercuri","Joi","Vineri","Sambata"};
  
  void setup() {
    lcd.begin(16,2);
    lcd.init();
    lcd.backlight();
    page_counter = 1;
    dht.begin();
    WiFiManager wifiManager;
    Serial.println("Conecting.....");
    wifiManager.autoConnect("Smart Drop");
    Serial.println("connected");
    Serial.begin(9600);
    //serial with uno
    s.begin(9600);

    pinMode(BUTTON_IRRIGATION_SWITCH_PIN, INPUT); 
   
    Firebase.begin(FIREBASE_HOST, FIREBASE_AUTH);
//    //Enable auto reconnect the WiFi when connection lost

    
    timeClient.begin();
    timeClient.setTimeOffset(utcOffsetInSeconds);
  

    pinMode(TEMPHUM_SENSOR_PIN ,INPUT);
    pinMode(RELAYS_PUMP_PIN,OUTPUT);
    pinMode(RELAYS_WATER_SWITCH_P0_PIN,OUTPUT);
    pinMode(RELAYS_WATER_SWITCH_P1_PIN,OUTPUT);
  
    temperatureTimeTrigger = 0;
    moistureTimeTrigger = millis();
    long firebaseTimeTrigger = millis();

        
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
    request.onReadyStateChange(requestCB);
    ticker.attach(2, sendRequest);

    previousDisplayMillis = millis();
  }
  
  
  void loop() {

//    moisture[0].port = "A0";
//    moisture[0].value = 50;
//
//    moisture[1].port = "A1";
//    moisture[1].value = 40;  

   if(board.registered != 1)
   {
      board = GetBoardByBoardSeries(series);
   }


   //ReadMoisture();  
   temperature = ReadTemperature();
   humidity = ReadHumidity();
   if (WiFi.status() == WL_CONNECTED && board.registered == 1)  //Check WiFi connection status
   {
    GetScheduleZonesState(dataToParseFromAPI);
    timeClient.update();
    unsigned int currentTimeFromServer = ((timeClient.getHours() * 3600) + (timeClient.getMinutes()* 60));
    unsigned int currentTimeFromServerForFirebase  = ((timeClient.getHours() * 3600) + (timeClient.getMinutes()* 60) + timeClient.getSeconds());
    String currentTimeFromServerForFirebaseDateAndTime = String( daysOfWeek[timeClient.getDay()] +" " + timeClient.getHours()) + ":" + String(timeClient.getMinutes()) + ":" + String(timeClient.getSeconds());
    currentTime = millis();

      
  
    Serial.print("System state working: ");
    Serial.println(SystemStateFromDb.working);
    Serial.print("System state manual: ");
    Serial.println(SystemStateFromDb.manual);
    Serial.print("System state local : ");
    Serial.println(localSystemState.working);
    
  
    Serial.print("Temperature");
    //temperature = 20;
    bool temperatureNotification;
    
     //automation code
     int startTime = TransformTimeInSeconds(schedule.beginTime);
     int stopTime = TransformTimeInSeconds(schedule.endTime);
     
    if((currentTimeFromServer >= startTime) && (currentTimeFromServer <= stopTime) && localSystemState.working == false  && SystemStateFromDb.automationMode == true)
    {
      if(temperature >= schedule.temperatureMin && temperature <= schedule.temperatureMax)
      {
        a = true;
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
          UpdateWorking(false,true,SystemStateFromDb.automationMode,board.id);
          digitalWrite(RELAYS_PUMP_PIN,LOW);
          localSystemState.working = true;
          notificationSent[0] = false;
          notificationSent[1] = false;
         // SendNotification(FCMToken, "Irigarea automata a inceput");
          temperatureNotification = false;
          SystemStateFromDb.working = true;
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
    else if(SystemStateFromDb.automationMode == false && localSystemState.working == true && SystemStateFromDb.manual == false)
   {
    Serial.println("intra ultimul");
    Serial.println(SystemStateFromDb.working);
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

    if(localSystemState.working == true) {
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
  
   
    if((currentTime - temperatureTimeTrigger >= TEMPHUM_INTERVAL_TIME_POST))
    {
         PostSensorValue(board.id,"Temperature","D0",temperature);
         PostSensorValue(board.id,"Humidity","D1",humidity);
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
      
//      if(moisture[0].value < zones[0].startMoisture && localSystemState.working == false && notificationSent[0] == false)
//      {
//        notificationSent[0]=true;
//        String text0 = "A scazut umiditatea in zona: ";
//        String name0 = String(zones[0].name);
//        String notifText0 = text0 + name0;
//        Serial.print("Zone name :");
//        Serial.println(notifText0);
//        // SendNotification(FCMToken,notifText0);
//      }     
//    
//       if(moisture[1].value < zones[1].startMoisture && localSystemState.working == false && notificationSent[1]==false)
//        {
//          notificationSent[1]=true;
//          String text1 = "A scazut umiditatea in zona: ";
//          String name1 = String(zones[1].name);
//          String notifText1 = text1 + name1;
//          Serial.print("Zone name:");
//          Serial.println(notifText1);
//        //  SendNotification(FCMToken,notifText1);
//        }
//
//        if(moisture[0].value > zones[0].stopMoisture && localSystemState.working == true && SystemStateFromDb.working == true && SystemStateFromDb.manual == true  && notificationMoistureMaxSent[0] == false)
//      {
//        notificationMoistureMaxSent[0]=true;
//        String text0 = "S-a atins umiditatea dorita in zona: ";
//        String name0 = String(zones[0].name);
//        String notifText0 = text0 + name0;
//        Serial.print("Zone name :");
//        Serial.println(notifText0);
//       // SendNotification(FCMToken,notifText0);
//      }
//    
//       if(moisture[1].value > zones[1].stopMoisture && localSystemState.working == true && SystemStateFromDb.working == true && SystemStateFromDb.manual == true  && notificationMoistureMaxSent[1] == false)
//        {
//          notificationMoistureMaxSent[1]=true;
//          String text1 = "S-a atins umiditatea dorita in zona: ";
//          String name1 = String(zones[1].name);
//          String notifText1 = text1 + name1;
//          Serial.print("Zone name:");
//          Serial.println(notifText1);
//       //   SendNotification(FCMToken,notifText1);
//      }
          
          unsigned int firebaseTimeTrigger;
          if((currentTime - firebaseTimeTrigger >= FIREBASE_INTERVAL_TIME_POST_SYSTEM))
          {
            String pathForFirebase = "System_Series/" +series+ "/Last_Time_Seen_Online/Miliseconds";
            Firebase.setInt(pathForFirebase,currentTimeFromServerForFirebase);
            String pathForFirebaseDateTime = "System_Series/" +series+ "/Last_Time_Seen_Online/DateAndTime";
            Firebase.setString(pathForFirebaseDateTime,currentTimeFromServerForFirebaseDateAndTime);

            firebaseTimeTrigger = millis();
          }
   }
   else {
      Serial.println("System not connected or not registered");
    }

    if (currentTime - previousDisplayMillis > DISPLAY_INTERVAL_TIME_CHANGE_PAGE) {  //If interval is reached, return to home page 1
       previousDisplayMillis = millis();
       lcd.clear();
       DisplayInformationsOnPages();
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
      float newT = dht.readTemperature();
      float currentTemp;
      if (isnan(newT)) {
        Serial.println("Failed to read from DHT sensor!");
        currentTemp = temperature;
      }
      else {
        currentTemp = newT;
        Serial.print("Temperatura:");
        Serial.println(currentTemp );
      }
      return currentTemp;
    }

    float ReadHumidity()
    {
      float newH = dht.readHumidity();
      float currentHum;
      if (isnan(newH)) {
        Serial.println("Failed to read from DHT sensor!");
        currentHum = humidity;
      }
      else {
        currentHum = newH;
        Serial.print("Humidity:");
        Serial.println(currentHum);
      }
      return currentHum;
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

      String GetDateFromDateTime(String date)
      {
        int splitT = date.indexOf("T");
        String  dateFromDate = date.substring(0, splitT);
        return dateFromDate;
      }

      String GetTimeFromDateTime(String date)
      {
        int splitT = date.indexOf("T");
        String  timeFromDate = date.substring(splitT+1, date.lastIndexOf(":" +1));
      
        int firstColon = timeFromDate.indexOf(":");
        int secondColon = timeFromDate.lastIndexOf(":");
        
        // get the substrings for hour, minute second:
        String hourString = timeFromDate.substring(0, firstColon);
        String minString = timeFromDate.substring(firstColon + 1, secondColon);

        String timeToReturn = hourString + ":" + minString;
        return timeToReturn;
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

      void DisplayInformationsOnPages()
      {
        switch (page_counter) {
   
      case 1:{     //Design of home page 1
        lcd.setCursor(0, 0);  
        lcd.print("Temp:" + String(int(temperature)));
        lcd.print((char)223);
        lcd.print("C");
        lcd.setCursor(11, 0);
        lcd.print(String(timeClient.getHours()) + ":" + String(timeClient.getMinutes()));
        lcd.setCursor(0, 1);  
        lcd.print("Umid. aer:" + String(int(humidity)) + "%");
      }
      break;
  
       case 2: { //Design of page 2 
      }
   
       case 3: {   //Design of page 3 
       lcd.setCursor(0,0);
       lcd.print("CONEXIUNEA LA   ");
       lcd.setCursor(0,1);
       lcd.print("WI-FI PIERDUTA  ");
      }
      break;

       case 4: {   //Design of page 3 
       lcd.setCursor(6,0);
       lcd.print("SMART");
       lcd.setCursor(6,1);
       lcd.print("DROP");
      }
      break;

      case 5: {
        lcd.setCursor(0, 0);
        String automationState;
        String workingState;
         if(localSystemState.working)
         {
           workingState = "ON";
         }
         else
         {
          workingState = "OFF";
         }
       lcd.print("Irigare:" + workingState);
       if(SystemStateFromDb.automationMode)
       {
        automationState = "ON";
       }
       else
       {
        automationState = "OFF";
       }
       lcd.setCursor(0,1);
       lcd.print("Irigare aut.:" + automationState);
      }
      
    }//switch
    
       
       if(page_counter == 1){
        Serial.println(WiFi.status());
        if(WiFi.status() == WL_CONNECTED){
         page_counter = 5;
        }
        else{
         page_counter = 3;
        }
       }
       else if (page_counter ==5)
       {
           page_counter = 4;
          
       }
         
       else if(page_counter == 3)
       {
          page_counter = 4;
       }  
       else if(page_counter == 4)
       {
          page_counter = 1;  
       }
    }

    void TurnOnOffIrrigation()
    {     
      Serial.println("interuption");

      if(stateButton == HIGH && previousIrrigationState == LOW){
        if(localSystemState.working == true)
        {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,HIGH);
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,HIGH);
          digitalWrite(RELAYS_PUMP_PIN,HIGH);
          UpdateWorking(true,false,false,board.id);
          intreruptionModeOn = false;
        }
        else if(localSystemState.working == false)
        {
          digitalWrite(RELAYS_WATER_SWITCH_P0_PIN,LOW);
          digitalWrite(RELAYS_WATER_SWITCH_P1_PIN,LOW);
          digitalWrite(RELAYS_PUMP_PIN,LOW);
          UpdateWorking(true,true,false,board.id);
          intreruptionModeOn = true;
        }
       
     }
    }
      
