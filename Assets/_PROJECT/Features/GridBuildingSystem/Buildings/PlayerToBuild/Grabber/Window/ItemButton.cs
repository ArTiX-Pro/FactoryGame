using ArTiX.Inventory;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemButton : ButtonExtension<ItemSO>
    {
        public override void AddClickEvent(Action<ItemSO> onClickEvent, in ItemSO parameter) 
        {
            base.AddClickEvent(onClickEvent, parameter);
            GetComponent<Image>().sprite = parameter.Icon;
        }
    }
}