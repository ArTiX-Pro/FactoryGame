using ArTiX.GridBuildingSystem.Datas;
using ArTiX.GridBuildingSystem.UI;
using ArTiX.Input;
using ArTiX.Utils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    public class HUD : Singleton<HUD>
    {
        [SerializeField] private float toggleBuildingAnimDuration;
        [SerializeField] private Ease toggleBuildingAnimEasing;
        [SerializeField] private Transform buildingPanel;

        [SerializeField] private float buildingPanelActiveHeight;
        private float BuildingPanelActiveHeight
        {
            get => buildingPanelActiveHeight * CurrentScaleFactor.y;
        }

        [SerializeField] private float buildingPanelInactiveHeight;
        private float BuildingPanelInactiveHeight
        {
            get => buildingPanelInactiveHeight * CurrentScaleFactor.y;
        }

        [SerializeField] private BuildingButton prefabBuildingBtn;
        [SerializeField] private Transform buildingBtnsHolder;

        public Vector2 CurrentScaleFactor { get; private set; }

        private bool isBuildingPanelActive;

        private Tween tween;

        protected override void Awake()
        {
            SetInstance(this);
        }

        private void Start()
        {
            InputManager.Instance.OnToggleBuildingPanel += ToggleBuildingPanel;
        }

        private void OnRectTransformDimensionsChange()
        {
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            CurrentScaleFactor = new Vector2(Screen.width / canvasScaler.referenceResolution.x, Screen.height / canvasScaler.referenceResolution.y);

            buildingPanel.position = new Vector3(
                x: buildingPanel.position.x,
                y: isBuildingPanelActive ? BuildingPanelActiveHeight : BuildingPanelInactiveHeight,
                z: buildingPanel.position.z);
        }

        public void SetBuildingBtns(List<BuildingSO> availableBuildings)
        {
            BuildingButton button;
            foreach (BuildingSO buildingData in availableBuildings)
            {
                button = Instantiate(prefabBuildingBtn, buildingBtnsHolder);
                button.AddClickEvent(SelectBuilding, buildingData);
            }
        }

        public void ToggleBuildingPanel()
        {
            isBuildingPanelActive = !isBuildingPanelActive;

            tween.Kill();
            tween = buildingPanel.DOMoveY(
                endValue: isBuildingPanelActive ? BuildingPanelActiveHeight : BuildingPanelInactiveHeight,
                duration: toggleBuildingAnimDuration
            ).SetEase(toggleBuildingAnimEasing);
        }

        public void EnterDestructionMode()
        {
            GridBuildingSystem.GridBuildingSystem.Instance.EnterDestructionState();
        }

        private void SelectBuilding(BuildingSO buildingDatas)
        {
            GridBuildingSystem.GridBuildingSystem.Instance.SelectBuilding(buildingDatas);
        }
    }
}