using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// ������UI Layer��
    /// </summary>
    public abstract class UILayer<TScreen> : MonoBehaviour where TScreen : IScreenController
    {
        protected Dictionary<string, TScreen> registeredScreens;

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="screen">�������Ͳ���</param>
        public abstract void ShowScreen(TScreen screen);

        /// <summary>
        /// ��ʾһ�����������
        /// </summary>
        /// <typeparam name="TProps">��������</typeparam>
        /// <param name="screen">�������Ͳ���</param>
        /// <param name="props">���Բ���</param>
        public abstract void ShowScreen<TProps>(TScreen screen, TProps props) where TProps : IScreenProperties;

        /// <summary>
        /// ���ؽ���
        /// </summary>
        /// <param name="screen">�������Ͳ���</param>
        public abstract void HideScreen(TScreen screen);

        /// <summary>
        /// ��ʼ��Layer��
        /// </summary>
        public virtual void Initialize()
        {
            registeredScreens = new Dictionary<string, TScreen>();
        }

        /// <summary>
        /// �������Ľ��浱������ӽڵ�
        /// </summary>
        /// <param name="controller">�����controller</param>
        /// <param name="screenTransform">����ڵ�</param>
        public virtual void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            screenTransform.SetParent(transform, false);
        }

        /// <summary>
        /// ע������controller������ȷ�Ľ���ID
        /// </summary>
        /// <param name="screenID">����ID</param>
        /// <param name="controller">����controller</param>
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
        /// ����IDȡ��ע������controller
        /// </summary>
        /// <param name="screenID">����id</param>
        /// <param name="controller">��ȡ���Ľ���controller</param>
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
        /// ����ID��ʾ����
        /// </summary>
        /// <param name="screenID">����ID</param>
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
        /// ����ID��ʾ����,���Ͼ�������Բ���
        /// </summary>
        /// <typeparam name="TProps">��������</typeparam>
        /// <param name="screenID">����ID</param>
        /// <param name="properties">���Բ���</param>
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
        /// ����ID���ؽ���
        /// </summary>
        /// <param name="screenID">����ID</param>
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
        /// ����ID�鿴�Ƿ�ע��
        /// </summary>
        /// <param name="screenID">����ID</param>
        /// <returns></returns>
        public bool IsScreenRegistered(string screenID)
        {
            return registeredScreens.ContainsKey(screenID);
        }

        /// <summary>
        /// �������н���
        /// </summary>
        /// <param name="shouldAnimateWhenHiding">���ص�ʱ���Ƿ���Ҫ����</param>
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
