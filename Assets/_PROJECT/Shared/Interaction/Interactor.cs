using ArTiX.Utils.TickSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.Interaction
{
    [RequireComponent(typeof(PlayerInput))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Camera interactionCamera;
        [SerializeField] private InteractorSO datas;

        private IInteractable interactable;
        private Outline outline;

        private void Start()
        {
            TickSystem.Instance.OnTick += CheckForInteractable;

            interactionCamera ??= Camera.main;
        }

        public void Interact(InputAction.CallbackContext ctxt)
        {
            if (ctxt.started)
                interactable?.Interact();
        }

        private void CheckForInteractable(object sender, EventArgs e)
        {
            Ray ray = interactionCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, datas.InteractionDistance, datas.InteractionMask))
            {
                if (!hit.collider.TryGetComponent(out IInteractable foundInteractable))
                {
                    Debug.LogWarning(
                        $"Object is on interaction layer but does not implement {nameof(IInteractable)} interface.");
                    return;
                }

                if (foundInteractable != interactable)
                {
                    if (outline != null) outline.enabled = false;

                    interactable = foundInteractable;
                    if (!hit.collider.TryGetComponent(out outline))
                    {
                        outline = hit.collider.gameObject.AddComponent(typeof(Outline)) as Outline;
                    }

                    outline.OutlineColor = datas.OutlineColor;
                    outline.OutlineMode = datas.OutlineMode;
                    outline.OutlineWidth = datas.OutlineWidth;
                    outline.enabled = true;
                }
            }
            else if (interactable != null)
            {
                interactable = null;
                outline.enabled = false;
                outline = null;
            }
        }
    }
}