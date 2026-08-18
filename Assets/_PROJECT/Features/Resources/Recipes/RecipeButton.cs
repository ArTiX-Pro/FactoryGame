using ArTiX.FactoryGame.Datas;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.FactoryGame.UI
{
    [RequireComponent(typeof(Image))]
    public class RecipeButton : BaseButton<RecipeSO>
    {
        public override void Initialize(Action<RecipeSO> onClickEvent, in RecipeSO parameter) 
        {
            base.Initialize(onClickEvent, parameter); 
            GetComponent<Image>().sprite = parameter.ResourceOutput.resource.Icon;
        }
    }
}