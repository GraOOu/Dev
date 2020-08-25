package ai.cheap.petbot.tracking;

import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.MatOfDouble;
import org.opencv.core.MatOfRect;
import org.opencv.core.Point;
import org.opencv.core.Rect;
import org.opencv.core.Scalar;
import org.opencv.imgproc.Imgproc;
import org.opencv.objdetect.HOGDescriptor;

import java.lang.reflect.Array;

/**
 * Created by GraOOu on 13/02/2016.
 */
public class OpenCVDefaultDetection extends TrackingStrategy
{
    HOGDescriptor   hog;

    @Override
    public void Initialize ( TrackingContext context )
    {
        super.Initialize ( context );

        // From PySearch

        hog = new HOGDescriptor ( );
        hog.setSVMDetector ( HOGDescriptor.getDefaultPeopleDetector ( ) );
    }

    protected void DrawCircle ( Mat img )
    {
        int y = img.rows ( ) / 2;
        int x = img.cols ( ) / 2;

        Imgproc.circle ( img, new Point ( x, y ), x / 4, new Scalar ( 0 ), 2 );
    }

    /*
     Requires grayscale ( the approach works on grayscale images only )
     */

    protected MatOfRect ProcessHOG ( Mat mGray )
    {
        // Detect people in the image

        MatOfRect   bbox    = new MatOfRect   ( );
        MatOfDouble weights = new MatOfDouble ( );

        hog.detectMultiScale ( mGray, bbox, weights );

        return bbox;
    }

    @Override
    public Mat Process ( Mat mGray )
    {
        MatOfRect bbox = ProcessHOG ( mGray );

        //  Draw the original bounding boxes

        Rect [ ] bboxes = bbox.toArray ( );

        for ( Rect rect : bboxes )
        {
            Point ptFirst   = new Point ( rect.x, rect.y );
            Point ptSecond  = new Point ( rect.x + rect.width, rect.y + rect.height );

            Imgproc.rectangle ( mGray, ptFirst, ptSecond, new Scalar ( 0 ), 2 );
        }

        TrackingContext context = TrackingContext.GetInstance ( );

        if ( context._displayInfo )
        {
            String infos = "NbHumans : ";
            infos += bboxes.length;

            Point  position  = new Point ( 10, TrackingContext.Height * 0.85 );
            double fontScale = 0.5;

            Imgproc.putText ( mGray, infos, position, Core.FONT_HERSHEY_COMPLEX_SMALL, fontScale, new Scalar ( 255 ) );
        }

        return mGray;
    }
}
