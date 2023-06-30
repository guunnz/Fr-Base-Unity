using Architecture.ViewManager;
using UniRx;

namespace ExampleView.Presentation
{
    public class NavigationButtonWidgetPresenter
    {
        private readonly INavigationButtonWidget view;
        private readonly IViewManager viewManager;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly CompositeDisposable buttonDisposable = new CompositeDisposable();

        public NavigationButtonWidgetPresenter(INavigationButtonWidget view, IViewManager viewManager)
        {
            this.view = view;
            this.viewManager = viewManager;
            this
                .view
                .OnShowView
                .Subscribe(_ => Present())
                .AddTo(disposables);
            this
                .view
                .OnHideView
                .Subscribe(_ => Hide())
                .AddTo(disposables);
            this
                .view
                .OnDisposeView
                .Subscribe(_ => CleanUp())
                .AddTo(disposables);
        }

        private void Present()
        {
            view
                .OnClick
                .Do(_ => viewManager.GetOut(view.OutPort))
                .Subscribe()
                .AddTo(buttonDisposable);
        }

        private void Hide()
        {
            buttonDisposable.Clear();
        }

        private void CleanUp()
        {
            disposables.Clear();
        }
    }
}