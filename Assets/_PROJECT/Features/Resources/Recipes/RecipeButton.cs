using ArTiX.FactoryGame.Datas;
using ArTiX.Utils;
using System;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    public class RecipeButton : BaseButton<RecipeSO>
    {
        public override void Initialize(Action<RecipeSO> onClickEvent, in RecipeSO parameter) 
        {
            base.Initialize(onClickEvent, parameter);
            GetComponentInChildren<Image>().sprite = parameter.ResourceOutput.resource.Icon;
        }
    }
}