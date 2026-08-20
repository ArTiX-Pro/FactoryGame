using ArTiX.FactoryGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ArTiX.Utils
{
    public class MouseTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform prefabTooltip;
        [SerializeField] private Font font;
        [SerializeField] private Vector2 backgroundSizeOffset;

        private RectTransform tooltipObj;

        private readonly TextGenerator textGenerator = new TextGenerator();
        private TextGenerationSettings settings;

        private IMouseTooltip tooltip;

        private void Start()
        {
            tooltipObj = Instantiate(prefabTooltip, HUD.Instance.transform);
            tooltipObj.gameObject.SetActive(false);

            TextMeshProUGUI textObj = tooltipObj.GetComponentInChildren<TextMeshProUGUI>();
            settings = new TextGenerationSettings
                    {
                        fontSize = (int)textObj.fontSize,
                        font = font,
                        verticalOverflow = VerticalWrapMode.Overflow
                    };
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue) &&
                hit.collider.TryGetComponent(out IMouseTooltip mouseTooltip))
            {
                if (mouseTooltip != tooltip && mouseTooltip.Tooltip != "")
                {
                    tooltipObj.gameObject.SetActive(true);
                    tooltip = mouseTooltip;
                    UpdateText();
                }
            }
            else
            {
                tooltipObj.gameObject.SetActive(false);
                tooltip = null;
                return;
            }

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            tooltipObj.anchorMax = tooltipObj.anchorMin = mouseScreenPos / screenSize;
            tooltipObj.anchoredPosition = tooltipObj.sizeDelta / 2;
        }

        private void UpdateText()
        {
            float width = textGenerator.GetPreferredWidth(tooltip.Tooltip, settings);
            float height = textGenerator.GetPreferredHeight(tooltip.Tooltip, settings);

            TextMeshProUGUI textObj = tooltipObj.GetComponentInChildren<TextMeshProUGUI>();
            textObj.text = tooltip.Tooltip;
            textObj.rectTransform.sizeDelta = new Vector2(width, height) + backgroundSizeOffset;

            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipObj);
        }
    }
}