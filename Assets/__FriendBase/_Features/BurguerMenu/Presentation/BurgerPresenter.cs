using AuthFlow;
using BurguerMenu.Core.Domain;
using JetBrains.Annotations;
using UniRx;

namespace BurguerMenu.Presentation
{
    [UsedImplicitly]
    public class BurgerPresenter
    {
        readonly IBurguerView view;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable sectionDisposables = new CompositeDisposable();

        public BurgerPresenter(IBurguerView view)
        {
            this.view = view;

            sectionDisposables.AddTo(disposables);

            this.view.OnHideView.Subscribe(Hide).AddTo(disposables);
            this.view.OnDisposeView.Subscribe(Dispose).AddTo(disposables);

            view.OnEnabled.Subscribe(Present).AddTo(disposables);
            view.OnDisabled.Subscribe(Hide).AddTo(disposables);
        }

        void Present()
        {
            view.OnHelpButton.Subscribe(PresentHelp).AddTo(sectionDisposables);
            view.OnEditAccountButton.Subscribe(PresentEditAccount).AddTo(sectionDisposables);
            view.OnLogOutButton
                .Do(Hide)
                .Subscribe(LogOut).AddTo(sectionDisposables);

            view.OnLogOutGuestButton
                 .Do(Hide)
                 .Subscribe(LogOut).AddTo(sectionDisposables);
            view.ShowSection(BurgerSection.Menu);
        }

        private void PresentHelp()
        {
            view.OnSupportButton.Subscribe(PresentSupport).AddTo(sectionDisposables);
            view.OnTermsButton.Subscribe(PresentTerms).AddTo(sectionDisposables);
            view.OnCodeOfConductButton.Subscribe(PresentConduct).AddTo(sectionDisposables);
            view.OnPrivacyPolicyButton.Subscribe(PresentPrivacy).AddTo(sectionDisposables);

            view.ShowSection(BurgerSection.Help);
        }

        private void PresentEditAccount()
        {
            view.ShowSection(BurgerSection.EditAccount);
        }

        private void PresentSupport()
        {
            view.VisitExternalSectionUrl(HelpSections.Support);
        }

        void PresentTerms()
        {
            view.VisitExternalSectionUrl(HelpSections.TermsAndConditions);
        }

        void PresentConduct()
        {
            view.VisitExternalSectionUrl(HelpSections.CodeOfConduct);
        }

        void PresentPrivacy()
        {
            view.VisitExternalSectionUrl(HelpSections.PrivacyPolicy);
        }

        void LogOut()
        {
            CurrentRoom.Instance.DestroyRoom();
            JesseUtils.Logout();
        }

        void Hide()
        {

            view.HideSections();
            sectionDisposables.Clear();
        }

        void Dispose()
        {
            disposables.Clear();
        }
    }
}