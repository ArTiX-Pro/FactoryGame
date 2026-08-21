using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Input;
using ArTiX.Interaction;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
{
    public class BuildingState : GameState
    {
        private const float ROTATION_DELTA = 90f;
        private const float MAX_ROTATION = 360;
        private float currentRotation;
        /// <summary>
        /// Because the Y axis goes UP and we want to rotate our building positively by looking FROM up, 
        /// we need to invert the rotation.
        /// </summary>
        private float WorldRotation => -currentRotation;

        private BuildingSO currentBuildingDatas;
        private Ghost ghost; 
        
        private Vector2Int currentCell;

        public override void EnterState()
        {
            InputManager.Instance.OnSelect += CreateBuilding;
            InputManager.Instance.OnCancel += ExitState;
            InputManager.Instance.OnRotate += RotateBuilding;

            Interactor.Instance.Disable();
            currentRotation = 0;
        }

        public override void Update()
        {
            // Snap to cell pos
            Vector2Int cellPos = GridBuildingSystem.Instance.GetMouseCellPos();
            if (currentCell == cellPos) return;
            currentCell = cellPos;

            ghost.SetPosition(cellPos);
        }

        public override void ExitState()
        {
            currentRotation = 0;
            ghost.Kill();
            ghost = null;

            Interactor.Instance.Enable();
            InputManager.Instance.OnSelect -= CreateBuilding;
            InputManager.Instance.OnCancel -= ExitState;
            InputManager.Instance.OnRotate -= RotateBuilding;

            base.ExitState();
        }

        public void SetBuildingDatas(in BuildingSO buildingDatas)
        {
            currentBuildingDatas = buildingDatas;
            ghost?.Kill();
            ghost = Ghost.Create(buildingDatas.Ghost);
        }

        private void CreateBuilding()
        {
            GridBuildingSystem.Instance.CreateBuilding(currentBuildingDatas, currentCell, currentRotation);
        }

        private void RotateBuilding(int rotateDirection)
        {
            currentRotation = (currentRotation + (ROTATION_DELTA * rotateDirection)) % MAX_ROTATION;
            ghost.SetRotation(WorldRotation);
        }
    }
}