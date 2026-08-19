using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
{
    public class Ghost : MonoBehaviour
    {
        public static Ghost Create(Ghost ghost)
        {
            return Instantiate(ghost);
        }

        [SerializeField] private GhostSO datas;

        private MasterTween.Tween rotationTween;
        private MasterTween.Tween positionTween;

        private MasterTween.Tween colorTween;

        private bool isValid;

        private void Start()
        {
            SetIsValid(true);
        }

        public void SetPosition(in Vector3 position)
        {
            positionTween?.Kill();
            transform.TweenPosition(
                targetPos: position,
                animParams: datas.PositionAnim,
                tween: positionTween);
        }

        public void SetRotation(in float rotation)
        {
            rotationTween?.Kill();
            transform.TweenRotation(
                targetRota: Quaternion.Euler(x: 0, rotation, z: 0),
                animParams: datas.RotationAnim,
                tween: rotationTween);
        }

        public void SetIsValid(in bool isValid)
        {
            if (isValid == this.isValid) return;
            this.isValid = isValid;

            colorTween?.Kill();
            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
            {
                renderer.material.TweenColorProperty(
                    property: Utilities.COLOR,
                    targetColor: isValid ? datas.ValidColor : datas.UnvalidColor,
                    animParams: datas.ColorChangeAnim,
                    tween: colorTween
                );
            }
        }

        public void Kill()
        {
            positionTween?.Kill();
            rotationTween?.Kill();
            Destroy(gameObject);
        }
    }
}