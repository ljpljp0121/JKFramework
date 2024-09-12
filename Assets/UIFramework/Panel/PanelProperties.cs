﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板属性类
    /// </summary>
    public class PanelProperties : IPanelProperties
    {
        [SerializeField]
        [Tooltip("面板根据其优先级进入不同的副层级，可以在'面板层级'中设置副层级。")]
        private PanelPriority priority;

        public PanelPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
