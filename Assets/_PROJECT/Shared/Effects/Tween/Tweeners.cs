using System;
using UnityEngine;
using static ArTiX.Effects.Tween.Tween;

namespace ArTiX.Effects.Tween
{
    // ABSTRACTS
    public abstract class Tweener
    {
        protected readonly AnimParams animParams;
        private readonly bool scaled;

        public float Delay { get; private set; }
        protected float timer;

        public event Action OnFinishedStepEvent;

        private MonoBehaviour target;

        protected Tweener(in MonoBehaviour target, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent)
        {
            this.animParams = animParams;
            this.scaled = scaled;
            Delay = delay;
            OnFinishedStepEvent = onFinishedStepEvent;

            timer = 0;
            this.target = target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when finished, false otherwise.</returns>
        public bool UpdateTweener()
        {
            if (target == null) 
            {
                OnFinishedStepEvent = null;
                return true; 
            }

            if (Delay > 0)
            {
                Delay -= scaled ? Time.deltaTime : Time.deltaTime / Time.timeScale;
                return false;
            }

            timer += scaled ? Time.deltaTime : Time.deltaTime / Time.timeScale;

            InvokeEvent();

            if (timer >= animParams.Duration)
            {
                OnFinishedStepEvent?.Invoke();
                return true;
            }

            return false;
        }

        protected abstract void InvokeEvent();
    }

    public abstract class EventTweener<T> : Tweener
    {
        protected readonly T startValue;
        protected readonly T targetValue;

        private event Action<T> tweenEvent;

        private Tween masterTween;

        protected EventTweener(MonoBehaviour target, Tween masterTween, T startValue, T targetValue, Action<T> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, animParams, delay, scaled, onFinishedStepEvent)
        {
            this.startValue = startValue;
            this.targetValue = targetValue;
            this.tweenEvent = tweenEvent;
            this.masterTween = masterTween;
        }

        protected EventTweener(MonoBehaviour target, Tween masterTween, TweeningParams<T> tweenParams, Action<T> tweenEvent) :
            base(target, tweenParams.animParams, tweenParams.delay, tweenParams.timeScaled, tweenParams.onFinishedStepEvent)
        {
            startValue = tweenParams.startValue;
            targetValue = tweenParams.targetValue;
            this.tweenEvent = tweenEvent;
            this.masterTween = masterTween;
        }

        protected override void InvokeEvent()
        {
            tweenEvent.Invoke(InterpolateValue());
        }

        protected abstract T InterpolateValue();
    }

    // INSTANTIABLE
    public class FloatTweener : EventTweener<float>
    {
        private Material mat;
        private CanvasGroup group;
        private string property;

        public FloatTweener(MonoBehaviour target, Tween masterTween, float startValue, float targetValue, Action<float> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
        {
        }

        public FloatTweener(MonoBehaviour target, Tween masterTween, Material material, in string property, float startValue, float targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
        {
            mat = material;
            this.property = property;
        }

        public FloatTweener(MonoBehaviour target, Tween masterTween, CanvasGroup group, float startValue, float targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, tweenEvent: null, animParams, delay, scaled, onFinishedStepEvent)
        {
            this.group = group;
        }

        protected override void InvokeEvent()
        {
            if (mat != null)
                mat.SetFloat(property, InterpolateValue());
            else if (group != null)
                group.alpha = InterpolateValue();
            else
                base.InvokeEvent();
        }

        protected override float InterpolateValue()
        {
            return Tween.InterpolateValue(startValue, targetValue, timer, animParams);
        }
    }

    public class Vector2Tweener : EventTweener<Vector2>
    {
        public Vector2Tweener(MonoBehaviour target, Tween masterTween, Vector2 startValue, Vector2 targetValue, Action<Vector2> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
        {
        }

        protected override Vector2 InterpolateValue()
        {
            return Tween.InterpolateValue(startValue, targetValue, timer, animParams);
        }
    }

    public class Vector3Tweener : EventTweener<Vector3>
    {
        private Transform tweenedTransform;
        private ETransformProperty property;

        public Vector3Tweener(MonoBehaviour target, Tween masterTween, Vector3 startValue, Vector3 targetValue, Action<Vector3> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
        {
        }

        public Vector3Tweener(MonoBehaviour target, Tween masterTween, Transform transform, ETransformProperty property, Vector3 startValue, Vector3 targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
        {
            tweenedTransform = transform;
            this.property = property;
        }

        protected override void InvokeEvent()
        {
            if (tweenedTransform != null)
            {
                switch (property)
                {
                    case ETransformProperty.Position:
                        tweenedTransform.position = InterpolateValue();
                        break;
                    case ETransformProperty.Scale:
                        tweenedTransform.localScale = InterpolateValue();
                        break;
                }
            }
            else
                base.InvokeEvent();
        }

        protected override Vector3 InterpolateValue()
        {
            return Tween.InterpolateValue(startValue, targetValue, timer, animParams);
        }
    }

    public class QuaternionTweener : EventTweener<Quaternion>
    {
        private Transform tweenedTransform;

        public QuaternionTweener(MonoBehaviour target, Tween masterTween, Quaternion startValue, Quaternion targetValue, Action<Quaternion> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
        {
        }

        public QuaternionTweener(MonoBehaviour target, Tween masterTween, Transform transform, Quaternion startValue, Quaternion targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
        {
            tweenedTransform = transform;
        }

        protected override void InvokeEvent()
        {
            if (tweenedTransform != null) tweenedTransform.rotation = InterpolateValue();
            else base.InvokeEvent();
        }

        protected override Quaternion InterpolateValue()
        {
            return Tween.InterpolateValue(startValue, targetValue, timer, animParams);
        }
    }

    public class ColorTweener : EventTweener<Color>
    {
        private Material mat;
        private string property;

        public ColorTweener(MonoBehaviour target, Tween masterTween, Color startValue, Color targetValue, Action<Color> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
        {
        }

        public ColorTweener(MonoBehaviour target, Tween masterTween, Material material, in string property, Color startValue, Color targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
            base(target, masterTween, startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
        {
            mat = material;
            this.property = property;
        }

        protected override void InvokeEvent()
        {
            if (mat != null)
            {
                mat.SetColor(property, InterpolateValue());
            }
            else
                base.InvokeEvent();
        }

        protected override Color InterpolateValue()
        {
            return Tween.InterpolateValue(startValue, targetValue, timer, animParams);
        }
    }
}