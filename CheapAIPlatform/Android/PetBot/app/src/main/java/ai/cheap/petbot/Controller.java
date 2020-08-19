package ai.cheap.petbot;

import org.opencv.core.Mat;
import org.opencv.imgproc.Imgproc;

public abstract  class Controller {

    // Debug Text
    /*
        Imgproc.putText ( mat, cmd.trim(), new Point(10,50),0,1.5
            , new Scalar(0,255,0),3);
    */

    abstract void Update ( ParamContainer params );

}
