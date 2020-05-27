using System;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Util.GameEvents;

namespace Assets.Scripts.Misc
{
    public class AudioHandler : IInitializable, IDisposable
    {
        private readonly SignalBus signalBus;
        private readonly Settings settings;
        private readonly AudioSource audioSource;

        public AudioHandler(AudioSource audioSource, Settings settings, SignalBus signalBus)
        {
            this.signalBus = signalBus;
            this.settings = settings;
            this.audioSource = audioSource;
        }

        public void Initialize()
        {
            signalBus.Subscribe<StackSignal>(OntStack);
            signalBus.Subscribe<PerfectStackSignal>(OnPerfectStack);
        }

        private void OntStack()
        {
            audioSource.PlayOneShot(settings.StackSound);
        }

        private void OnPerfectStack()
        {
            audioSource.PlayOneShot(settings.PerfectStackSound);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<StackSignal>(OntStack);
            signalBus.Unsubscribe<PerfectStackSignal>(OnPerfectStack);
        }


        [Serializable]
        public class Settings
        {
            public AudioClip StackSound;
            public AudioClip PerfectStackSound;
        }
    }
}
