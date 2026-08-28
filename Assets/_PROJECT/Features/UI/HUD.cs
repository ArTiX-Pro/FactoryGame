using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.GridBuildingSystem.UI;
using ArTiX.Input;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.FactoryGame.UI
{
    public class HUD : MonoBehaviour
    {
        private static HUD instance;
        public static HUD Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject(nameof(HUD), components: typeof(HUD)).GetComponent<HUD>();
                
                return instance;
            }
        }

        [SerializeField] private Tween.AnimParams toggleBuildingAnim;

        [SerializeField] private Transform buildingPanel;
        [SerializeField] private float buildingPanelActiveHeight, buildingPanelInactiveHeight;

        [SerializeField] private BuildingButton prefabBuildingBtn;
        [SerializeField] private Transform buildingBtnsHolder;

        private bool isBuildingPanelActive;

        private Tween tween;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
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
                button.Initialize(SelectBuilding, buildingData);
            }
        }

        public void ToggleBuildingPanel()
        {
            isBuildingPanelActive = !isBuildingPanelActive;

            tween?.Kill();
            tween = Tween.Create();
            buildingPanel.TweenPosition(this,
                targetPos: new Vector3(
                    x: buildingPanel.position.x, 
                    y: isBuildingPanelActive ? buildingPanelActiveHeight : buildingPanelInactiveHeight),
                animParams: toggleBuildingAnim,
                tween: ref tween
            );
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