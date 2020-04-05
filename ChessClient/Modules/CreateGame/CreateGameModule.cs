using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ChessClient.Modules.CreateGame
{
    class CreateGameModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var manager = containerProvider.Resolve<IRegionManager>();
            manager.RegisterViewWithRegion("MainRegion", typeof(CreateGame.Views.CreateGame));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) { }
    }
}
