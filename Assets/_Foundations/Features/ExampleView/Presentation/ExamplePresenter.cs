using Architecture.ViewManager;
using ExampleView.View;
using UniRx;

namespace ExampleView.Presentation
{
    public class ExamplePresenter
    {
        private readonly IExampleScreen screen;
        private readonly IViewManager viewManager;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        //we can use more than one disposables
        private readonly CompositeDisposable buttonSubscription = new CompositeDisposable();


        /// <summary>
        /// we can subscribe to the life-time observables because screen interface inherits from <see cref="Architecture.MVP.IPresentable"/>>
        /// </summary>
        public ExamplePresenter(IExampleScreen screen, IViewManager viewManager)
        {
            this.screen = screen;
            this.viewManager = viewManager;
            this.screen.OnShowView.Subscribe(_ => Present()).AddTo(disposables);
            this.screen.OnHideView.Subscribe(_ => Hide()).AddTo(disposables);
            this.screen.OnDisposeView.Subscribe(_ => CleanUp()).AddTo(disposables);
            buttonSubscription
                .AddTo(disposables); // as it works with the composite design pattern, we can make a tree of disposables
        }


        private void Present()
        {
            //the underscore on lambdas is always used to ignore the parameter (when it's one parameter)
            //we use this to ignore the Unit parameter (cause UniRx uses Unit to emulate Void/Parameterless operations)
            screen.OnGoToNextView.Subscribe(_ => MoveToNextView()).AddTo(buttonSubscription);
        }

        private void MoveToNextView()
        {
            //using the edge label we can move towards the views graph from our node to another
            //viewManager.GetOut("next-view");
            viewManager.Show<ExamplePopup>();


        }

        private void Hide()
        {
            //remove the button subscription
            buttonSubscription.Clear();
        }

        private void CleanUp()
        {
            //cleanup every subscription
            disposables.Clear();
        }
    }
}