using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework.Window;
using Utils;

public class StartDemoSignal : ASignal { }

public class StartWindowController : WindowController
{
    public void UI_Start()
    {
        Signals.Get<StartDemoSignal>().Dispatch();
    }
}

