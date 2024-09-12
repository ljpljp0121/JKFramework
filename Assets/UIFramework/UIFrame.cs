using System;
using UIFramework.Panel;
using UIFramework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 所有的对外接口都在这，相当于UIManager之类
    /// </summary>
    public class UIFrame : MonoBehaviour
    {
        [Tooltip("如果您想手动初始化此UI框架,请将其设置为false")]
        [SerializeField] private bool initializeOnAwake = true;

        private PanelUILayer panelLayer;
        private WindowUILayer windowLayer;

        private Canvas mainCanvas;
        private GraphicRaycaster graphicRaycaster;

        /// <summary>
        /// 主Canvas
        /// </summary>
        public Canvas MainCanvas
        {
            get
            {
                if (mainCanvas == null)
                {
                    mainCanvas = GetComponent<Canvas>();
                }

                return mainCanvas;
            }
        }

        /// <summary>
        /// 主Canvas的摄像机
        /// </summary>
        public Camera UICamera
        {
            get { return MainCanvas.worldCamera; }
        }

        private void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            if (panelLayer == null)
            {
                panelLayer = gameObject.GetComponentInChildren<PanelUILayer>(true);
                if (panelLayer == null)
                {
                    Debug.LogError("[UI Frame] UI Frame lacks Panel Layer!");
                }
                else
                {
                    panelLayer.Initialize();
                }
            }

            if (windowLayer == null)
            {
                windowLayer = gameObject.GetComponentInChildren<WindowUILayer>(true);
                if (windowLayer == null)
                {
                    Debug.LogError("[UI Frame] UI Frame lacks Window Layer!");
                }
                else
                {
                    windowLayer.Initialize();
                    windowLayer.RequestScreenBlock += OnRequestScreenBlock;
                    windowLayer.RequestScreenUnblock += OnRequestScreenUnblock;
                }
            }

            graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();

        }

        /// <summary>
        /// 通过ID显示一个面板
        /// </summary>
        public void ShowPanel(string screenID)
        {
            panelLayer.ShowScreenByID(screenID);
        }

        /// <summary>
        /// 通过ID和属性显示一个面板
        /// </summary>
        public void ShowPanel<T>(string screenID, T properties) where T : IPanelProperties
        {
            panelLayer.ShowScreenByID<T>(screenID, properties);
        }

        /// <summary>
        /// 通过ID隐藏一个面板
        /// </summary>
        public void HidePanel(string screenID)
        {
            panelLayer.HideScreenByID(screenID);
        }

        /// <summary>
        /// 通过ID显示窗口
        /// </summary>
        public void OpenWindow(string screenID)
        {
            windowLayer.ShowScreenByID(screenID);
        }

        /// <summary>
        /// 通过ID隐藏窗口
        /// </summary>
        public void CloseWindow(string screenID)
        {
            windowLayer.HideScreenByID(screenID);
        }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        public void CloseCurrentWindow()
        {
            if (windowLayer.CurrentWindow != null)
            {
                CloseWindow(windowLayer.CurrentWindow.ScreenID);
            }
        }

        /// <summary>
        /// 通过ID显示窗口并传递属性参数
        /// </summary>
        public void OpenWindow<T>(string screenID, T properties) where T : IWindowProperties
        {
            windowLayer.ShowScreenByID<T>(screenID, properties);
        }


        /// <summary>
        /// 通过id去显示，搜索到什么就是什么
        /// </summary>
        /// <param name="screenID"></param>
        public void ShowScreen(string screenID)
        {
            Type type;
            if (IsScreenRegistered(screenID, out type))
            {
                if (type == typeof(IWindowController))
                {
                    OpenWindow(screenID);
                }
                else if (type == typeof(IPanelController))
                {
                    ShowPanel(screenID);
                }
            }
            else
            {
                Debug.LogError($"Tried to open Screen id{screenID} but is is not registered as Window or Panel");
            }
        }

        /// <summary>
        /// 注册一个界面，如果传了screenTransform,相当于制定了父节点
        /// </summary>
        public void RegisterScreen(string screenID, IScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window != null)
            {
                windowLayer.RegisterScreen(screenID, window);
                if (screenTransform != null)
                {
                    windowLayer.ReparentScreen(controller, screenTransform);
                }

                return;
            }

            IPanelController panel = controller as IPanelController;
            if (panel != null)
            {
                panelLayer.RegisterScreen(screenID, panel);
                if (screenTransform != null)
                {
                    panelLayer.ReparentScreen(controller, screenTransform);
                }
            }
        }

        /// <summary>
        /// 注册一个面板,不注册显示不出来的
        /// </summary>
        public void RegisterPanel<TPanel>(string screenID, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.RegisterScreen(screenID, controller);
        }

        /// <summary>
        /// 注销一个面板
        /// </summary>
        public void UnregisterPanel<TPanel>(string screenID, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.UnregisterScreen(screenID, controller);
        }

        /// <summary>
        /// 注册一个窗口,不注册显示不出来的
        /// </summary>
        public void RegisterWindow<TWindow>(string screenID, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.RegisterScreen(screenID, controller);
        }

        /// <summary>
        /// 注销一个窗口
        /// </summary>
        public void UnregisterWindow<TWindow>(string screenID, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.UnregisterScreen(screenID, controller);
        }

        /// <summary>
        /// 根据面板ID检测是否开启中
        /// </summary>
        public bool IsPanelOpen(string panelID)
        {
            return panelLayer.IsPanelVisible(panelID);
        }

        /// <summary>
        /// 隐藏所有界面
        /// </summary>
        public void HdieAll(bool animate = true)
        {
            HideAllPanels(animate);
            CloseAllWindows(animate);
        }

        /// <summary>
        /// 隐藏所有面板层界面
        /// </summary>
        private void HideAllPanels(bool animate)
        {
            panelLayer.HideAll(animate);
        }

        /// <summary>
        /// 隐藏所有窗口层界面
        /// </summary>
        private void CloseAllWindows(bool animate)
        {
            windowLayer.HideAll(animate);
        }

        /// <summary>
        /// 检查界面是否被注册过
        /// </summary>
        public bool IsScreenRegistered(string screenID)
        {
            if (windowLayer.IsScreenRegistered(screenID))
            {
                return true;
            }

            if (panelLayer.IsScreenRegistered(screenID))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查界面是否被注册过，但多了一个类型返回
        /// </summary>
        public bool IsScreenRegistered(string screenID, out Type type)
        {
            if (windowLayer.IsScreenRegistered(screenID))
            {
                type = typeof(IWindowController);
                return true;
            }

            if (panelLayer.IsScreenRegistered(screenID))
            {
                type = typeof(IPanelController);
                return true;
            }
            type = null;
            return false;
        }

        private void OnRequestScreenBlock()
        {
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = false;
            }
        }

        private void OnRequestScreenUnblock()
        {
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = true;
            }
        }
    }
}
