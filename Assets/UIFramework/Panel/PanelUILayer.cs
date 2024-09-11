using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 这个Layer层是控制面板的
    /// 面板是界面的一种，没有历史记录，没有队列
    /// 就是简单的显示在界面中
    /// 比如说体力槽，小地图这种常驻的
    /// </summary>
    public class PanelUILayer : UILayer<IPanelController>
    {
        [SerializeField]
        [Tooltip("优先级并行层的设置,注册到此层的面板将根据其优先级重新归属到不同的并行层对象")]
        private PanelPriorityLayerList priorityLayers = null;


        public override void ShowScreen(IPanelController screen)
        {
            screen.Show();
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps properties)
        {
            screen.Show(properties);
        }

        public override void HideScreen(IPanelController screen)
        {
            screen.Hide();
        }

        public bool IsPanelVisible(string panelID)
        {
            IPanelController panel;
            if (registeredScreens.TryGetValue(panelID, out panel))
            {
                return panel.IsVisible;
            }

            return false;
        }

        public override void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            var ctl = controller as IPanelController;
            if (ctl != null)
            {
                ReparentToParaLayer(ctl.Proiority, screenTransform);
            }
            else
            {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        private void ReparentToParaLayer(PanelPriority proiority, Transform screenTransform)
        {
            Transform trans;
            if (!priorityLayers.ParaLayerLookup.TryGetValue(proiority, out trans))
            {
                trans = transform;
            }

            screenTransform.SetParent(trans, false);
        }
    }

}
