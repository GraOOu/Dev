#include <Arduino.h>

#include "Utils.h"
#include "Meas.h"

#include "MotorShield.h"

// Init  --------------------------------------------------------------------------------

bool _isFutherTest = false;

// Ultrasonic Sensor

const int trigPin = 9;
const int echoPin = 10;

// RF receiver

const int frontPin = A4;
const int backPin  = A5;
const int rightPin = A0;
const int leftPin  = A1;

// Motor

MotorShield motor = MotorShield ( );

// Direction

#define __Motor_Left__      1
#define __Motor_Right__     2
#define __Motor_Front__     4
#define __Motor_Rear__      8

#define __Motor_Stop__     16
#define __Motor_Ahead__    32

#define  __NoDetection__  512

int newDir = __NoDetection__;
int _currentDir   = __NoDetection__;

// Init ---------------------------------------------------------------------------------

void setup ( ) 
{
  // Connexion PC
  
  Serial.begin ( _isFutherTest ? 115200 : 9600 );
  
  // Config Pin

  // Ultrasonic Sensor

  pinMode ( trigPin, OUTPUT ); 
  pinMode ( echoPin, INPUT  );

  digitalWrite ( trigPin, LOW );

  // Motor Shield

  motor.Init ( );

  Serial.println ( "Setup done..." ); 
  
  // InitTimersSafe ( ); 
}

// Serial Command Handling ---------------------------------------------------------------------------------

char* info = "Grrr!!!";

void SerialCmdHandling ( )
{
  String received;

  // Serial.println ( info ); 

  if ( getLine ( ) )
  {
    received = String ( receivedChars );

    // Reset Steering
    newDir = 0;
    
    if ( received.indexOf ( 'F' ) >= 0 ) 
    {
        newDir |= __Motor_Front__;
        info = "Motor Foward !";
     }

    if ( received.indexOf ( 'B' ) >= 0 )
    {
        newDir |= __Motor_Rear__;
        info = "Motor Back !";
    }

    if ( received.indexOf ( 'R' ) >= 0 )
    {
        newDir |= __Motor_Right__;
        info = "Motor Right !";
    }
    
    if ( received.indexOf ( 'L' ) >= 0 )
    {
        newDir |= __Motor_Left__;
        info = "Motor Left !";
    }

    if ( received.indexOf ( 'S' ) >= 0 )
    {
        newDir |= __Motor_Stop__;
        info = "Motor Stop";
    }

    if ( received.indexOf ( 'A' ) >= 0 )
    {
        newDir |= __Motor_Ahead__;
        info = "Motor Ahead";
    }
    
    if ( received == 'i' ) 
    {
        displayInfo = !displayInfo;
    }
    
    // Serial.println ( info ); 
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
  float distance = duration* SOUND_SPEED / 2;
  
  // Prints the distance on the Serial Monitor
  // Serial.print ( "Distance: " );
  // Serial.println ( distance );

  return distance;
}

// Little car gameplay --------------------------------------------------------------------

// Remote command doesn't have the Ahead or Stop button
// To handle this it is needed to reset the steering after a short latency

int _AnalogThreshold = 1000;
bool _RcCommandGiven = false;
bool _RLCommandGiven = false;

bool RcCmdHandling ( )
{
  //  ---
  
  int frontPinState = analogRead ( frontPin );
  int backPinState  = analogRead ( backPin );
  int rightPinState = analogRead ( rightPin );
  int leftPinState  = analogRead ( leftPin );

  // Handle Reset Steering

  
   int nextDir = 0;
   // Todo
   // Serial.println ( "AS" );    
  
  
  if ( frontPinState >= _AnalogThreshold )
  {
    nextDir |= __Motor_Front__;
    Serial.println ( "F" );
    _RcCommandGiven = true;
  }

  if ( backPinState >= _AnalogThreshold )
  {
    nextDir |= __Motor_Rear__;
    Serial.println ( "B" );
    _RcCommandGiven = true;
  }
 
  if ( rightPinState >= _AnalogThreshold )
  {
    nextDir |= __Motor_Right__;
    Serial.println ( "R" );    
    _RLCommandGiven = true;
  }
  
  if ( leftPinState >= _AnalogThreshold )
  {
    nextDir |= __Motor_Left__;
    Serial.println ( "L" );    
    _RLCommandGiven = true;
  }  
  
  if ( _RcCommandGiven && ( nextDir == 0 ) )
  {
    newDir = __Motor_Stop__;
    Serial.println ( "S" ); 
    _RcCommandGiven = false;
    return true;
  }

  if ( _RLCommandGiven && ( nextDir == 0 ) )
  {
    newDir = __Motor_Ahead__;
    Serial.println ( "A" ); 
    _RLCommandGiven = false;
    return true;
  }
  
  if ( nextDir != 0 )
  {
    newDir = nextDir;
  }

  return true;
}

// Main ---------------------------------------------------------------------------------

unsigned long _period         = 50;
unsigned long _prevMillis     = -1;
unsigned long _meanDelay      = 50;
unsigned long _delayStability = 8;

void loop ( ) 
{
  // Meas
  
  ClearInfo ( );

  // DisplayInfo ( "☻" );

  // newDir = __NoDetection__;

  {
    // Serial Cmd
    SerialCmdHandling ( );

    // Ultrasonic Sensor
    CollisionDetection ( );
  
    // Cmd RadioCmd
    RcCmdHandling ( );
  }

  if ( newDir != _currentDir ) 
  {
    // Execute resulting command

    if ( newDir & __Motor_Front__ ) 
    {
      motor.Foward ( );  
    }

    if ( newDir & __Motor_Rear__ ) 
    {
      motor.Backward ( );
    }

    if ( newDir & __Motor_Right__ ) 
    {
      motor.Right ( );
    }

    if ( newDir & __Motor_Left__ ) 
    {
      motor.Left ( );
    }

    if ( newDir & __Motor_Stop__ ) 
    {
      motor.Stop ( );
    }

    if ( newDir & __Motor_Ahead__ ) 
    {
      motor.Ahead ( );
    }

    
    // No Cmd
    /*
    if ( newDir == __NoDetection__ ) 
    {
      motor.Stop ( );
      motor.Ahead ( );
    
      // return false;
    } 
    */ 
  
    // New cmd stop first
    
    // if ( newDir != _currentDir ) 
    {
      // Serial.println ( "New Command" ); 
      _currentDir = newDir;
    }
    
  }
  
  // ---

  // RT regulation
  // Fail after 50 days
  
  unsigned long now = millis ( );
  if ( _prevMillis == -1 ) _prevMillis = now;
  unsigned long timePerf = now - _prevMillis;
  
   _meanDelay = ( _meanDelay * _delayStability + ( _period - timePerf ) ) / ( _delayStability + 1 );

  if ( _meanDelay < _period ) delay ( _meanDelay );

  _prevMillis = millis ( );
  
  if ( timePerf > _period ) 
  {
    DisplayInfo ( "Perf echec ! : ", true );
    DisplayInfo ( String ( timePerf ).c_str ( ), true );
    DisplayInfo ( "ms ! \n" );
    
    return;
  }
}
