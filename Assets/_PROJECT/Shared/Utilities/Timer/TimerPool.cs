using ArTiX.Utils.Pool;
using UnityEngine;

namespace ArTiX.Utils
{
    public class TimerPool : Singleton<TimerPool>
    {
        [SerializeField] private Timer prefabTimer;

        public Pool<Timer> Pool { get; private set; }
        private const int DEFAULT_POOL_SIZE = 10;

        protected override void Awake()
        {
            SetInstance(this);
            Pool = new Pool<Timer>(CreateTimer, DEFAULT_POOL_SIZE);
        }

        // Only used to populate the pool.
        private Timer CreateTimer()
        {
            Timer timer = Instantiate(prefabTimer);
            timer.Pool = Pool;
            timer.gameObject.SetActive(false);
            return timer;
        }
    }
}
