using ArTiX.Utils.Pool;
using UnityEngine;

namespace ArTiX.Utils.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        private const int INITIAL_POOL_SIZE = 16;
        private const int MAX_POOL_SIZE = 48;

        [SerializeField] private AudioSourceExtension sourcePrefab;

        private Pool<AudioSource> pool;

        protected override void Awake()
        {
            SetInstance(this);

            if (sourcePrefab == null)
            {
                Debug.LogError($"{nameof(AudioManager)} has no {nameof(sourcePrefab)} assigned, the audio pool stays empty.", this);
                return;
            }

            pool = new Pool<AudioSource>(CreateSource, INITIAL_POOL_SIZE, MAX_POOL_SIZE);
        }

        private AudioSource CreateSource()
        {
            AudioSourceExtension pooled = Instantiate(sourcePrefab, transform);
            pooled.gameObject.SetActive(false);
            return pooled.GetComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            s_instance = null;
        }
    }
}
