#ifndef __MEAS_H__
#define __MEAS_H__
 
#include <Arduino.h>
#include "Utils.h"
 
class Meas 
{
    public:
    
      int line;
      int type;
      
      double gain;
      double offset;
      int    intValue;
      double value;
      
      double mean;
      double stability;
};

// Get Meas ( Analogic ) --------------------------------------------------------------------

double GetMeas ( Meas* meas )
{
  meas->intValue = analogRead ( meas->line );
  meas->value = ( meas->intValue * 5.0 / 1023 ) * meas->gain + meas->offset;
      
  meas->mean = ( meas->mean * meas->stability ) + ( meas->value * ( 1 - meas->stability ) );
  
  return meas->mean;
}
 
#endif //__MEAS_H__


