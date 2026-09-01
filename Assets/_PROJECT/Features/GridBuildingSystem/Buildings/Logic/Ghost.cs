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
        [SerializeField] private BuildingSO buildingDatas;

        private Vector2Int cellPos;
        private float targetRotation;

        private Tween rotationTween;
        private Tween positionTween;

        private Tween colorTween;

        private void Start()
        {
            UpdateColor();
        }

        public void SetPosition(in Vector2Int cellPos)
        {
            if (positionTween != null)
                positionTween.Kill(this);
            positionTween = null;

            this.cellPos = cellPos;
            transform.TweenPosition(this,
                targetPos: GridBuildingSystem.Instance.ConvertCellPosToWorldPos(cellPos),
                animParams: datas.PositionAnim,
                tween: ref positionTween);

            UpdateColor();
        }

        public void SetRotation(in float rotation)
        {
            if (rotationTween != null) rotationTween.Kill(this);
            rotationTween = null;

            targetRotation = rotation;
            transform.TweenRotation(this,
                targetRota: Quaternion.Euler(x: 0, rotation, z: 0),
                animParams: datas.RotationAnim,
                tween: ref rotationTween);

            UpdateColor();
        }

        private void UpdateColor()
        {
            bool isValid = GridBuildingSystem.Instance.CanBuild(buildingDatas, cellPos, targetRotation);

            if (colorTween != null) colorTween.Kill(this);
            colorTween = null;

            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
            {
                renderer.material.TweenColorProperty(this, this,
                    property: Utilities.COLOR,
                    targetColor: isValid ? datas.ValidColor : datas.UnvalidColor,
                    animParams: datas.ColorChangeAnim,
                    tween: ref colorTween
                );
            }
        }

        public void Destroy()
        {
            if (positionTween != null) positionTween.Kill(this);
            if (rotationTween != null) rotationTween.Kill(this);
            if (colorTween != null) colorTween.Kill(this);

            Destroy(gameObject);
        }
    }
}