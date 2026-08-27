using System;
using UnityEngine;

namespace ArTiX.Utils
{
    public class Timer : MonoBehaviour
    {
        private static readonly Pool<Timer> pool;

        static Timer()
        {
            pool = new Pool<Timer>(CreateTimer, 5);
        }
        // Only used to populate the pool.
        private static Timer CreateTimer()
        {
            return new GameObject(nameof(Timer), typeof(Timer)).GetComponent<Timer>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="startOnCreate"></param>
        /// <param name="autodestroy"></param>
        /// <param name="scaled"></param>
        /// <param name="repetitionNumber"></param>
        /// <param name="onTimerActiveEvent">Called every frame while the timer is active.</param>
        /// <param name="onFinishedEvent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Timer Create(MonoBehaviour target, float duration, bool startOnCreate = false, bool autodestroy = false, bool scaled = true, int repetitionNumber = 0, in bool infinite = false, Action onTimerActiveEvent = null, Action onFinishedEvent = null, string name = "Timer")
        {
            Timer timer = pool.GetPooledObject();

            if (timer == null) timer = new GameObject().AddComponent(typeof(Timer)) as Timer;

            timer.gameObject.name = name;
            timer.duration = duration;
            timer.scaled = scaled;
            timer.autodestroy = autodestroy;
            timer.infinite = infinite;
            timer.repetitionNumber = timer.currentRepetitionNumber = repetitionNumber;
            timer.OnActiveEvent = onTimerActiveEvent;
            timer.OnFinishedEvent = onFinishedEvent;
            timer.IsActive = startOnCreate;
            timer.target = target;

            return timer;
        }

        public bool IsActive { get; private set; }
        public float ElapsedTime { get; private set; }
        public float ElapsedTimeAsPercentage => ElapsedTime / duration;
        public float RemainingTime => duration - ElapsedTime;

        public float duration;

        private MonoBehaviour target;

        private bool scaled;
        private bool autodestroy;
        private bool infinite;
        private int repetitionNumber = 0;

        private int currentRepetitionNumber = 0;

        /// <summary>
        /// Called every frame while the timer is active.
        /// </summary>
        public event Action OnActiveEvent;
        public event Action OnFinishedEvent;

        private Timer() { }

        private void Update()
        {
            if (target == null) 
            {
                Kill();
                return;
            }

            if (IsActive && currentRepetitionNumber >= 0)
            {
                ElapsedTime += scaled ? Time.deltaTime : Time.unscaledDeltaTime;
                
                OnActiveEvent?.Invoke();

                if (ElapsedTime > duration)
                {
                    if (!infinite) currentRepetitionNumber--;

                    ElapsedTime = 0;

                    if (currentRepetitionNumber == -1)
                    {
                        if (autodestroy) Kill();
                        else Stop();
                    }

                    OnFinishedEvent?.Invoke();
                }
            }
        }

        public void Play()
        {
            Stop();
            IsActive = true;
        }

        public void Stop()
        {
            currentRepetitionNumber = repetitionNumber;
            IsActive = false;
            ElapsedTime = 0;
        }

        public void Kill()
        {
            Stop();
            pool.ReturnToPool(this);
        }
    }
}