using ChessClient.Modules.EventAgregator;
using ChessClient.Modules.FindGame.ViewModels;
using Prism.Events;
using Prism.Regions;

namespace ChessClient.Modules.Game.ViewModels
{
    class GameViewModel: INavigationAware
    {
        private readonly IEventAggregator _ea;

        public GameViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var game = navigationContext.Parameters["CurrentGame"] as GameDesc;
            _ea.GetEvent<GameDescriptionSentEvent>().Publish(game);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
