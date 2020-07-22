#ifndef __MotorShield_H__
#define __MotorShield_H__
 
#include <Arduino.h>
#include "Utils.h"

#include <AFMotor.h>

class MotorShield 
{
    public:
    
      // Front - Back
      
      AF_DCMotor* motorFbFront;
      AF_DCMotor* motorFbBack;

      // Left - Right
      
      AF_DCMotor* motorRl;

      Motor ( )
      {
        motorFbFront = new AF_DCMotor ( 1 );
        motorFbBack  = new AF_DCMotor ( 2 );

        motorRl = new AF_DCMotor ( 3 );
      }
      
      void Init ( )
      {
        motorFbFront->setSpeed ( 0 );
        motorFbFront->run ( RELEASE );

        motorFbBack->setSpeed ( 0 );
        motorFbBack->run ( RELEASE );

        motorRl->setSpeed ( 0 );
        motorRl->run ( RELEASE );
      }

      void Foward ( )
      {
        motorFbFront->run ( FORWARD );
        motorFbFront->setSpeed ( 200 );

        motorFbBack->run ( FORWARD );
        motorFbBack->setSpeed ( 200 );
      }

      void Backward ( )
      {
        motorFbFront->run ( BACKWARD );
        motorFbFront->setSpeed ( 200 );

        motorFbBack->run ( BACKWARD );
        motorFbBack->setSpeed ( 200 );
      }

      void Stop ( )
      {
        motorFbFront->setSpeed ( 0 );
        motorFbFront->run ( RELEASE );

        motorFbBack->setSpeed ( 0 );
        motorFbBack->run ( RELEASE );
      }

      void Left ( )
      {
        motorRl->run ( FORWARD );
        motorRl->setSpeed ( 200 );
      }

      void Right ( )
      {
        motorRl->run ( BACKWARD );
        motorRl->setSpeed ( 200 );
      }

      void Ahead ( )
      {
        motorRl->setSpeed ( 0 );
        motorRl->run ( RELEASE );
      }
};


#endif //__MotorShield_H__


