using UnityEngine;

namespace ArTiX.Utils.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private const int INITIAL_POOL_SIZE = 16;
        private const int MAX_POOL_SIZE = 48;

        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject(nameof(AudioManager), typeof(AudioManager)).GetComponent<AudioManager>();

                return instance;
            }
        }
    }
}
