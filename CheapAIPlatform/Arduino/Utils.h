#ifndef __UTILS_H__
#define __UTILS_H__
 
#include <Arduino.h>

// UTILZ ---------------------------------------------------------------------------------


double ComputeLinear ( double ratio, double a, double b )
{
  double result = ratio * a + ( 1.0 - ratio ) * b;
  
  return result;
}


bool ClipValue ( double value, double minVal, double maxVal )
{
  return ( ( value < minVal ) || ( value > maxVal ) );
}


int pwmPinValue ( double ratio )
{
  return ( ( int ) ( round ( 256.0 * ratio ) ) );
}

// a remplacer par des String

char floatStr [ 8 ];

char* DoubleToString ( double val )
{
  dtostrf ( val, 7, 3, floatStr );
  return floatStr;
}

// ---

#define InfoLen  256
#define DebugTempo               2000 // ms

char infoBuf [ InfoLen ]; 
int  infoIdx = 0;
bool displayInfo = true;
int  cptDisplayInfo = 0;

void DisplayInfo  ( const char* info, bool stack  = false  )
{
  if ( !displayInfo  ) return ;
  
  int len = strlen ( info ) + 1;
  
  // Test Overflow
  
  if ( infoIdx + len >= ( InfoLen - 1 ) )
  {
    Serial.print ( "Too much info !" );
    return;
  }
  
  // Copy info
  
  strncpy ( &(infoBuf [ infoIdx ]), info, len );
  infoIdx += len;
  infoBuf [ infoIdx ] = 0;
  infoIdx--;
  
  // Display
  
  if ( !stack && displayInfo && ( cptDisplayInfo > DebugTempo ) )
  {
      Serial.println ( infoBuf );
      
      infoIdx = 0;
      infoBuf [ 0 ] = 0;
      cptDisplayInfo = 0;
  }
}

void ClearInfo ( )
{
  infoIdx = 0;
  infoBuf [ 0 ] = 0;
}


// COM PC ---------------------------------------------------------------------------------

const byte numChars = 32; // Ugly
char       receivedChars [ numChars ];


bool getLine ( ) 
{
  static byte ndx = 0;
  char endMarker = '\n';
  char rc;
	
  while ( Serial.available ( ) > 0 )
  {
    rc = Serial.read ( );

    if ( rc != endMarker ) 
    {
      receivedChars [ ndx ] = rc;
      ndx++;
      if ( ndx >= numChars ) 
      {
        ndx = numChars - 1;
        Serial.println ( "Serial Error : Line too large" );
      }
    }
    else 
    {
      receivedChars [ ndx ] = '\0'; // terminate the string
      ndx = 0;
      return true;
    }
  }
  return false;
}

#endif //__UTILS_H__

