using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Input;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
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

        [SerializeField] private MasterTween.AnimParams toggleBuildingAnim;

        [SerializeField] private Transform buildingPanel;
        [SerializeField] private float buildingPanelActiveHeight, buildingPanelInactiveHeight;

        [SerializeField] private Transform buildingBtnsHolder;

        private bool isBuildingPanelActive;

        private MasterTween.Tween tween;

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

        public void ToggleBuildingPanel()
        {
            isBuildingPanelActive = !isBuildingPanelActive;

            tween?.Kill();
            tween = MasterTween.Create();
            buildingPanel.TweenPosition(
                targetPos: new Vector3(
                    x: buildingPanel.position.x, 
                    y: isBuildingPanelActive ? buildingPanelActiveHeight : buildingPanelInactiveHeight),
                animParams: toggleBuildingAnim
            );
        }

        public void SelectBuilding(PlacedObjectDatas buildingDatas)
        {
            GridBuildingSystem.Instance.SelectBuilding(buildingDatas);
        }
    }
}