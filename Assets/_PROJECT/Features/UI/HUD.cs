using ArTiX.GridBuildingSystem.Datas;
using ArTiX.GridBuildingSystem.UI;
using ArTiX.Input;
using ArTiX.Utils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.FactoryGame.UI
{
    public class HUD : Singleton<HUD>
    {
        [SerializeField] private float toggleBuildingAnimDuration;
        [SerializeField] private Ease toggleBuildingAnimEasing;
        [SerializeField] private Transform buildingPanel;
        [SerializeField] private float buildingPanelActiveHeight, buildingPanelInactiveHeight;

        [SerializeField] private BuildingButton prefabBuildingBtn;
        [SerializeField] private Transform buildingBtnsHolder;

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
            tween = buildingPanel.DOMove(
                endValue: new Vector3(
                    x: buildingPanel.position.x,
                    y: isBuildingPanelActive ? buildingPanelActiveHeight : buildingPanelInactiveHeight),
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