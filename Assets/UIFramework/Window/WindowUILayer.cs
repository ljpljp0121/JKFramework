using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// ���layer��������еĴ���
    /// ����ʾ��¼�Ͷ��еģ�����һ��ֻ��ʾһ��
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
