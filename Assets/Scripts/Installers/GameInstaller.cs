using System;
using Assets.Scripts.Main;
using Assets.Scripts.Misc;
using Assets.Scripts.Slab;
using Assets.Scripts.Slab.States;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Slab.Slab;
using static Assets.Scripts.Util.GameEvents;

namespace Assets.Scripts.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        private Settings settings = null;

        public override void InstallBindings()
        {
            Container.Bind<Controls>().AsCached();

            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<SlabManager>().AsSingle();

            Container.Bind<Slab.Slab>().FromComponentInNewPrefab(settings.InitialSlabPrefab).WhenInjectedInto<SlabManager>(); 

            Container.BindFactory<Slab.Slab, SlabFactory>()
                .FromComponentInNewPrefab(settings.SlabPrefab)
                .WithGameObjectName("Slab")
                .UnderTransformGroup("Slabs");

            Container.Bind<SlabStateFactory>().AsSingle();
            Container.BindFactory<Slab.Slab, SlabStateFalling, SlabStateFalling.Factory>().WhenInjectedInto<SlabStateFactory>();
            Container.BindFactory<Slab.Slab, SlabStateStacked, SlabStateStacked.Factory>().WhenInjectedInto<SlabStateFactory>();
            Container.BindFactory<Slab.Slab, SlabStateMoving, SlabStateMoving.Factory>().WhenInjectedInto<SlabStateFactory>();

            Container.BindInterfacesTo<AudioHandler>().AsSingle();
            Container.Bind<GUIStyle>().AsTransient();

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameOverSignal>();
            Container.DeclareSignal<GameStartSignal>();
            Container.DeclareSignal<StackSignal>();
            Container.DeclareSignal<PerfectStackSignal>();
        }

        [Serializable]
        public class Settings
        {
            public GameObject SlabPrefab;
            public GameObject InitialSlabPrefab;
        }
    }
}