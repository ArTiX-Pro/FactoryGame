using System;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public static class TweenClassExtension
    {
        #region Transform

        // POSITION
        public static Tween TweenPosition(this Transform transform, MonoBehaviour target, in Vector3 startPos, in Vector3 targetPos, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, transform, ETransformProperty.Position, startPos, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenPosition(this Transform transform, MonoBehaviour target, in Vector3 targetPos, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, transform, ETransformProperty.Position, transform.position, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // ROTATION
        public static Tween TweenRotation(this Transform transform, MonoBehaviour target, in Quaternion startRota, in Quaternion targetRota, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, transform, startRota, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenRotation(this Transform transform, MonoBehaviour target, in Quaternion targetRota, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, transform, transform.rotation, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // SCALE
        public static Tween TweenScale(this Transform transform, MonoBehaviour target, in Vector3 startScale, in Vector3 targetScale, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, transform, ETransformProperty.Scale, startScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenScale(this Transform transform, MonoBehaviour target, in Vector3 targetScale, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, transform, ETransformProperty.Scale, transform.localScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        #region Material

        public static Tween TweenFloatProperty(this Material mat, MonoBehaviour target, in string property, in float startValue, in float targetValue, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenFloatProperty(this Material mat, MonoBehaviour target, in string property, in float targetValue, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, mat.GetFloat(property), targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenColorProperty(this Material mat, MonoBehaviour target, in string property, in Color startColor, in Color targetColor, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, startColor, targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenColorProperty(this Material mat, MonoBehaviour target, in string property, in Color targetColor, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, mat.GetColor(property), targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        public static Tween TweenAlpha(this CanvasGroup group, MonoBehaviour target, in float targetValue, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, group, group.alpha, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenAlpha(this CanvasGroup group, MonoBehaviour target, in float startValue, in float targetValue, Tween.AnimParams animParams,
            ref Tween tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(target, group, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

    }
}
