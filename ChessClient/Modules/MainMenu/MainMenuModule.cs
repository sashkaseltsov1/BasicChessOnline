using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ChessClient.Modules.MainMenu
{
    class MainMenuModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var manager = containerProvider.Resolve<IRegionManager>();
            manager.RegisterViewWithRegion("MainRegion", typeof(MainMenu.Views.MainMenu));
            manager.RegisterViewWithRegion("LeftRegion", typeof(MainMenu.LeftArea.Views.Menu));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainMenu.LeftArea.Views.Menu>();
            containerRegistry.RegisterForNavigation<MainMenu.LeftArea.Views.Signup>();
            containerRegistry.RegisterForNavigation<MainMenu.LeftArea.Views.Login>();
            containerRegistry.RegisterForNavigation<MainMenu.LeftArea.Views.Profile>();
        }
    }
}
