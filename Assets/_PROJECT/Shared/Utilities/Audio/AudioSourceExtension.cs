using ArTiX.Utils.Pool;
using System;
using UnityEngine;

namespace ArTiX.Utils.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceExtension : MonoBehaviour, IPooledObject<AudioSource>
    {
        public AudioSource Source { get; private set; }

        public Pool<AudioSource> Pool { get; set; }

        public event EventHandler OnFinishPlayingEvent;

        private void Awake()
        {
            Source = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Source.time >= Source.clip.length)
                OnFinishPlayingEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Release() => Pool.ReturnToPool(Source);
    }
}