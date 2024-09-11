using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// ���Layer���ǿ�������
    /// ����ǽ����һ�֣�û����ʷ��¼��û�ж���
    /// ���Ǽ򵥵���ʾ�ڽ�����
    /// ����˵�����ۣ�С��ͼ���ֳ�פ��
    /// </summary>
    public class PanelUILayer : UILayer<IPanelController>
    {
        [SerializeField]
        [Tooltip("���ȼ����в������,ע�ᵽ�˲����彫���������ȼ����¹�������ͬ�Ĳ��в����")]
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
