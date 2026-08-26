using ArTiX.GridBuildingSystem.Datas;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.FactoryGame
{
    public class ProceduralGenerator : MonoBehaviour
    {
        [Serializable]
        private struct SResourceRarity
        {
            [field: SerializeField] public PlacedObjectSO ResourceNode { get; private set; }
            [field: SerializeField, Range(0, 1)] public float ValueOffset {  get; private set; }
        }

        [SerializeField] private GridSO gridDatas;
        [SerializeField, Range(0, 1)] private float rarity;
        [SerializeField] private float noiseFrequency;
        [SerializeField] private SResourceRarity[] resourcesRarity;

        private void Start()
        {
            Noise.FastNoise fastNoise = new Noise.FastNoise();
            fastNoise.SetFrequency(noiseFrequency);

            List<Vector2Int> possibleCells = new List<Vector2Int>();

            int width = gridDatas.Width;
            int height = gridDatas.Height;

            float[] rawNoise = new float[width * height];
            float perlin;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // fastNoise.GetPerlin gives a value between [-1;1], we move it between [0;1]
                    perlin = fastNoise.GetPerlin((float)x / width, y / (float)height);
                    perlin += 1;
                    perlin *= .5f;
                    rawNoise[(x * height) + y] = perlin;
                }
            }

            // Raw Perlin values aren't spread evenly across [0;1], they bunch up around .5,
            // so we equalize them against their own distribution before comparing to ValueOffset.

            int count = rawNoise.Length;
            int[] sortedIndices = new int[count];
            for (int i = 0; i < count; i++)
                sortedIndices[i] = i;

            Array.Sort(sortedIndices, (a, b) => rawNoise[a].CompareTo(rawNoise[b]));

            float[] equalizedNoise = new float[count];
            for (int rank = 0; rank < count; rank++)
                equalizedNoise[sortedIndices[rank]] = rank / (float)(count - 1);

            GridBuildingSystem.GridBuildingSystem grid = GridBuildingSystem.GridBuildingSystem.Instance;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    perlin = equalizedNoise[(x * height) + y];

                    foreach (SResourceRarity resourceRarity in resourcesRarity)
                    {
                        if (Mathf.Abs(perlin - resourceRarity.ValueOffset) < rarity)
                        {
                            grid.CreateBuilding(resourceRarity.ResourceNode, new Vector2Int(x, y));
                            break;
                        }
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}