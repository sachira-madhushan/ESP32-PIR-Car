#include <WiFi.h>

const char *ssid = "ESP32";
const char *password = "sacheera";

const int pirPin = 13;
WiFiClient client;
int pirStatus;
WiFiServer server(80);

void setup() {
  Serial.begin(115200);
  WiFi.softAP(ssid, password);

  Serial.println("Access Point Started");
  Serial.print("IP Address: ");
  Serial.println(WiFi.softAPIP());
  pinMode(pirPin, INPUT);
  server.begin();
  Serial.println(WiFi.localIP());
}

void loop() {
  
  if (client) {
    pirStatus = digitalRead(pirPin);

    sendPirData(client, pirStatus);
  }else{
    client = server.available();
  }
  delay(500);
}

void sendPirData(WiFiClient client, int pirStatus) {
  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: text/html");
  client.println("Connection: close");
  client.println();
  client.print("Respond:");
  client.print(pirStatus);
  client.println();
}
