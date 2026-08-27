using ArTiX.Utils;
using ArTiX.Utils.Pool;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public class TweenPool : Singleton<TweenPool>
    {
        [SerializeField] private Tween prefabTween;

        public Pool<Tween> Pool { get; private set; }

        private static int DEFAULT_POOL_SIZE = 30;

        protected override void Awake()
        {
            SetInstance(this);
            Pool = new Pool<Tween>(CreateTween, DEFAULT_POOL_SIZE);
        }

        private Tween CreateTween()
        {
            Tween tween = Instantiate(prefabTween);
            tween.Pool = Pool;
            return tween;
        }
    }
}