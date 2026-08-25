using ArTiX.Utils.TickSystem;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ArTiX.Interaction
{
    [RequireComponent(typeof(PlayerInput))]
    public class Interactor : MonoBehaviour
    {
        private static Interactor instance;
        public static Interactor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(Interactor), typeof(Interactor))
                        .GetComponent<Interactor>();
                }
                return instance;
            }
        }

        [SerializeField] private Camera interactionCamera;
        [SerializeField] private InteractorSO datas;

        private IInteractable interactable;
        private Outline outline;

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
            interactionCamera ??= Camera.main;
            Enable();
        }

        public void Interact(InputAction.CallbackContext ctxt)
        {
            if (ctxt.started)
                interactable?.Interact();
        }

        private void CheckForInteractable(object sender, EventArgs e)
        {
            void DisableOutline()
            {
                if (outline == null) return;
                outline.enabled = false;
                interactable = null;
                outline = null;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                DisableOutline();
                return; 
            }

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
            else
            {
                DisableOutline();
            }
        }

        public void Enable()
        {
            TickSystem.Instance.OnTick += CheckForInteractable;
        }
        public void Disable()
        {
            TickSystem.Instance.OnTick -= CheckForInteractable;
        }
    }
}