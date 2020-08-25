package ai.cheap.petbot.tracking;

import org.opencv.core.Mat;

/**
 * Created by GraOOu on 13/02/2016.
 */

public abstract class TrackingStrategy
{
    protected TrackingContext _context = null;

    public void Initialize ( TrackingContext context )
    {
        _context = context;
    }

    public abstract Mat Process ( Mat img );
}
