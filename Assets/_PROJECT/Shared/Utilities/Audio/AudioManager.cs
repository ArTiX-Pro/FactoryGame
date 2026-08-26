using UnityEngine;

namespace ArTiX.Utils.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(AudioManager), typeof(AudioManager)).GetComponent<AudioManager>();
                }
                return instance;
            }
        }

        [SerializeField] private AudioDatasSO datas;

        

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning($"An object of type {nameof(AudioManager)} already exists.");
                Destroy(gameObject);
                return;
            }

            instance = this;
        }
    }
}