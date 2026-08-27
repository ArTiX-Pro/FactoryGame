using System;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public static class TweenClassExtension
    {
        #region Transform

        // POSITION
        public static Tween TweenPosition(this Transform transform, in Vector3 startPos, in Vector3 targetPos, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(transform, ETransformProperty.Position, startPos, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenPosition(this Transform transform, in Vector3 targetPos, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(transform, ETransformProperty.Position, transform.position, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // ROTATION
        public static Tween TweenRotation(this Transform transform, in Quaternion startRota, in Quaternion targetRota, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            tween ??= Tween.Create();
            tween.TweenProperty(transform, startRota, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenRotation(this Transform transform, in Quaternion targetRota, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            tween ??= Tween.Create();
            tween.TweenProperty(transform, transform.rotation, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // SCALE
        public static Tween TweenScale(this Transform transform, in Vector3 startScale, in Vector3 targetScale, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(transform, ETransformProperty.Scale, startScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenScale(this Transform transform, in Vector3 targetScale, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(transform, ETransformProperty.Scale, transform.localScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        #region Material

        public static Tween TweenFloatProperty(this Material mat, in string property, in float startValue, in float targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(mat, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenFloatProperty(this Material mat, in string property, in float targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(mat, property, mat.GetFloat(property), targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenColorProperty(this Material mat, in string property, in Color startColor, in Color targetColor, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(mat, property, startColor, targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenColorProperty(this Material mat, in string property, in Color targetColor, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            if (tween == null) tween = Tween.Create();
            tween.TweenProperty(mat, property, mat.GetColor(property), targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        public static Tween TweenAlpha(this CanvasGroup group, in float targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            tween ??= Tween.Create();
            tween.TweenProperty(group, group.alpha, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static Tween TweenAlpha(this CanvasGroup group, in float startValue, in float targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, Tween tween = null)
        {
            tween ??= Tween.Create();
            tween.TweenProperty(group, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

    }
}