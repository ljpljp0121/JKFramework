using System;
using UIFramework.Panel;
using UIFramework.Window;

namespace UIFramework
{
    /// <summary>
    /// 所有UI界面必须实现的接口
    /// </summary>
    public interface IScreenController
    {
        string ScreenID { get; set; }
        bool IsVisible { get; }

        void Show(IScreenProperties props = null);

        void Hide(bool animate = true);

        Action<IScreenController> ScreenDestroyed { get; set; }
    }

    /// <summary>
    /// 所有面板必须实现的接口
    /// </summary>
    public interface IPanelController : IScreenController
    {
        PanelPriority Proiority { get; }
    }

    /// <summary>
    /// 所有窗口必须实现的接口
    /// </summary>
    public interface IWindowController : IScreenController
    {
        bool HideOnForegroundLost { get; }
        bool IsPopup { get; }
        WindowPriority WindowPriority { get; }
    }
}
