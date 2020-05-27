using Assets.Scripts.Misc;
using Assets.Scripts.Slab;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameInstaller.Settings GameInstaller;
        public SlabManager.Settings SlabManager;
        public AudioHandler.Settings AudioHandler;

        public override void InstallBindings()
        {
            Container.BindInstance(GameInstaller);
            Container.BindInstance(SlabManager);
            Container.BindInstance(AudioHandler);
        }
    }
}