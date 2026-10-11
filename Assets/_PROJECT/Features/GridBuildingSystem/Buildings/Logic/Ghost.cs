using ArTiX.GridBuildingSystem.Datas;
using DG.Tweening;
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
        private float targetWorldRotation;

        private Tween positionTween;
        private Tween rotationTween;
        private Tween colorTween;

        private void Start()
        {
            UpdateColor();
        }

        public void SetPosition(in Vector2Int cellPos)
        {
            this.cellPos = cellPos;
            positionTween?.Kill();
            positionTween = transform.DOMove(
                endValue: GridBuildingSystem.Instance.ConvertCellPosToWorldPos(cellPos),
                datas.TweenPositionDuration)
                .SetEase(datas.TweenPositionEasing);

            UpdateColor();
        }

        public void SetRotation(in float rotation)
        {
            targetWorldRotation = rotation;

            rotationTween?.Kill();
            rotationTween =
            transform.DORotateQuaternion(
                endValue: Quaternion.Euler(x: 0, rotation, z: 0),
                duration: datas.TweenRotationDuration)
                .SetEase(datas.TweenRotationEasing);

            UpdateColor();
        }

        private void UpdateColor()
        {
            bool isValid = GridBuildingSystem.Instance.CanBuild(buildingDatas, cellPos, -targetWorldRotation);

            colorTween?.Kill();

            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
            {
                colorTween = renderer.material.DOColor(
                    endValue: isValid ? datas.ValidColor : datas.UnvalidColor,
                    duration: datas.ColorChangeDuration
                ).SetEase(datas.ColorChangeEasing);
            }
        }

        public void Destroy()
        {
            positionTween.Kill();
            rotationTween.Kill();
            colorTween.Kill();

            Destroy(gameObject);
        }
    }
}
