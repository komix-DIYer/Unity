// #include <Adafruit_RGBLCDShield.h>
// Adafruit_RGBLCDShield lcd = Adafruit_RGBLCDShield();

#define SIZE_OF_ARRAY(ary)  (sizeof(ary)/sizeof((ary)[0]))

boolean serial_sendable = false;
int time=0, dt=0, timeElapsed=0;
int val[10];
String s = "";
  
void setup()
{
  Serial.begin(9600);
  
  // lcd.begin(16, 2);
  // lcd.setBacklight(0x7); // White
  
  delay(500);
}


void loop()
{
  dt = millis() - time;
  time = millis();
  timeElapsed += dt;
  
  /*
  lcd.setCursor(0, 0);
  lcd.print(String(dt));
  lcd.setCursor(0, 1);
  lcd.print(String(val[0]) + ", " + String(val[1]));
  */
  
  if (serial_sendable || timeElapsed>1000) {
    Serial.print(dt);
    Serial.print(",");
    Serial.print(timeElapsed);
    Serial.println();
    serial_sendable = false;
    timeElapsed = 0;
  }
  
  delay(35);
  
}

void serialEvent()
{   
  while(1)
  {
    if(Serial.available()==0) break;
    char tmp = Serial.read();
    if(tmp=='\0') break;
    if(tmp=='h')
    {
      s = "";
    }
    else
    {
      s += String(tmp);
    }
  }
  
  String v[10] = {'\0'};
  size_t arraysize = SIZE_OF_ARRAY(v);
  char delimiter = ',';
  int index = split(v, arraysize, s, delimiter);
  for(int i=0;i<index; i++) val[i] = v[i].toInt();
  
  serial_sendable = true;
  // lcd.clear();
}

int split(String *result, size_t resultsize, String data, char delimiter)
{
    int index = 0;
    int datalength = data.length();
    for (int i = 0; i < datalength; i++)
    {
        char tmp = data.charAt(i);
        if ( tmp == delimiter )
        {
            index++;
            if ( index > (resultsize - 1)) return -1;
        }
        else result[index] += tmp;
    }
    return (index + 1);
}



