using System;
using UIFramework.Panel;
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
            graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();

        }
    }
}
