using Architecture.Context;
using Architecture.Injector.Core;
using LoadingScreen;

namespace Architecture.ViewManager
{
    public class ViewFirstFlow : ScriptModule
    {
        public override void Init()
        {
            Injection.Get<IViewManager>().Show<LoadingScreenView>();
        }
        
        
    }
}