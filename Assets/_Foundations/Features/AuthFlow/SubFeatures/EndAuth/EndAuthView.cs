using Architecture.Injector.Core;
using Architecture.ViewManager;

namespace AuthFlow.EndAuth
{
    public class EndAuthView : ViewNode, IEndAuthScreen
    {
        protected override void OnInit()
        {
            this.CreatePresenter<EndAuthPresenter, IEndAuthScreen>();
        }

        
    }
}