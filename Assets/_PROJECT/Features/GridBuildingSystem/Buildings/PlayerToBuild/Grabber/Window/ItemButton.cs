using ArTiX.Inventory;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    [RequireComponent(typeof(SpriteToggle))]
    public class ItemButton : ButtonExtension<ItemSO>, IMouseTooltip
    {
        [SerializeField] private Image icon;

        public string Tooltip => parameter.Name;

        public override void AddClickEvent(Action<ItemSO> onClickEvent, in ItemSO parameter)
        {
            base.AddClickEvent(onClickEvent, parameter);
            icon.sprite = parameter.Icon;
        }
    }
}