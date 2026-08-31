using ArTiX.FactoryGame.Datas;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    public class RecipeButton : ButtonExtension<RecipeSO>
    {
        [SerializeField] private Image icon;

        public override void AddClickEvent(Action<RecipeSO> onClickEvent, in RecipeSO parameter)
        {
            base.AddClickEvent(onClickEvent, parameter);
            icon.sprite = parameter.ResourceOutput.resource.Icon;
            icon.enabled = true;
        }
    }
}