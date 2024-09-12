using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework.Core;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板控制器
    /// </summary>
    public abstract class PanelController : APanelController<PanelProperties>
    {

    }

    /// <summary>
    /// 面板控制器基类
    /// </summary>
    public abstract class APanelController<T> : UIScreenController<T>, IPanelController where T : IPanelProperties
    {
        public PanelPriority Proiority
        {
            get
            {
                if (Properties != null)
                {
                    return Properties.Priority;
                }
                else
                {
                    return PanelPriority.None;
                }
            }
        }

        protected sealed override void SetProperties(T props)
        {
            base.SetProperties(props);
        }
    }
}
