using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.GridBuildingSystem.UI
{
    [RequireComponent(typeof(Image))]
    public class BuildingButton : BaseButton<BuildingSO>, IMouseTooltip
    {
        public string Tooltip => parameter.Name;

        public override void Initialize(Action<BuildingSO> onClickEvent, in BuildingSO parameter)
        {
            base.Initialize(onClickEvent, parameter);

            GetComponent<Image>().sprite = parameter.Icon;
        }
    }
}