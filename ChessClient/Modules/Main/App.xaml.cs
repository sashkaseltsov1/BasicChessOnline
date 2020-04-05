using ChessClient.Main.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;

namespace ChessClient
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) { }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Modules.MainMenu.MainMenuModule>();
            moduleCatalog.AddModule<Modules.FindGame.FindGameModule>();
            moduleCatalog.AddModule<Modules.CreateGame.CreateGameModule>();
            moduleCatalog.AddModule<Modules.Game.GameModule>();
        }
    }
}
