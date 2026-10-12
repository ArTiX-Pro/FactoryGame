using TMPro;
using UnityEngine;

namespace ArTiX.Utils.Window
{
    public abstract class ObjectWindow<T> : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI titleObj;

        protected T owner;

        public virtual void Initialize(T owner, string title = "Title")
        {
            this.owner = owner;
            titleObj.text = title;
        }
    }
}