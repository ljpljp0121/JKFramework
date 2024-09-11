using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 基础的UI Layer层
    /// </summary>
    public abstract class UILayer<TScreen> : MonoBehaviour where TScreen : IScreenController
    {
        protected Dictionary<string, TScreen> registeredScreens;

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="screen">界面类型参数</param>
        public abstract void ShowScreen(TScreen screen);

        /// <summary>
        /// 显示一个界面带参数
        /// </summary>
        /// <typeparam name="TProps">属性类型</typeparam>
        /// <param name="screen">界面类型参数</param>
        /// <param name="props">属性参数</param>
        public abstract void ShowScreen<TProps>(TScreen screen, TProps props) where TProps : IScreenProperties;

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="screen">界面类型参数</param>
        public abstract void HideScreen(TScreen screen);

        /// <summary>
        /// 初始化Layer层
        /// </summary>
        public virtual void Initialize()
        {
            registeredScreens = new Dictionary<string, TScreen>();
        }

        /// <summary>
        /// 传进来的界面当作层的子节点
        /// </summary>
        /// <param name="controller">界面的controller</param>
        /// <param name="screenTransform">界面节点</param>
        public virtual void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            screenTransform.SetParent(transform, false);
        }

        /// <summary>
        /// 注册界面的controller带上明确的界面ID
        /// </summary>
        /// <param name="screenID">界面ID</param>
        /// <param name="controller">界面controller</param>
        public void RegisterScreen(string screenID, TScreen controller)
        {
            if (!registeredScreens.ContainsKey(screenID))
            {
                ProcessScreenRegister(screenID, controller);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen controller already registered for id:" + screenID);
            }
        }

        /// <summary>
        /// 根据ID取消注册界面的controller
        /// </summary>
        /// <param name="screenID">界面id</param>
        /// <param name="controller">被取消的界面controller</param>
        private void UnregisterScreen(string screenID, TScreen controller)
        {
            if (registeredScreens.ContainsKey(screenID))
            {
                ProcessScreenUnRegister(screenID, controller);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen controller not registered for id:" + screenID);
            }
        }

        /// <summary>
        /// 根据ID显示界面
        /// </summary>
        /// <param name="screenID">界面ID</param>
        public void ShowScreenByID(string screenID)
        {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenID, out ctl))
            {
                ShowScreen(ctl);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen  ID " + screenID + "not registered to this layer");
            }
        }

        /// <summary>
        /// 根据ID显示界面,带上具体的属性参数
        /// </summary>
        /// <typeparam name="TProps">属性类型</typeparam>
        /// <param name="screenID">界面ID</param>
        /// <param name="properties">属性参数</param>
        public void ShowScreenByID<TProps>(string screenID,TProps properties) where TProps : IScreenProperties
        {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenID, out ctl))
            {
                ShowScreen(ctl,properties);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen  ID " + screenID + "not registered to this layer");
            }
        }

        /// <summary>
        /// 根据ID隐藏界面
        /// </summary>
        /// <param name="screenID">界面ID</param>
        public void HideScreenByID(string screenID)
        {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenID, out ctl))
            {
                HideScreen(ctl);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen  ID " + screenID + "not registered to this layer");
            }
        }

        /// <summary>
        /// 根据ID查看是否注册
        /// </summary>
        /// <param name="screenID">界面ID</param>
        /// <returns></returns>
        public bool IsScreenRegistered(string screenID)
        {
            return registeredScreens.ContainsKey(screenID);
        }

        /// <summary>
        /// 隐藏所有界面
        /// </summary>
        /// <param name="shouldAnimateWhenHiding">隐藏的时候是否需要动画</param>
        public virtual void HideAll(bool shouldAnimateWhenHiding = true)
        {
            foreach (var screen in registeredScreens)
            {
                screen.Value.Hide(shouldAnimateWhenHiding);
            }
        }

        protected virtual void ProcessScreenRegister(string screenID, TScreen controller)
        {
            controller.ScreenID = screenID;
            registeredScreens.Add(screenID, controller);
            controller.ScreenDestroyed += OnScreenDestroyed;
        }

        protected virtual void ProcessScreenUnRegister(string screenID, TScreen controller)
        {
            registeredScreens.Remove(screenID);
            controller.ScreenDestroyed -= OnScreenDestroyed;
        }

        private void OnScreenDestroyed(IScreenController screen)
        {
            if (!string.IsNullOrEmpty(screen.ScreenID)
                && registeredScreens.ContainsKey(screen.ScreenID))
            {
                UnregisterScreen(screen.ScreenID, (TScreen)screen);
            }
        }


    }
}
