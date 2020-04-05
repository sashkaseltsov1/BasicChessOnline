using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ChessClient.Modules.FindGame
{
    class FindGameModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var manager = containerProvider.Resolve<IRegionManager>();
            manager.RegisterViewWithRegion("MainRegion", typeof(FindGame.Views.FindGame));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) { }
    }
}
