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
        public static Material MatDestroy { get; private set; }

        private readonly Texture2D texDestroyCursor;

        private PlacedObject currentBuilding;

        public DestroyState(Texture2D destructionCursor, Material matDestroy)
        {
            texDestroyCursor = destructionCursor;
            MatDestroy = matDestroy;
        }

        public override void EnterState()
        {
            InputManager.Instance.OnSelect += DestroyBuilding;
            InputManager.Instance.OnCancel += ExitState;

            Interactor.Instance.Disable();
            GridBuildingSystem.Instance.ToggleGridVisual(true);

            Cursor.SetCursor(texDestroyCursor, Vector2.up * texDestroyCursor.height, CursorMode.ForceSoftware);

            TickSystem.Instance.OnTick += CheckForPlacedObject;
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
                currentBuilding.ExitDestroyPreview();
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
                        currentBuilding.ExitDestroyPreview();

                    currentBuilding = placedObject;
                    currentBuilding.EnterDestroyPreview();
                }
            }
            else if (currentBuilding != null)
            {
                currentBuilding.ExitDestroyPreview();
                currentBuilding = null;
            }
        }

        private void DestroyBuilding()
        {
            if (currentBuilding == null) return;

            GridBuildingSystem.Instance.DestroyBuilding(currentBuilding);
        }
    }
}