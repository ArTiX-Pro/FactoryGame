using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Input;
using ArTiX.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.GridBuildingSystem
{
    public class GridBuildingSystem : MonoBehaviour
    {
        private static GridBuildingSystem instance;
        public static GridBuildingSystem Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(GridBuildingSystem), typeof(GridBuildingSystem))
                        .GetComponent<GridBuildingSystem>();
                }
                return instance;
            }
        }

        #region Variables

        [SerializeField] private GridSO datas;

        private Grid<PlacedObject> grid;

        private Transform ghost;
        private PlacedObjectSO currentBuildingDatas;
        private float currentRotation;
        private float BuildingRotationInWorld => -currentRotation;
        private MasterTween.Tween buildingRotationTween;

        private const float ROTATION_DELTA = 90f;
        private const float MAX_ROTATION = 360;

        private delegate void DoAction();
        private DoAction doAction;

        #endregion

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
            GenerateGrid();
        }

        private void Update()
        {
            doAction?.Invoke();
        }

        public void GenerateGrid()
        {
            if (grid != null)
            {
                for (int i = 0; i < datas.Width; i++)
                {
                    for (int j = 0; j < datas.Height; j++)
                    {
                        if (grid.GetCellValue(i, j) != null)
                            Destroy(grid.GetCellValue(i, j));
                    }
                }

                grid.Clear();
                return;
            }

            grid = new Grid<PlacedObject>(datas.Width, datas.Height);

            GameObject gridVisual = Instantiate(datas.PrfbGridVisual, datas.Origin, Quaternion.identity);
            // Dividing by 10 because using a plane and a plane base size is 10 units in width and height
            gridVisual.transform.localScale = new Vector3(datas.Width / 10, 0, datas.Height / 10);

            Material gridMat = gridVisual.GetComponent<Renderer>().material;
            gridMat.SetFloat(GridSO.WIDTH, datas.Width);
            gridMat.SetFloat(GridSO.HEIGHT, datas.Height);
            gridMat.SetFloat(GridSO.CELL_SIZE, datas.CellSize);
            gridMat.SetFloat(GridSO.LINE_WIDTH, datas.LineWidth);
        }

        public PlacedObject GetObjectOnCell(Vector2Int cell) => grid.GetCellValue(cell);

        #region World pos, grid pos

        private Vector2Int ConvertWorldPosToCellPos(Vector3 worldPos)
        {
            worldPos.y = 0;
            worldPos += .5f * new Vector3(datas.Width, 0, datas.Height);

            return new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.z));
        }

        private Vector3 ConvertCellPosToWorldPos(in Vector2Int cell)
        {
            Vector3 worldPos = new Vector3(
                x: datas.CellSize * (cell.x + .5f - (datas.Width * .5f)), 
                y: 0, 
                z: datas.CellSize * (cell.y + .5f - (datas.Width * .5f))
            );

            return worldPos;
        }

        private Vector3 SnapPos(in Vector3 worldPos) 
            => ConvertCellPosToWorldPos(ConvertWorldPosToCellPos(worldPos));

        private Vector3 RealWorldMousePosOnGrid()
        {
            Transform camTrans = Camera.main.transform;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            // Intersection between a line between camera and the ground.
            float t = -camTrans.position.y / ray.direction.y;

            return new Vector3(
                x: (t * ray.direction.x) + camTrans.position.x,
                y: 0,
                z: (t * ray.direction.z) + camTrans.position.z);
        }

        private Vector2Int GetMouseCellPos() => ConvertWorldPosToCellPos(RealWorldMousePosOnGrid());

        #endregion

        #region State Machine

        private void InitBuildingState()
        {
            InputManager.Instance.OnSelect += CreateBuilding;
            InputManager.Instance.OnCancel += ExitBuildingState;
            InputManager.Instance.OnRotate += RotateBuilding;

            currentRotation = 0;

            doAction = BuildingState;
        }

        private void BuildingState()
        {
            // Snap to cell pos
            Vector2Int cellPos = GetMouseCellPos();
            ghost.position = MasterTween.InterpolateValue(
                startValue: ghost.position,
                targetValue: ConvertCellPosToWorldPos(cellPos),
                percentage: Time.deltaTime * datas.GhostSpeed,
                transition: datas.GhostMoveTransition);

            foreach (Renderer renderer in ghost.GetComponentsInChildren<Renderer>())
                renderer.material.SetColor(Utilities.COLOR, 
                    value: CanBuild(cellPos) ? datas.ValidColor : datas.UnvalidColor);
        }

        private void ExitBuildingState()
        {
            doAction = null;
            currentRotation = 0;
            Destroy(ghost.gameObject);
            InputManager.Instance.OnSelect -= CreateBuilding;
            InputManager.Instance.OnCancel -= ExitBuildingState;
            InputManager.Instance.OnRotate -= RotateBuilding;

            buildingRotationTween?.Kill();
        }

        #endregion

        #region Building

        public void SelectBuilding(PlacedObjectSO buildingDatas)
        {
            if (doAction == BuildingState) Destroy(ghost.gameObject);
            else InitBuildingState();

            ghost = Instantiate(buildingDatas.Ghost);
            currentBuildingDatas = buildingDatas;
        }

        private void RotateBuilding(int rotateDirection)
        {
            currentRotation = (currentRotation + (ROTATION_DELTA * rotateDirection)) % MAX_ROTATION;
            buildingRotationTween?.Kill();
            buildingRotationTween = MasterTween.Create("buildingRotationTween");
            ghost.TweenRotation(
                targetRota: Quaternion.Euler(x: 0, BuildingRotationInWorld, z: 0),
                animParams: new MasterTween.AnimParams(duration: .4f, transition: MasterTween.ETransition.SmoothStop3),
                tween: buildingRotationTween);
        }

        private void CreateBuilding()
        {
            Vector2Int origin = GetMouseCellPos();
            if (CanBuild(origin))
            {
                PlacedObject building = Instantiate(
                    original: currentBuildingDatas.Prefab,
                    position: SnapPos(RealWorldMousePosOnGrid()),
                    rotation: Quaternion.Euler(x: 0, BuildingRotationInWorld, z: 0)
                );

                List<Vector2Int> occupiedCells = currentBuildingDatas.GetOccupiedCells(origin, currentRotation);
                foreach (Vector2Int cell in occupiedCells)
                {
                    grid.SetCellValue(cell, building);
                }

                building.Initialize(origin);
            }
            else
            {
                // Fail effects
            }
        }

        private bool CanBuild(in Vector2Int origin)
        {
            foreach (Vector2Int cell in currentBuildingDatas.GetOccupiedCells(origin, currentRotation))
            {
                if (grid.GetCellValue(cell) != null || !grid.IsCellWithinGrid(cell))
                    return false;
            }

            return true;
        }

        public void DestroyBuilding(PlacedObject building)
        {
            foreach (Vector2Int cell in building.GetOccupiedCells())
            {
                grid.SetCellValue(cell, null);
            }

            Destroy(building.gameObject);
        }

        #endregion
    }
}