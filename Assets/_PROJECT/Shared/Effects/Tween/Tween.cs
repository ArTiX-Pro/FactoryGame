using ArTiX.Utils.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public enum ETransformProperty
    {
        Position,
        Rotation,
        Scale
    }

    public class Tween : MonoBehaviour, IPooledObject<Tween>
    {
        #region Structs

        [Serializable]
        public struct TweeningParams<T>
        {
            public T startValue;
            public T targetValue;
            public AnimParams animParams;
            public float delay;
            public bool parrallel;
            public bool timeScaled;
            public Action onFinishedStepEvent;

            public TweeningParams(T startValue, T targetValue, AnimParams animParams, 
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
            {
                this.startValue = startValue;
                this.targetValue = targetValue;
                this.animParams = animParams;
                this.delay = delay;
                this.parrallel = parrallel;
                this.timeScaled = timeScaled;
                this.onFinishedStepEvent = onFinishedStepEvent;
            }
        }

        [Serializable]
        public struct AnimParams
        {
            [SerializeField] private float duration;
            [SerializeField] private ETransition transition;

            public readonly float Duration => duration;
            public readonly ETransition Transition => transition;

            public AnimParams(float duration, ETransition transition)
            {
                this.duration = duration;
                this.transition = transition;
            }
        }

        [Serializable]
        public struct InOutAnimParams
        {
            public AnimParams inAnim;
            public float holdDuration;
            public AnimParams outAnim;
        }

        public enum ETransition
        {
            Linear,
            SmoothStart2,
            SmoothStart3,
            SmoothStart4,
            SmoothStart5,
            SmoothStop2,
            SmoothStop3,
            SmoothStop4,
            SmoothStop5,
            SmoothStep3,
            SmoothStep4,
            SmoothStep5,
            Arch2,
            SmoothStartArch3,
            SmoothStopArch3,
            SmoothStartExpo,
            SmoothStopExpo,
            SmoothStepExpo,
            StartBack,
            StopBack,
            StepBack,
            StartElastic,
            StopElastic,
            StepElastic,
            StartBounce,
            StopBounce,
            StepBounce
        }

        #endregion

        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Tween Create(in string name = "Tween")
        {
            Tween tween = TweenPool.Instance.Pool.GetPooledObject();
            if (tween == null) tween = new GameObject().AddComponent(typeof(Tween)) as Tween;
            tween.name = name;

            return tween;
        }

        #region Interpolation Methods

        // FLOATS
        public static float InterpolateValue(float startValue, float targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Mathf.LerpUnclamped(startValue, targetValue, result);
        }

        public static float InterpolateValue(float startValue, float targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static float InterpolateValue(float startValue, float targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        // VECTOR 2
        public static Vector2 InterpolateValue(Vector2 startValue, Vector2 targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Vector2.LerpUnclamped(startValue, targetValue, result);
        }

        public static Vector2 InterpolateValue(Vector2 startValue, Vector2 targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Vector2 InterpolateValue(Vector2 startValue, Vector2 targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        // VECTOR 3
        public static Vector3 InterpolateValue(Vector3 startValue, Vector3 targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Vector3.LerpUnclamped(startValue, targetValue, result);
        }

        public static Vector3 InterpolateValue(Vector3 startValue, Vector3 targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Vector3 InterpolateValue(Vector3 startValue, Vector3 targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        // QUATERNIONS
        public static Quaternion InterpolateValue(Quaternion startValue, Quaternion targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Quaternion.LerpUnclamped(startValue, targetValue, result);
        }

        public static Quaternion InterpolateValue(Quaternion startValue, Quaternion targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Quaternion InterpolateValue(Quaternion startValue, Quaternion targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        #region COLORS
        public static Color InterpolateValue(Color startValue, Color targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Color.LerpUnclamped(startValue, targetValue, result);
        }

        public static Color InterpolateValue(Color startValue, Color targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Color InterpolateValue(Color startValue, Color targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="easing"></param>
        /// <param name="transition"></param>
        /// <returns>A value between 0 & 1 which corresponds to the given equation</returns>
        private static float GetValue(float t, ETransition transition)
        {
            const float C1 = 1.70158f;
            const float C2 = C1 * 1.525f;
            const float C3 = C1 + 1;
            const float C4 = 2 * Mathf.PI / 3;
            const float C5 = 2 * Mathf.PI / 4.5f;
            const float N1 = 7.5625f;
            const float D1 = 2.75f;

            switch (transition)
            {
                case ETransition.Linear:
                    return t;

                case ETransition.SmoothStart2:
                    return t * t;

                case ETransition.SmoothStart3:
                    return t * t * t;

                case ETransition.SmoothStart4:
                    return t * t * t * t;

                case ETransition.SmoothStart5:
                    return t * t * t * t * t;

                case ETransition.SmoothStop2:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart2);

                case ETransition.SmoothStop3:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart3);

                case ETransition.SmoothStop4:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart4);

                case ETransition.SmoothStop5:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart5);

                case ETransition.SmoothStep3:
                    return Mathf.Lerp(GetValue(t, ETransition.SmoothStart2), GetValue(t, ETransition.SmoothStop2), t);

                case ETransition.SmoothStep4:
                    return Mathf.Lerp(GetValue(t, ETransition.SmoothStart3), GetValue(t, ETransition.SmoothStop3), t);

                case ETransition.SmoothStep5:
                    return Mathf.Lerp(GetValue(t, ETransition.SmoothStart4), GetValue(t, ETransition.SmoothStop4), t);

                case ETransition.Arch2:
                    return t * (1 - t) * 4;

                case ETransition.SmoothStartArch3:
                    return t * (1 - t) * t / (2f / 3f * (1 - (2f / 3f)) * t);

                case ETransition.SmoothStopArch3:
                    return t * GetValue(1 - t, ETransition.SmoothStart2) / (1f / 3f * GetValue(1 - (1f / 3f), ETransition.SmoothStart2));

                case ETransition.SmoothStartExpo:
                    return Mathf.Pow(2, (10 * t) - 10);

                case ETransition.SmoothStopExpo:
                    return 1 - Mathf.Pow(2, -10 * t);

                case ETransition.SmoothStepExpo:
                    return 0.5f * (t < 0.5f ? Mathf.Pow(2, (20 * t) - 10)
                            : (2 - Mathf.Pow(2, (-20 * t) + 10)));

                case ETransition.StartBack:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return (C3 * t * t * t) - (C1 * t * t);

                case ETransition.StopBack:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return 1 + (C3 * (t - 1) * (t - 1) * (t - 1)) + (C1 * (t - 1) * (t - 1));

                case ETransition.StepBack:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return t < 0.5f
                            ? 4 * t * t * (((C2 + 1) * 2 * t) - C2) * 0.5f
                            : ((Mathf.Pow((2 * t) - 2, 2) * (((C2 + 1) * ((t * 2) - 2)) + C2)) + 2) * 0.5f;

                case ETransition.StartElastic:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return -Mathf.Pow(2, (10 * t) - 10) * Mathf.Sin(((t * 10) - 10.75f) * C4);

                case ETransition.StopElastic:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return (Mathf.Pow(2, -10 * t) * Mathf.Sin(((t * 10) - 0.75f) * C4)) + 1;

                case ETransition.StepElastic:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return t < 0.5f
                            ? -(Mathf.Pow(2, (20 * t) - 10) * Mathf.Sin(((20 * t) - 11.125f) * C5)) * 0.5f
                            : (Mathf.Pow(2, (-20 * t) + 10) * Mathf.Sin(((20 * t) - 11.125f) * C5) * 0.5f) + 1;

                case ETransition.StartBounce:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return 1 - GetValue(1 - t, ETransition.StopBounce);

                case ETransition.StopBounce:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    if (t < 1 / D1) return N1 * t * t;
                    else if (t < 2 / D1) return (N1 * (t -= 1.5f / D1) * t) + 0.75f;
                    else if (t < 2.5 / D1) return (N1 * (t -= 2.25f / D1) * t) + 0.9375f;
                    else return (N1 * (t -= 2.625f / D1) * t) + 0.984375f;

                case ETransition.StepBounce:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return 0.5f * (t < 0.5f
                            ? (1 - GetValue(1 - (2 * t), ETransition.StopBounce))
                            : (1 + GetValue((2 * t) - 1, ETransition.StopBounce)));
            }

            return t;
        }

        #endregion

        #endregion

        #region Instance

        private Tween() { }

        #region Variables

        private readonly List<List<Tweener>> tweenersLists = new List<List<Tweener>>();

        public event Action OnFinishedEvent;

        public bool IsActive { get; private set; }
        public Pool<Tween> Pool { get; set; }

        #endregion

        private void Update()
        {
            if (IsActive)
            {
                if (tweenersLists.Count > 0)
                {
                    List<Tweener> currentTweeners = tweenersLists[0];

                    for (int i = currentTweeners.Count - 1; i >= 0; i--)
                    {
                        if (currentTweeners[i].UpdateTweener() && currentTweeners.Count > i)
                            currentTweeners.RemoveAt(i);
                    }

                    if (currentTweeners.Count == 0) tweenersLists.RemoveAt(0);
                }
                else
                {
                    if (OnFinishedEvent?.Target != null)
                        OnFinishedEvent.Invoke();

                    Kill();
                }
            }
        }

        #region Add Property

        public void TweenProperty(Transform transform, ETransformProperty property, Vector3 startValue, Vector3 targetValue, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new Vector3Tweener(this, transform, property, startValue, targetValue, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        public void TweenProperty(Transform transform, Quaternion startRota, Quaternion targetRota, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new QuaternionTweener(this, transform, startRota, targetRota, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        public void TweenProperty(Material material, in string property, float startValue, float targetValue, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new FloatTweener(this, material, property, startValue, targetValue, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        public void TweenProperty(Material material, in string property, Color startColor, Color targetColor, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new ColorTweener(this, material, property, startColor, targetColor, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        public void TweenProperty(CanvasGroup group, float startValue, float targetValue, AnimParams animParams, float delay, bool parrallel, bool timeScaled, Action onFinishedStepEvent)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new FloatTweener(this, group, startValue, targetValue, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        #endregion

        #region Add Event

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        public void TweenEvent(float startValue, float targetValue, Action<float> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new FloatTweener(this, startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        public void TweenEvent(Vector2 startValue, Vector2 targetValue, Action<Vector2> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new Vector2Tweener(this, startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        public void TweenEvent(Vector3 startValue, Vector3 targetValue, Action<Vector3> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new Vector3Tweener(this, startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        public void TweenEvent(Quaternion startValue, Quaternion targetValue, Action<Quaternion> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new QuaternionTweener(this, startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        private void SetupTweener(out List<Tweener> tweenerList, ref float delay, bool parrallel = false)
        {
            IsActive = true;

            if (!parrallel || tweenersLists.Count == 0)
            {
                tweenerList = new List<Tweener>();
                tweenersLists.Add(tweenerList);
            }
            else
                tweenerList = tweenersLists[tweenersLists.Count - 1];

            if (parrallel && tweenerList.Count >= 1)
                delay += tweenerList[0].Delay;
        }

        #endregion

        public void Remove(Tweener tweener)
        {
            int nbTweenerList = tweenersLists.Count;
            for (int i = 0; i < nbTweenerList; i++)
            {
                if (tweenersLists[i].Contains(tweener))
                {
                    tweenersLists[i].Remove(tweener);
                    return;
                }
            }
        }

        public void Kill()
        {
            if (!IsActive) return;

            OnFinishedEvent = null;
            tweenersLists.Clear();
            IsActive = false;
            Release();
        }

        private void OnDisable()
        {
            IsActive = false;
        }

        public void Release() => TweenPool.Instance.Pool.ReturnToPool(this);

        #endregion
    }
}