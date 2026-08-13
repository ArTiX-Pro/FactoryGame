using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.GridBuildingSystem.Tools;
using ArTiX.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "BaseBuilding", menuName = "Datas/PlacedObject/BaseBuilding")]
    public class PlacedObjectSO : ScriptableObject
    {
        [Title("Base datas")]
        [SerializeField] private string objName;
        [SerializeField] private Sprite icon;
        [SerializeField] private PlacedObject prefab;
        [SerializeField] private Transform ghost;

        [SerializeField] private Vector2Int[] cellArray;

#if UNITY_EDITOR

        [Button]
        public void SetCells()
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene("Assets/_PROJECT/Scenes/scn_GridCellCreator.unity");
            EditorApplication.EnterPlaymode();
            SetupGridValue.Instance.Building = this;
            SetupGridValue.Instance.previousScenePath = currentScenePath;
        }

        public Vector2Int[] CellArray
        {
            get => cellArray;
            set => cellArray = value;
        }

#endif

        public string       Name => objName;
        public Sprite       Icon => icon;
        public PlacedObject Prefab => prefab;
        public Transform    Ghost => ghost;

        public List<Vector2Int> GetOccupiedCells(in Vector2Int offset, in float rotation)
        {
            List<Vector2Int> occupiedCells = new List<Vector2Int>();
            foreach (Vector2Int cell in cellArray)
            {
                occupiedCells.Add(cell.Rotate(rotation) + offset);
            }

            return occupiedCells;
        }
    }
}