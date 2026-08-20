using ArTiX.Inventory;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemButton : BaseButton<ItemSO>
    {
        public override void Initialize(Action<ItemSO> onClickEvent, in ItemSO parameter) 
        {
            base.Initialize(onClickEvent, parameter);
            GetComponent<Image>().sprite = parameter.Icon;
        }
    }
}