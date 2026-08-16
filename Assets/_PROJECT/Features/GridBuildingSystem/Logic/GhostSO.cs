using ArTiX.Effects.Tween;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "GhostSO", menuName = "Datas/GhostSO")]
    public class GhostSO : ScriptableObject
    {
        [SerializeField] private MasterTween.AnimParams positionAnim;
        public MasterTween.AnimParams PositionAnim => positionAnim;

        [SerializeField] private MasterTween.AnimParams rotationAnim;
        public MasterTween.AnimParams RotationAnim => rotationAnim;

        [SerializeField] private Color validColor = Color.green;
        public Color ValidColor => validColor;

        [SerializeField] private Color unvalidColor = Color.red;
        public Color UnvalidColor => unvalidColor;
    }
}