﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework.ViewAnimation
{
    //public class AniComponent
    //{
        
    //}

    /// <summary>
    /// 界面动画组件
    /// </summary>
    public abstract class AniComponent : MonoBehaviour
    {
        /// <summary>
        /// 动画播放，当动画执行完调用CallWhenFinished
        /// </summary>
        public abstract void Animate(Transform target, Action callWhenFinished);
    }
}
