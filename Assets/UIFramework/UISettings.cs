﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI的模板
    /// </summary>
    [CreateAssetMenu(fileName = "UISettings", menuName = "UI/UI Settings")]
    public class UISettings : ScriptableObject
    {
        [Tooltip("UI Frame 的预制体")]
        [SerializeField] private UIFrame templateUIPrefab = null;
        [Tooltip("界面的预制体(包括面板和窗口)")]
        [SerializeField] private List<GameObject> screensToRegister = null;
        [Tooltip("实例化是否停用")]
        [SerializeField] private bool deactivateScreenGOs = true;

        /// <summary>
        /// 创建一个UI Frame对象
        /// </summary>
        public UIFrame CreateUIInstance(bool instanceAndRegisterScreens = true)
        {
            var newUI = Instantiate(templateUIPrefab);

            if (instanceAndRegisterScreens)
            {
                foreach (var screen in screensToRegister)
                {
                    var screenInstance = Instantiate(screen);
                    var screenController = screenInstance.GetComponent<IScreenController>();

                    if (screenController != null)
                    {
                        newUI.RegisterScreen(screen.name, screenController, screenInstance.transform);
                        if (deactivateScreenGOs && screenInstance.activeSelf)
                        {
                            screenInstance.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogError("Screen does not contain a ScreenController! Skipping" + screen.name);
                    }
                }
            }

            return newUI;
        }

        private void OnValidate()
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            for (int i = 0; i < screensToRegister.Count; i++)
            {
                var screenCtl = screensToRegister[i].GetComponent<IScreenController>();
                if (screenCtl == null)
                {
                    objectsToRemove.Add(screensToRegister[i]);
                }
            }

            if (objectsToRemove.Count > 0)
            {
                Debug.LogError("Some GameObjects that were added to Screen Prefab List did not have");
                foreach (var obj in objectsToRemove)
                {
                    Debug.LogError($"Removed {obj.name} from {name} as it has no Screen Controller");
                    screensToRegister.Remove(obj);
                }
            }
        }
    }
}
