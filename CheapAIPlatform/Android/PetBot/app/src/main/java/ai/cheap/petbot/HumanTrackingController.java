package ai.cheap.petbot;

import org.opencv.core.Point;
import org.opencv.core.Scalar;
import org.opencv.imgproc.Imgproc;

import ai.cheap.petbot.tracking.WalkingPedestrianTracking;
import ai.cheap.petbot.tracking.TrackingContext;
import ai.cheap.petbot.tracking.TrackingStrategy;


public class HumanTrackingController extends Controller {

    TrackingStrategy _trackingStrategy;
    TrackingContext  _trackingContext;

    public static double GoThreshold = 0.15;
    public static double TurnLeftThreshold  =  0.25;
    public static double TurnRightThreshold = -0.25;

    HumanTrackingController ( ) {

        _trackingContext = TrackingContext.GetInstance ( );
        _trackingContext._displayInfo = true;

        _trackingStrategy = new WalkingPedestrianTracking( );
        _trackingStrategy.Initialize ( _trackingContext );

    }

    public void Update ( ParamContainer params ) {

        params.Frame = _trackingStrategy.Process ( params.Frame );
        params.Cmd = "SA";

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

        if ( _trackingContext.TurnRatio > TurnLeftThreshold )
        {
            params.Cmd = "FL";
        }
        else if ( _trackingContext.TurnRatio < TurnRightThreshold )
        {
            params.Cmd = "FR";
        }
        else
        {
            params.Cmd += "A";
        }

        if ( _trackingContext._displayInfo )
        {
            // Debug Text
            Imgproc.putText ( params.Frame, params.Cmd, new Point(10,50),0,1.5
                            , new Scalar(0,255,0),3);
        }
    }
}
