using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.Input;
using ArTiX.Interaction;
using ArTiX.Utils;
using ArTiX.Utils.TickSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.GridBuildingSystem
{
    public class DestroyState : GameState
    {
        private const string DESTROY_CURSOR_TEXTURE_PATH = "Assets/_PROJECT/Features/UI/tex_BulldozerCursor.png";

        private PlacedObject currentBuilding;
        private Texture2D textureDestroyCursor;

        public override void EnterState()
        {
            InputManager.Instance.OnSelect += DestroyBuilding;
            InputManager.Instance.OnCancel += ExitState;

            Interactor.Instance.Disable();

            textureDestroyCursor ??= AssetDatabase.LoadAssetAtPath<Texture2D>(DESTROY_CURSOR_TEXTURE_PATH);
            Cursor.SetCursor(textureDestroyCursor, Vector2.zero, CursorMode.ForceSoftware);

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

            //Resets the cursor to the default  
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            base.ExitState();
        }

        private void CheckForPlacedObject(object sender, System.EventArgs e)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.TryGetComponent(out PlacedObject placedObject))
                {
                    currentBuilding = placedObject;
                }
            }
        }

        private void DestroyBuilding()
        {
            if (currentBuilding == null) return;

            GridBuildingSystem.Instance.DestroyBuilding(currentBuilding);
        }
    }
}