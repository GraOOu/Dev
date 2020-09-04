package ai.cheap.petbot.tracking;

import android.os.Environment;

import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.MatOfRect;
import org.opencv.core.Point;
import org.opencv.core.Rect;
import org.opencv.core.Scalar;
import org.opencv.imgproc.Imgproc;

import java.lang.Math;

/**
 * Created by GraOOu on 13/02/2016.
 */
public class WalkingPedestrianTracking extends OpenCVDefaultDetection
{
    /*
        Select the centerer rect
    */

    @Override
    public Mat Process ( Mat mGray )
    {
        MatOfRect bbox = ProcessHOG ( mGray );

        // Compute image center

        Point imgCenter = new Point ( mGray.width ( ) / 2, mGray.height ( ) / 2 );

        Rect    theGoodRect     = null;
        double  theGoodRectDist = Double.MAX_VALUE;

        // Find the rect with the center at the more at the center

        for ( Rect rect : bbox.toArray ( ) )
        {
            Point ptCenter =  new Point ( rect.x + rect.width / 2, rect.y + rect.height / 2 );

            double currentDist =  Math.pow ( ptCenter.x - imgCenter.x, 2 ) + Math.pow ( ptCenter.y - imgCenter.y, 2 );

            if ( currentDist < theGoodRectDist )
            {
                theGoodRect     = rect;
                theGoodRectDist = currentDist;
            }
        }

        // Display result

        if ( theGoodRect != null )
        {
            Point ptMin = new Point ( theGoodRect.x, theGoodRect.y );
            Point ptMax = new Point ( theGoodRect.x + theGoodRect.width, theGoodRect.y + theGoodRect.height );

            Imgproc.rectangle ( mGray, ptMin, ptMax, new Scalar ( 0 ), 2 );

            TrackingContext context = TrackingContext.GetInstance ( );

            // Compute SpeedRatio

            double heightRatio = theGoodRect.height / ( TrackingContext.Height * TrackingContext.HumanSizeRatio );
            context.SpeedRatio = ( 1 / heightRatio ) - 1.0;

            // Compute TurnRation

            int    screenCenter = TrackingContext.Width / 2;
            int    rectCenter   = theGoodRect.x + theGoodRect.width / 2;
            context.TurnRatio   = ( screenCenter - rectCenter ) / screenCenter;

            if ( context._displayInfo )
            {
                // Display SpeedRatio

                double textPosX = 10;
                double textPosY = TrackingContext.Height * 0.85;

                Point  position     = new Point ( textPosX, textPosY );
                double fontScale    = 0.5;

                String infos = "SpeedRatio : ";
                infos += String.format ( "%.2f", context.SpeedRatio );
                Imgproc.putText ( mGray, infos, position, Core.FONT_HERSHEY_COMPLEX_SMALL, fontScale, new Scalar ( 255 ) );

                // Display TurnRation

                position = new Point ( 10, TrackingContext.Height * 0.85 + 12 );

                infos = "TurnRatio : ";
                infos += String.format ( "%.2f", context.TurnRatio );
                Imgproc.putText ( mGray, infos, position, Core.FONT_HERSHEY_COMPLEX_SMALL, fontScale, new Scalar ( 255 ) );
            }

        }

        // Save the value in context

        _context._selectedRect      = theGoodRect;
        _context._selectedDistValue = theGoodRectDist;

        return mGray;
    }
}
