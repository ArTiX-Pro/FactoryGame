using UnityEngine;

namespace ArTiX.Utils.Window
{
    public class BaseWindow : MonoBehaviour
    {
        public virtual void CloseWindow()
        {
            Destroy(gameObject);
        }
    }
}