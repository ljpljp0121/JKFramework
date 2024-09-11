using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 这个layer层控制所有的窗口
    /// 有显示记录和队列的，并且一次只显示一个
    /// </summary>
    public class WindowUILayer : UILayer<IWindowController>
    {
        [SerializeField] private WindowParaLayer priorityParaLayer = null;

        public IWindowController CurrentWindow {  get; private set; }

        private Queue<WindowHistoryEntry> windowQueue;
        private Stack<WindowHistoryEntry> windowHistory;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnblock;


        private HashSet<IScreenController> screensTransitioning;

        public override void Initialize()
        {
            base.Initialize();
            registeredScreens = new Dictionary<string, IWindowController>();
            windowQueue = new Queue<WindowHistoryEntry>();
            windowHistory = new Stack<WindowHistoryEntry>();
            screensTransitioning = new HashSet<IScreenController>();
        }
    }

}
