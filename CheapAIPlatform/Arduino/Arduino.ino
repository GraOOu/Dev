#include <Arduino.h>

#include "Utils.h"
#include "Meas.h"


// INIT  --------------------------------------------------------------------------------

bool _isFutherTest = false;

// Ultrasonic Sensor

const int trigPin = 9;
const int echoPin = 10;

// RF receiver

const int frontPin = A4;
const int backPin  = A5;
const int rightPin = A0;
const int leftPin  = A1;

void setup ( ) 
{
  // Connexion PC
  
  Serial.begin ( _isFutherTest ? 115200 : 9600 );
  
  // Config Pin

  // Ultrasonic Sensor

  pinMode ( trigPin, OUTPUT ); 
  pinMode ( echoPin, INPUT  );

  digitalWrite ( trigPin, LOW );

  // RF receiver
/*
  pinMode ( frontPin, INPUT ); 
  pinMode ( backPin,  INPUT  );

  pinMode ( rightPin, INPUT ); 
  pinMode ( leftPin,  INPUT  );
*/
  Serial.println ( "Setup done..." ); 
  
  // InitTimersSafe ( ); 
}


// Serial Command Handling ---------------------------------------------------------------------------------

char* info = "Grrr!!!";

void SerialCmdHandling ( )
{
  int received;

  // Serial.println ( info ); 

  if ( getLine ( ) )
  {
    received = receivedChars [ 0 ];
    
    if ( received == 'f' ) 
    {
        // double speed = atof ( &( receivedChars [ 1 ] ) );
      
        info = "Motor Foward !";
     }

    if ( received == 'b' ) 
    {
        info = "Motor Back !";
    }
    
    if ( received == 'l' ) 
    {
        info = "Motor Left !";
    }
    
    if ( received == 'r' ) 
    {
        info = "Motor Right !";
    }
    
    if ( received == 'i' ) 
    {
        displayInfo = !displayInfo;
    }
    
    if ( received == 's' ) 
    {
        info = "Motor Stop";
    }

    Serial.println ( info ); 
  }  
}


// Ultrasonic Sensor HC-SR04 --------------------------------------------------------------------

const unsigned long MEASURE_TIMEOUT = 25000UL; // 25ms = ~8m à 340m/s

const float SOUND_SPEED = 340.0 / 1000;

int CollisionDetection ( )
{
  // Sets the trigPin on HIGH state for 10 micro seconds
  digitalWrite ( trigPin, HIGH );
  delayMicroseconds ( 10 );
  digitalWrite ( trigPin, LOW );
  
  // Reads the echoPin, returns the sound wave travel time in microseconds
  long duration = pulseIn ( echoPin, HIGH, MEASURE_TIMEOUT );
  
  // Calculating the distance
  float distance = duration* SOUND_SPEED/ 2;
  
  // Prints the distance on the Serial Monitor
  // Serial.print ( "Distance: " );
  // Serial.println ( distance );

  return distance;
}

// Little car gameplay --------------------------------------------------------------------

#define __Motor_Left__      0
#define __Motor_Right__     2
#define __Motor_Front__     4
#define __Motor_Rear__      8

#define  __NoDetection__    9

int    _currentDir   = __NoDetection__;

int    _AnalogThreshold = 1000;

bool LittleCarGameplayAttitude ( )
{
  int newDir = __NoDetection__;
  
  //  ---
  
  int frontPinState = analogRead ( frontPin );
  int backPinState  = analogRead ( backPin );
  int rightPinState = analogRead ( rightPin );
  int leftPinState  = analogRead ( leftPin );
  
  if ( frontPinState >= _AnalogThreshold )
  {
    newDir = __Motor_Front__;
    // DisplayInfo ( String ( FBValue ).c_str ( ) );
    Serial.println ( "Front" );
  }
  
  if ( backPinState >= _AnalogThreshold )
  {
    newDir = __Motor_Rear__;
    Serial.println ( "Back" );
  }
 
  if ( rightPinState >= _AnalogThreshold )
  {
    newDir = __Motor_Right__;
    Serial.println ( "Right" );
  }
  
  if ( leftPinState >= _AnalogThreshold )
  {
    newDir = __Motor_Left__;
    Serial.println ( "Left" );
  }  
  
  // No Cmd
  
  if ( newDir == __NoDetection__ ) 
  {
    return false;
  }  
  
  // New cmd stop first
    
  if ( newDir != _currentDir ) 
  {
    Serial.println ( "New Command" ); 
    _currentDir   = newDir;
  }
    
  // Translate to engine
  
  // DisplayInfo ( "Action" );

  return true;
}

// Main ---------------------------------------------------------------------------------

unsigned long _period         = 20;
unsigned long _prevMillis     = -1;
unsigned long _meanDelay      = 20;
unsigned long _delayStability = 8;

void loop ( ) 
{
  // Meas
  
  ClearInfo ( );

  // DisplayInfo ( "☻" );

  {
    // Serial Cmd
    SerialCmdHandling ( );

    // Ultrasonic Sensor
    CollisionDetection ( );
  
    // Cmd RadioCmd
    LittleCarGameplayAttitude ( );
  }
  
  // ---

  // Régulation temps réel
  // Echec au bout de 50 jours de temps de fonctionnement
  
  unsigned long now = millis ( );
  if ( _prevMillis == -1 ) _prevMillis = now;
  unsigned long timePerf = now - _prevMillis;
  _prevMillis = now;
  
   _meanDelay = ( _meanDelay * _delayStability + ( _period - timePerf ) ) / ( _delayStability + 1 );

  if ( _meanDelay < _period ) delay ( _meanDelay );
  
  if ( timePerf > _period ) 
  {
    DisplayInfo ( "Perf echec ! : ", true );
    DisplayInfo ( String ( timePerf ).c_str ( ), true );
    DisplayInfo ( "ms ! \n" );
    
    return;
  }
}
