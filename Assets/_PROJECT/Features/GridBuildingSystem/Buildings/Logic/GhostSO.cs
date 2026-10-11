using DG.Tweening;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "GhostSO", menuName = "Datas/GhostSO")]
    public class GhostSO : ScriptableObject
    {
        [Header("Animation")]
        // POSITION
        [SerializeField] private float tweenPositionDuration;
        public float TweenPositionDuration => tweenPositionDuration;

        [SerializeField] private Ease tweenPositionEasing;
        public Ease TweenPositionEasing => tweenPositionEasing;

        // ROTATION
        [SerializeField] private float tweenRotationDuration;
        public float TweenRotationDuration => tweenRotationDuration;

        [SerializeField] private Ease tweenRotationEasing;
        public Ease TweenRotationEasing => tweenRotationEasing;

        // COLOR
        [SerializeField] private float colorChangeDuration;
        public float ColorChangeDuration => colorChangeDuration;

        [SerializeField] private Ease colorChangeEasing;
        public Ease ColorChangeEasing => colorChangeEasing;

        [Space(10)]
        [Header("Color")]
        [SerializeField] private Color validColor = Color.green;
        public Color ValidColor => validColor;

        [SerializeField] private Color unvalidColor = Color.red;
        public Color UnvalidColor => unvalidColor;
    }
}