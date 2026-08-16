using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
{
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private GhostSO datas;

        private MasterTween.Tween rotationTween;
        private MasterTween.Tween positionTween;

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
            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
                renderer.material.SetColor(Utilities.COLOR, isValid ? datas.ValidColor : datas.UnvalidColor);
        }

        public void Kill()
        {
            positionTween?.Kill();
            rotationTween?.Kill();
            Destroy(gameObject);
        }
    }
}