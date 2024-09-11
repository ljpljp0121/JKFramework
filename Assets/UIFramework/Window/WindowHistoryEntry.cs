using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口记录和队列
    /// </summary>
    public class WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IWindowProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IWindowProperties properties)
        {
            Screen = screen;
            Properties = properties;
        }
    }
}
