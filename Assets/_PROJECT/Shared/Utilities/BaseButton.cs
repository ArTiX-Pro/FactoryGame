using System;
using UnityEngine.UI;

namespace ArTiX.Utils
{
    public abstract class BaseButton<T> : Button
    {
        public event Action<T> OnClickEvent;
        protected T parameter;

        protected override void Awake()
        {
            base.Awake();
            onClick.AddListener(OnClick);
        }

        public virtual void Initialize(Action<T> onClickEvent, in T parameter)
        {
            OnClickEvent += onClickEvent;
            this.parameter = parameter;
        }

        private void OnClick() => OnClickEvent?.Invoke(parameter);
    }
}