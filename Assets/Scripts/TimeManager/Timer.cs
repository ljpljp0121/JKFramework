using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global
//�ο���ַ��https://github.com/akbiggs/UnityTimer/blob/master/Source/TimerExtensions.cs
public class Timer
{
    #region Public Properties/Fields

    public float Duration { get; }

    public bool IsLooped { get; }

    public bool IsCompleted { get; private set; }

    public bool UsesRealTime { get; } //Real time ����ʱ��̶ȸ��ĵ�Ӱ��

    public bool IsPaused => _timeElapsedBeforePause.HasValue;

    public bool IsCancelled => _timeElapsedBeforeCancel.HasValue;

    public bool IsDone => IsCompleted || IsCancelled || IsOwnerDestroyed;

    #endregion

    #region Public Static Methods

    /// <summary>
    /// ע��һ���µļ�ʱ�����ü�ʱ��Ӧ�ھ���һ��ʱ��󴥷��¼���
    ///
    /// ����������ʱ����ע��ļ�ʱ���������١�
    /// </summary>
    /// <param name="duration">��ʱ��Ӧ����֮ǰ�ȴ���ʱ�䣨����Ϊ��λ����</param>
    /// <param name="onComplete">��ʱ�����ʱ�����Ĳ�����</param>
    /// <param name="onUpdate">ÿ�θ��¼�ʱ��ʱӦ�����Ĳ����������Լ�ʱ���ĵ�ǰѭ����ʼ����������ʱ�䣨����Ϊ��λ����</param>
    /// <param name="isLooped">ִ�к��Ƿ�Ӧ�ظ���ʱ����</param>
    /// <param name="useRealTime">��ʱ����ʹ��ʵʱʱ�䣨��������ͣ����/�춯��Ӱ�죩������Ϸʱ�䣨������ͣ����/�춯��Ӱ�죩��</param>
    /// <param name="autoDestroyOwner">Ҫ���˼�ʱ�����ӵ��Ķ��󡣶������ٺ󣬼�ʱ���������Ҳ�ִ�С�
    /// ����������ⷳ�˵� <see cref=��NullReferenceException��/>s��ͨ����ֹ��ʱ�����в��ڸ���������ٺ�����丸�����</param>
    /// <returns>һ����ʱ���������������ͳ�����ݺ�ֹͣ/�ָ����ȡ�</returns>
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
    /// �رռ�ʱ������ʵ���ϵķ�����ȣ�����������Ҫ�ô��ǣ������ʱ��Ϊ null���򲻻�õ� <see cref=��NullReferenceException��/>��
    /// </summary>
    /// <param name="timer">Ҫ�رյļ�ʱ����</param>
    public static void Cancel(Timer timer)
    {
        timer?.Cancel();
    }

    /// <summary>
    /// ��ͣ��ʱ������ʵ���ϵķ�����ȣ�����������Ҫ�ô��ǣ������ʱ��Ϊ null���򲻻�õ� <see cref=��NullReferenceException��/>��
    /// </summary>
    /// <param name="timer">Ҫ��ͣ�ļ�ʱ����</param>
    public static void Pause(Timer timer)
    {
        timer?.Pause();
    }

    /// <summary>
    /// �ָ���ʱ������ʵ���ϵķ�����ȣ�����������Ҫ�ô��ǣ������ʱ��Ϊ null���򲻻�õ� <see cref=��NullReferenceException��/>��
    /// </summary>
    /// <param name="timer">Ҫ�ָ��ļ�ʱ����</param>
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

