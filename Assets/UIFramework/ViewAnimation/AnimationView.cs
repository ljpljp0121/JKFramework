﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework.ViewAnimation
{
    /// <summary>
    /// 具体动画实现，自己封装DoTween之类的都行
    /// </summary>
    public class AnimationView : AniComponent
    {
        [SerializeField]
        private AnimationClip clip = null;
        [SerializeField]
        private bool playReverse = false;

        private Action previousCallbackWhenFinished;

        public override void Animate(Transform target, Action callWhenFinished)
        {
            FinishPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null)
            {
                Debug.LogError("No Animation component in" + target);
                if (callWhenFinished != null)
                {
                    callWhenFinished();
                }

                return;
            }

            targetAnimation.clip = clip; 
            StartCoroutine(PlayAnimationRoutine(targetAnimation, callWhenFinished));
        }

        private IEnumerator PlayAnimationRoutine(Animation targetAnimation, Action callWhenFinished)
        {
            previousCallbackWhenFinished = callWhenFinished;
            foreach (AnimationState state in targetAnimation)
            {
                state.time = playReverse ? state.clip.length : 0f;
                state.speed = playReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            FinishPrevious();
        }

        private void FinishPrevious()
        {
            if (previousCallbackWhenFinished != null)
            {
                previousCallbackWhenFinished();
                previousCallbackWhenFinished = null;
            }

            StopAllCoroutines();
        }
    }
}
