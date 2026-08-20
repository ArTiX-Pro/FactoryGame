using ArTiX.FactoryGame.Datas;
using ArTiX.FactoryGame.UI;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject(nameof(GameManager), components: typeof(GameManager)).GetComponent<GameManager>();

                return instance;
            }
        }

        [SerializeField] private List<BuildingSO> availableBuildings;
        [SerializeField] private List<RecipeSO> availableRecipes;
        public List<RecipeSO> AvailableRecipes => availableRecipes;

        [SerializeField] private List<ItemSO> availableResources;
        public List<ItemSO> AvailableResources => availableResources;

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
            HUD.Instance.SetBuildingBtns(availableBuildings);
        }
    }
}