        // ��� Manager �����ڣ����ǻ�û���κ�ע��ļ�ʱ�����������������²���Ҫ���κ�����
    }

    public static void PauseAllRegisteredTimers()
    {
        if (Timer._manager != null)
        {
            Timer._manager.PauseAllTimers();
        }

        // ��� Manager �����ڣ����ǻ�û���κ�ע��ļ�ʱ�����������������²���Ҫ���κ�����
    }

    public static void ResumeAllRegisteredTimers()
    {
        if (Timer._manager != null)
        {
            Timer._manager.ResumeAllTimers();
        }

        // ��� Manager �����ڣ����ǻ�û���κ�ע��ļ�ʱ�����������������²���Ҫ���κ�����
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// ֹͣ���ڽ��л�����ͣ�ļ�ʱ������ʱ���� on completion �ص������ᱻ���á�
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
    /// ��ͣ�������еļ�ʱ������ͣ�ļ�ʱ�����Դ���ͣ��ͬһ��ָ���
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
    /// ������ͣ�ļ�ʱ���������ʱ����δ��ͣ����ִ���κβ�����
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
    /// ��ȡ�Դ˼�ʱ���ĵ�ǰ���ڿ�ʼ����������������
    /// </summary>
    /// <returns>T�Դ˼�ʱ���ĵ�ǰ���ڿ�ʼ��������������������� timer ��ѭ���ģ���Ϊ current loop��������ǣ���Ϊ start��
    ///
    /// �����ʱ����������У�����ڳ���ʱ�䡣
    ///
    /// �����ʱ����ȡ��/��ͣ������ڼ�ʱ��������ȡ��/��֮ͣ�侭����������</returns>
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
    /// ��ȡ��ʱ�����ǰʣ���������
    /// </summary>
    /// <returns>��ʱ�����֮ǰҪ��������������ʱ������δ��ͣ��ȡ�������ʱ�ž���ʱ�䡣�����ʱ����ɣ����ֵ�������㡣</returns>
    public float GetTimeRemaining()
    {
        return Duration - GetTimeElapsed();
    }

    /// <summary>
    /// �Ա�����ʽ��ȡ��ʱ���ӿ�ʼ�������Ľ��ȡ�
    /// </summary>
    /// <returns>һ������ 0 �� 1 ֮���ֵ��ָʾ��ʱ���ĳ���ʱ���ѹ�ȥ��</returns>
    public float GetRatioComplete()
    {
        return GetTimeElapsed() / Duration;
    }

    /// <summary>
    /// ��ȡ��ʱ����ʣ�¶��ٽ��ȣ��Ա��ʱ�ʾ����
    /// </summary>
    /// <returns>һ������ 0 �� 1 ֮���ֵ��ָʾ��ʱ���ĳ���ʱ�����ж���ʱ���������</returns>
    public float GetRatioRemaining()
    {
        return GetTimeRemaining() / Duration;
    }

    #endregion

    #region Private Static Properties/Fields

    // �������������ע��ļ�ʱ��
    private static TimerManager _manager;

    #endregion

    #region Private Properties/Fields

    private bool IsOwnerDestroyed => _hasAutoDestroyOwner && _autoDestroyOwner == null;

    private readonly Action _onComplete;
    private readonly Action<float> _onUpdate;
    private float _startTime;
    private float _lastUpdateTime;

    // ���� pausing ����ͣ�������ǽ� Start time ����ʼʱ�䣩 ��ǰ���ѹ�ȥ��ʱ�䡣
    // �������ֻ��鿪ʼʱ���뵱ǰ����ʱ�䣬�⽫�������Ǳ�ȡ������ͣʱ������ʱ�䣬���������Ҫ��������ͣ/ȡ��֮ǰ������ʱ��
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
    /// ������������������е� Ӧ�ó����С�<see cref="Timer"/>
    /// �⽫������һ�δ�����ʱ��ʱʵ���� -- �������ֶ�������ӵ������С�
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

        // ����ÿ֡��������ע��ļ�ʱ��
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
    /// ����ʱ�����ӵ���Ϊ�ϡ������Ϊ�ڼ�ʱ�����֮ǰ�����٣�
    /// ����ͨ���������ģ��򲻻�ִ�� timer �ص���
    /// </summary>
    /// <param name="behaviour">Ҫ���˼�ʱ�����ӵ�����Ϊ��</param>
    /// <param name="duration">��ʱ������֮ǰ�ȴ��ĳ���ʱ�䡣</param>
    /// <param name="onComplete">��ʱ������ʱҪ���еĲ�����</param>
    /// <param name="onUpdate">��ʱ����ÿִ֡�еĺ����������Ե�ǰ���ڿ�ʼ����������������</param>
    /// <param name="isLooped">��ʱ��ִ�к��Ƿ�Ӧ��������</param>
    /// <param name="useRealTime">��ʱ����ʹ��ʵʱ����������������ͣӰ�죩����
    /// game-time����ʱ������仯Ӱ�죩��</param>
    public static Timer AttachTimer(this MonoBehaviour behaviour, float duration, Action onComplete,
        Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
    {
        return Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
    }
}