using UnityEngine;

namespace ArTiX.Utils.Audio
{
    public class PlayClicSound : MonoBehaviour
    {
        public void PlayClickSound(AudioDatasSO datas) => AudioManager.Instance.Play(datas);
    }
}