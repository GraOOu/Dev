package ai.cheap.petbot;

import java.util.ArrayList;
import java.util.List;

public class ReplayerController extends Controller {

    List<String> commands = new ArrayList<String>();

    boolean recordActivated = false;
    boolean replayActivated = false;

    int step = 0;

    public void Update ( ParamContainer params ) {

        if ( params.Cmd.contains ( "F" ) )
        {
            recordActivated = true;
            return;
        }

        if ( params.Cmd.contains ( "B" ) )
        {
           replayActivated = true;
           return;
        }

        if ( replayActivated )
        {
            step = step++ % commands.size();
            params.Cmd = commands.get(step);
            return;
        }

        if ( recordActivated )
        {
            commands.add(params.Cmd);
        }
    }
}
