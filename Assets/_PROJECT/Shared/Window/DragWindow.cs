using ArTiX.FactoryGame.UI;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArTiX.Utils
{
    public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        public enum EWindowType
        {
            Default,
            Convertor,
            Grabber
        }

        public static DragWindow Create(in EWindowType windowType)
        {
            string windowPath;
            switch (windowType)
            {
                default:
                case EWindowType.Default:
                    windowPath = "Assets/_PROJECT/Shared/Window/Window.prefab";
                    break;
                case EWindowType.Convertor:
                    windowPath = "Assets/_PROJECT/Features/GridBuildingSystem/Buildings/PlayerToBuild/Convertor/Window/ConvertorWindow.prefab";
                    break;
                case EWindowType.Grabber:
                    windowPath = "Assets/_PROJECT/Features/GridBuildingSystem/Buildings/PlayerToBuild/Grabber/Window/prfb_GrabberWindow.prefab";
                    break;
            }

            GameObject window = AssetDatabase.LoadAssetAtPath<GameObject>(windowPath);
            window = Instantiate(window, HUD.Instance.transform);
            DragWindow dragWindow = window.GetComponentInChildren<DragWindow>();
            return dragWindow;
        }

        [SerializeField, Required] private RectTransform draggedWindow;
        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            if (canvas == null)
            {
                Transform currentTransform = transform.parent;
                while (canvas == null)
                {
                    if (!currentTransform.TryGetComponent<Canvas>(out canvas))
                        currentTransform = currentTransform.parent;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            draggedWindow.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            draggedWindow.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public virtual void CloseWindow()
        {
            Destroy(draggedWindow.gameObject);
        }
    }
}