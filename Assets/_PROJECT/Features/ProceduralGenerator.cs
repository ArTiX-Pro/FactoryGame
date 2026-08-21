using ArTiX.FactoryGame.Datas;
using ArTiX.GridBuildingSystem.Datas;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.FactoryGame
{
    public class ProceduralGenerator : MonoBehaviour
    {
        [SerializeField] private GridSO gridDatas;
        [SerializeField] private List<ResourceNodeSO> resourceNodes;

        private void Start()
        {
            Noise.FastNoise fastNoise = new Noise.FastNoise();
            fastNoise.SetFrequency(5);

            List<Vector2Int> possibleCells = new List<Vector2Int>();

            GridBuildingSystem.GridBuildingSystem grid = GridBuildingSystem.GridBuildingSystem.Instance;
            float perlin;
            for (int x = 0; x < gridDatas.Width; x++)
            {
                for (int y = 0; y < gridDatas.Height; y++)
                {
                    // fastNoise.GetPerlin gives a value between [-1;1], we move it between [0;1]
                    perlin = fastNoise.GetPerlin((float)x / gridDatas.Width, y / (float)gridDatas.Height);
                    perlin += 1;
                    perlin *= .5f;

                    Debug.Log(perlin);
                    if (Mathf.Abs(perlin - .5f) < 0.0000000001f)
                    {
                        grid.CreateBuilding(resourceNodes[0], new Vector2Int(x, y));
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}