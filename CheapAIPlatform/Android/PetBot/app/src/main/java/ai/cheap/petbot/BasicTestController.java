package ai.cheap.petbot;

import org.opencv.core.Point;
import org.opencv.core.Scalar;
import org.opencv.imgproc.Imgproc;

public class BasicTestController {

    // Small Automat
    private String[] commands = new String[] { "S", "A", "F", "L", "R", "B", "A", "S" };
    private int nextStepDuration = 20;
    private int step = 0;

    public void Update ( ParamContainer params ) {

        step++;
        int cmdIdx = ( step / nextStepDuration) % commands.length;
        params.Cmd = commands [ cmdIdx ];

        // Debug Text
        Imgproc.putText ( params.Frame, params.Cmd.trim(), new Point(10,50),0,1.5
                , new Scalar(0,255,0),3);

    }

}
