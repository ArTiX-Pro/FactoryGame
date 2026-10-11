using TMPro;
using UnityEngine;

namespace ArTiX.Utils.Window
{
    public abstract class ObjectWindow<T> : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI title;

        protected T owner;

        public virtual void Initialize(T owner)
        {
            this.owner = owner;
        }
    }
}