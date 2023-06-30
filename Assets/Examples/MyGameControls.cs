using Architecture.Injector.Core;
using Architecture.ViewManager;
using MainMenu;

namespace GameName
{
    public class MyGameControls : IGameControls
    {
        public void BackToMenu()
        {
            Injection.Get<IViewManager>().Show<MainMenuView>();
        }
    }
}