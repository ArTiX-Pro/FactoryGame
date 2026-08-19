using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
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

        private BuildingState buildingState;
        private DestroyState destroyState;
        private GameState currentState;
        private GameState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (currentState != null)
                    {
                        currentState.OnExitState -= ExitCurrentState;
                        currentState?.ExitState();
                    }

                    currentState = value;

                    if (currentState != null)
                    {
                        currentState.EnterState();
                        currentState.OnExitState += ExitCurrentState;
                    }
                }
            }
        }

        #endregion

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            GenerateGrid();

            buildingState = new BuildingState();
            destroyState = new DestroyState();
        }

        private void Update()
        {
            currentState?.Update();
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

        private void ExitCurrentState() => currentState = null;

        #region World pos, grid pos Conversion

        private Vector2Int ConvertWorldPosToCellPos(Vector3 worldPos)
        {
            worldPos.y = 0;
            worldPos += .5f * new Vector3(datas.Width, 0, datas.Height);

            return new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.z));
        }

        public Vector3 ConvertCellPosToWorldPos(in Vector2Int cell)
        {
            Vector3 worldPos = new Vector3(
                x: datas.CellSize * (cell.x + .5f - (datas.Width * .5f)),
                y: 0,
                z: datas.CellSize * (cell.y + .5f - (datas.Width * .5f))
            );

            return worldPos;
        }

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

        public Vector2Int GetMouseCellPos() => ConvertWorldPosToCellPos(RealWorldMousePosOnGrid());

        #endregion

        #region Building

        public void SelectBuilding(BuildingSO buildingDatas)
        {
            CurrentState = buildingState;
            buildingState.SetBuildingDatas(buildingDatas);
        }

        public void EnterDestructionState()
        {
            CurrentState = destroyState;
        }

        public void CreateBuilding(PlacedObjectSO placedObjectDatas, Vector2Int origin, float rotation = 0)
        {
            if (CanBuild(placedObjectDatas, origin))
            {
                PlacedObject building = Instantiate(
                    original: placedObjectDatas.Prefab,
                    position: ConvertCellPosToWorldPos(origin),
                    rotation: Quaternion.Euler(x: 0, -rotation, z: 0)
                );

                foreach (Vector2Int cell in placedObjectDatas.GetOccupiedCells(origin, rotation))
                    grid.SetCellValue(cell, building);

                building.Initialize(origin);
            }
            else
            {
                // Fail effects
            }
        }

        public bool CanBuild(in PlacedObjectSO placedObjectDatas, in Vector2Int origin, in float rotation = 0)
        {
            foreach (Vector2Int cell in placedObjectDatas.GetOccupiedCells(origin, rotation))
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

            building.Destroy();
        }

        #endregion
    }
}