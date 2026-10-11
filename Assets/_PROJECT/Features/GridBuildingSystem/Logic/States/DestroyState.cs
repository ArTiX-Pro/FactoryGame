using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Input;
using ArTiX.Interaction;
using ArTiX.Utils;
using ArTiX.Utils.TickSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.GridBuildingSystem
{
    public class DestroyState : GameState
    {
        private readonly Texture2D texDestroyCursor;
        private readonly Material matDestroy;
        private Material previousMaterial;

        private PlacedObject currentBuilding;

        public DestroyState(Texture2D destructionCursor, Material destructionMaterial)
        {
            texDestroyCursor = destructionCursor;
            matDestroy = destructionMaterial;
        }

        public override void EnterState()
        {
            InputManager.Instance.OnSelect += DestroyBuilding;
            InputManager.Instance.OnCancel += ExitState;

            Interactor.Instance.Disable();
            GridBuildingSystem.Instance.ToggleGridVisual(true);

            Cursor.SetCursor(texDestroyCursor, Vector2.zero, CursorMode.ForceSoftware);

            TickSystem.Instance.OnTick += CheckForPlacedObject;
        }

        public override void Update()
        {

        }

        public override void ExitState()
        {
            InputManager.Instance.OnSelect -= DestroyBuilding;
            InputManager.Instance.OnCancel -= ExitState;

            Interactor.Instance.Enable();
            GridBuildingSystem.Instance.ToggleGridVisual(false);

            //Resets the cursor to the default  
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (currentBuilding != null)
            {
                SetBuildingMaterial(previousMaterial);
                previousMaterial = null;
                currentBuilding = null;
            }

            TickSystem.Instance.OnTick -= CheckForPlacedObject;
            base.ExitState();
        }

        private void CheckForPlacedObject(object sender, System.EventArgs e)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out PlacedObject placedObject) && currentBuilding != placedObject)
                {
                    if (currentBuilding != null)
                        SetBuildingMaterial(previousMaterial);

                    currentBuilding = placedObject;
                    previousMaterial = currentBuilding.GetComponentInChildren<Renderer>().material;
                    SetBuildingMaterial(matDestroy);
                }
            }
            else if (currentBuilding != null)
            {
                SetBuildingMaterial(previousMaterial);
                previousMaterial = null;
                currentBuilding = null;
            }
        }

        private void SetBuildingMaterial(in Material material)
        {
            foreach (Renderer renderer in currentBuilding.GetComponentsInChildren<Renderer>())
            {
                renderer.material = material;
            }
        }

        private void DestroyBuilding()
        {
            if (currentBuilding == null) return;

            GridBuildingSystem.Instance.DestroyBuilding(currentBuilding);
        }
    }
}