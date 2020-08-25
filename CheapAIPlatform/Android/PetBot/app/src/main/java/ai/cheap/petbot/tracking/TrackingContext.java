package ai.cheap.petbot.tracking;

import org.opencv.core.Rect;

/**
 * Created by GraOOu on 13/02/2016.
 */
public class TrackingContext
{
    public static int Width    = 240;
    public static int Height   = 320;

    public static double HumanSizeRatio = 0.75;

    public static TrackingContext _instance = null;

    public int _strategyIdx = 0;

    private TrackingContext ( )
    {
        _algo        = algorithm.Default;
        _displayInfo = true;
    }

    public static TrackingContext GetInstance ( )
    {
        if ( _instance == null )
        {
            _instance = new TrackingContext ( );
        }
        return _instance;
    }

    public enum algorithm
    {
        Default,
        Grrr
    }

    public algorithm _algo;

    public Rect     _selectedRect;
    public double   _selectedDistValue;

    public double   _accelerationValue;
    public double   _turnValue;

    public boolean  _displayInfo;
}
