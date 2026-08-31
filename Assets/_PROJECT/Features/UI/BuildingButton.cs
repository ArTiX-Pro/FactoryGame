using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.GridBuildingSystem.UI
{
    [RequireComponent(typeof(Image))]
    public class BuildingButton : ButtonExtension<BuildingSO>, IMouseTooltip
    {
        [SerializeField] private Image icon;

        public string Tooltip => parameter.Name;

        public override void AddClickEvent(Action<BuildingSO> onClickEvent, in BuildingSO parameter)
        {
            base.AddClickEvent(onClickEvent, parameter);
            icon.sprite = parameter.Icon;
        }
    }
}