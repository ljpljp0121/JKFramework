using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global
//参考地址：https://github.com/akbiggs/UnityTimer/blob/master/Source/TimerExtensions.cs
public class Timer
{
    #region Public Properties/Fields

    public float Duration { get; }

    public bool IsLooped { get; }

    public bool IsCompleted { get; private set; }

    public bool UsesRealTime { get; } //Real time 不受时间刻度更改的影响

    public bool IsPaused => _timeElapsedBeforePause.HasValue;

    public bool IsCancelled => _timeElapsedBeforeCancel.HasValue;

    public bool IsDone => IsCompleted || IsCancelled || IsOwnerDestroyed;

    #endregion

    #region Public Static Methods

    /// <summary>
    /// 注册一个新的计时器，该计时器应在经过一定时间后触发事件。
    ///
    /// 当场景更改时，已注册的计时器将被销毁。
    /// </summary>
    /// <param name="duration">计时器应触发之前等待的时间（以秒为单位）。</param>
    /// <param name="onComplete">计时器完成时触发的操作。</param>
    /// <param name="onUpdate">每次更新计时器时应触发的操作。采用自计时器的当前循环开始以来经过的时间（以秒为单位）。</param>
    /// <param name="isLooped">执行后是否应重复计时器。</param>
    /// <param name="useRealTime">计时器是使用实时时间（即不受暂停、慢/快动作影响）还是游戏时间（将受暂停和慢/快动作影响）。</param>
    /// <param name="autoDestroyOwner">要将此计时器附加到的对象。对象销毁后，计时器将过期且不执行。
    /// 这允许你避免烦人的 <see cref=“NullReferenceException”/>s，通过阻止计时器运行并在父组件被销毁后访问其父组件。</param>
    /// <returns>一个计时器对象，允许您检查统计数据和停止/恢复进度。</returns>
    public static Timer Register(float duration, Action onComplete, Action<float> onUpdate = null,
        bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
    {
        if (_manager == null)
        {
            var managerInScene = Object.FindObjectOfType<TimerManager>();
            if (managerInScene != null)
            {
                _manager = managerInScene;
            }
            else
            {
                var managerObject = new GameObject { name = "TimerManager" };
                _manager = managerObject.AddComponent<TimerManager>();
            }
        }

        var timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
        _manager.RegisterTimer(timer);
        return timer;
    }

    public static Timer Register(float duration, bool isLooped, bool useRealTime, Action onComplete)
    {
        return Register(duration, onComplete, null, isLooped, useRealTime);
    }

    /// <summary>
    /// 关闭计时器。与实例上的方法相比，这样做的主要好处是，如果计时器为 null，则不会得到 <see cref=“NullReferenceException”/>。
    /// </summary>
    /// <param name="timer">要关闭的计时器。</param>
    public static void Cancel(Timer timer)
    {
        timer?.Cancel();
    }

    /// <summary>
    /// 暂停计时器。与实例上的方法相比，这样做的主要好处是，如果计时器为 null，则不会得到 <see cref=“NullReferenceException”/>。
    /// </summary>
    /// <param name="timer">要暂停的计时器。</param>
    public static void Pause(Timer timer)
    {
        timer?.Pause();
    }

    /// <summary>
    /// 恢复计时器。与实例上的方法相比，这样做的主要好处是，如果计时器为 null，则不会得到 <see cref=“NullReferenceException”/>。
    /// </summary>
    /// <param name="timer">要恢复的计时器。</param>
    public static void Resume(Timer timer)
    {
        if (timer != null)
        {
            timer.Resume();
        }
    }

    public static void CancelAllRegisteredTimers()
    {
        if (Timer._manager != null)
        {
            Timer._manager.CancelAllTimers();
        }

        // 如果 Manager 不存在，我们还没有任何注册的计时器，因此在这种情况下不需要做任何事情
    }

    public static void PauseAllRegisteredTimers()
    {
        if (Timer._manager != null)
        {
            Timer._manager.PauseAllTimers();
        }

        // 如果 Manager 不存在，我们还没有任何注册的计时器，因此在这种情况下不需要做任何事情
    }

    public static void ResumeAllRegisteredTimers()
    {
        if (Timer._manager != null)
        {
            Timer._manager.ResumeAllTimers();
        }

        // 如果 Manager 不存在，我们还没有任何注册的计时器，因此在这种情况下不需要做任何事情
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 停止正在进行或已暂停的计时器。计时器的 on completion 回调将不会被调用。
    /// </summary>
    public void Cancel()
    {
        if (IsDone)
        {
            return;
        }

        _timeElapsedBeforeCancel = GetTimeElapsed();
        _timeElapsedBeforePause = null;
    }

    /// <summary>
    /// 暂停正在运行的计时器。暂停的计时器可以从暂停的同一点恢复。
    /// </summary>
    public void Pause()
    {
        if (IsPaused || IsDone)
        {
            return;
        }

        _timeElapsedBeforePause = GetTimeElapsed();
    }

    /// <summary>
    /// 继续暂停的计时器。如果计时器尚未暂停，则不执行任何操作。
    /// </summary>
    public void Resume()
    {
        if (!IsPaused || IsDone)
        {
            return;
        }

        _timeElapsedBeforePause = null;
    }

    /// <summary>
    /// 获取自此计时器的当前周期开始以来经过的秒数。
    /// </summary>
    /// <returns>T自此计时器的当前周期开始以来经过的秒数，即如果 timer 是循环的，则为 current loop，如果不是，则为 start。
    ///
    /// 如果计时器已完成运行，则等于持续时间。
    ///
    /// 如果计时器被取消/暂停，则等于计时器启动和取消/暂停之间经过的秒数。</returns>
    public float GetTimeElapsed()
    {
        if (IsCompleted || GetWorldTime() >= GetFireTime())
        {
            return Duration;
        }

        return _timeElapsedBeforeCancel ??
               _timeElapsedBeforePause ??
               GetWorldTime() - _startTime;
    }

    /// <summary>
    /// 获取计时器完成前剩余的秒数。
    /// </summary>
    /// <returns>计时器完成之前要经过的秒数。计时器仅在未暂停、取消或完成时才经过时间。如果计时器完成，则此值将等于零。</returns>
    public float GetTimeRemaining()
    {
        return Duration - GetTimeElapsed();
    }

    /// <summary>
    /// 以比率形式获取计时器从开始到结束的进度。
    /// </summary>
    /// <returns>一个介于 0 到 1 之间的值，指示计时器的持续时间已过去。</returns>
    public float GetRatioComplete()
    {
        return GetTimeElapsed() / Duration;
    }

    /// <summary>
    /// 获取计时器还剩下多少进度（以比率表示）。
    /// </summary>
    /// <returns>一个介于 0 到 1 之间的值，指示计时器的持续时间仍有多少时间待经过。</returns>
    public float GetRatioRemaining()
    {
        return GetTimeRemaining() / Duration;
    }

    #endregion

    #region Private Static Properties/Fields

    // 负责更新所有已注册的计时器
    private static TimerManager _manager;

    #endregion

    #region Private Properties/Fields

    private bool IsOwnerDestroyed => _hasAutoDestroyOwner && _autoDestroyOwner == null;

    private readonly Action _onComplete;
    private readonly Action<float> _onUpdate;
    private float _startTime;
    private float _lastUpdateTime;

    // 对于 pausing （暂停），我们将 Start time （开始时间） 向前推已过去的时间。
    // 如果我们只检查开始时间与当前世界时间，这将扰乱我们被取消或暂停时经过的时间，因此我们需要缓存在暂停/取消之前经过的时间
    private float? _timeElapsedBeforeCancel;
    private float? _timeElapsedBeforePause;

    // after the auto destroy owner is destroyed, the timer will expire
    // this way you don't run into any annoying bugs with timers running and accessing objects
    // after they have been destroyed
    private readonly MonoBehaviour _autoDestroyOwner;
    private readonly bool _hasAutoDestroyOwner;

    #endregion

    #region Private Constructor (use static Register method to create new timer)

    private Timer(float duration, Action onComplete, Action<float> onUpdate,
        bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
    {
        Duration = duration;
        _onComplete = onComplete;
        _onUpdate = onUpdate;

        IsLooped = isLooped;
        UsesRealTime = usesRealTime;

        _autoDestroyOwner = autoDestroyOwner;
        _hasAutoDestroyOwner = autoDestroyOwner != null;

        _startTime = GetWorldTime();
        _lastUpdateTime = _startTime;
    }

    #endregion

    #region Private Methods

    private float GetWorldTime()
    {
        return UsesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetFireTime()
    {
        return _startTime + Duration;
    }

    private float GetTimeDelta()
    {
        return GetWorldTime() - _lastUpdateTime;
    }

    private void Update()
    {
        if (IsDone)
        {
            return;
        }

        if (IsPaused)
        {
            _startTime += GetTimeDelta();
            _lastUpdateTime = GetWorldTime();
            return;
        }

        _lastUpdateTime = GetWorldTime();

        if (_onUpdate != null)
        {
            _onUpdate(GetTimeElapsed());
        }

        if (GetWorldTime() >= GetFireTime())
        {

            if (_onComplete != null)
            {
                _onComplete();
            }

            if (IsLooped)
            {
                _startTime = GetWorldTime();
            }
            else
            {
                IsCompleted = true;
            }
        }
    }

    #endregion

    #region Manager Class (implementation detail, spawned automatically and updates all registered timers)

    /// <summary>
    /// 管理更新所有正在运行的 应用程序中。<see cref="Timer"/>
    /// 这将在您第一次创建计时器时实例化 -- 您无需手动将其添加到场景中。
    /// </summary>
    private class TimerManager : MonoBehaviour
    {
        private List<Timer> _timers = new List<Timer>();

        // buffer adding timers so we don't edit a collection during iteration
        private List<Timer> _timersToAdd = new List<Timer>();

        public void RegisterTimer(Timer timer)
        {
            _timersToAdd.Add(timer);
        }

        public void CancelAllTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Cancel();
            }

            _timers = new List<Timer>();
            _timersToAdd = new List<Timer>();
        }

        public void PauseAllTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Resume();
            }
        }

        // 更新每帧上所有已注册的计时器
        [UsedImplicitly]
        private void Update()
        {
            UpdateAllTimers();
        }

        private void UpdateAllTimers()
        {
            if (_timersToAdd.Count > 0)
            {
                _timers.AddRange(_timersToAdd);
                _timersToAdd.Clear();
            }

            foreach (var timer in _timers)
            {
                timer.Update();
            }

            _timers.RemoveAll(t => t.IsDone);
        }
    }

    #endregion

}

public static class TimerExtensions
{
    /// <summary>
    /// 将计时器附加到行为上。如果行为在计时器完成之前被销毁，
    /// 例如通过场景更改，则不会执行 timer 回调。
    /// </summary>
    /// <param name="behaviour">要将此计时器附加到的行为。</param>
    /// <param name="duration">计时器触发之前等待的持续时间。</param>
    /// <param name="onComplete">计时器出发时要运行的操作。</param>
    /// <param name="onUpdate">计时器内每帧执行的函数，采用自当前周期开始以来经过的秒数。</param>
    /// <param name="isLooped">定时器执行后是否应该重启。</param>
    /// <param name="useRealTime">计时器是使用实时（不受慢动作或暂停影响）还是
    /// game-time（受时间比例变化影响）。</param>
    public static Timer AttachTimer(this MonoBehaviour behaviour, float duration, Action onComplete,
        Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
    {
        return Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
    }
}