using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ChessClient.Modules.Game
{
    class GameModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var manager = containerProvider.Resolve<IRegionManager>();
            manager.RegisterViewWithRegion("MainRegion", typeof(Game.Views.Game));
            manager.RegisterViewWithRegion("ChessBoardRegion", typeof(Game.Views.ChessBoard));
            manager.RegisterViewWithRegion("GameDescriptionRegion", typeof(Game.Views.Description));
            manager.RegisterViewWithRegion("ChatRegion", typeof(Game.Views.Chat));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) { }
    }
}
