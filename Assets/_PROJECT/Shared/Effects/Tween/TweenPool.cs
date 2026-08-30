using ArTiX.Utils;
using ArTiX.Utils.Pool;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public class TweenPool : Singleton<TweenPool>
    {
        [SerializeField] private Tween prefabTween;

        public Pool<Tween> Pool { get; private set; }
        private const int DEFAULT_POOL_SIZE = 30;
        private const int MAX_POOL_SIZE = DEFAULT_POOL_SIZE * 2;

        protected override void Awake()
        {
            SetInstance(this);
            Pool = new Pool<Tween>(CreateTween, DEFAULT_POOL_SIZE, MAX_POOL_SIZE);
        }

        private Tween CreateTween()
        {
            Tween tween = Instantiate(prefabTween);
            return tween;
        }
    }
}