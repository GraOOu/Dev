package ai.cheap.petbot;

import ai.cheap.petbot.tracking.WalkingPedestrianTracking;
import ai.cheap.petbot.tracking.TrackingContext;
import ai.cheap.petbot.tracking.TrackingStrategy;


public class HumanTrackingController extends Controller {

    TrackingStrategy _trackingStrategy;
    TrackingContext  _trackingContext;

    public static double GoThreshold = 0.25;
    public static double TurnLeftThreshold  = -0.2;
    public static double TurnRightThreshold =  0.2;

    HumanTrackingController ( ) {

        _trackingContext = TrackingContext.GetInstance ( );
        _trackingContext._displayInfo = true;

        _trackingStrategy = new WalkingPedestrianTracking( );
        _trackingStrategy.Initialize ( _trackingContext );

    }

    public void Update ( ParamContainer params ) {

        params.Frame = _trackingStrategy.Process ( params.Frame );
        params.Cmd = null;

        // Speed

        if ( _trackingContext.SpeedRatio > GoThreshold )
        {
            params.Cmd = "F";
        }
        else
        {
            params.Cmd = "S";
        }

        // Steering

        if ( _trackingContext.TurnRatio < TurnLeftThreshold )
        {
            params.Cmd = "FL";
        }
        else if ( _trackingContext.TurnRatio > TurnRightThreshold )
        {
            params.Cmd = "FR";
        }
        else
        {
            params.Cmd += "A";
        }
    }
}
