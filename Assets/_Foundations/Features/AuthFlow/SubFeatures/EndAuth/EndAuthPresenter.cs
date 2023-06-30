using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using AuthFlow.Terms.Core.Actions;
using Data;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.EndAuth
{
    [UsedImplicitly]
    public class EndAuthPresenter
    {
        readonly IEndAuthScreen view;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly ILocalUserInfo localUserInfo;
        readonly IAboutYouStateManager aboutYouState;
        readonly IViewManager viewManager;
        readonly AreTermsAccepted areTermsAccepted;


        public EndAuthPresenter(IEndAuthScreen view, IAboutYouStateManager aboutYouState, ILocalUserInfo localUserInfo, IViewManager viewManager, AreTermsAccepted areTermsAccepted)
        {
            this.view = view;
            this.localUserInfo = localUserInfo;
            this.aboutYouState = aboutYouState;
            this.viewManager = viewManager;
            this.areTermsAccepted = areTermsAccepted;
            view.OnShowView.Subscribe(Present).AddTo(disposables);
        }

        void Present()
        {
            viewManager.GetOut(GetOutPort());
        }

        string GetOutPort()
        {
          // We ended up here without being logged in somehow .. this should
          // never happen, but I'm putting it in here because if it did it's
          // a catastrophic failure.
          if (FirebaseAuth.DefaultInstance.CurrentUser == null) return "landing-view";

          // View 0
          if (FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified == false) return "verify-email";

          // View 1
          if (localUserInfo["terms"] != "True") return "terms";

          // NOTE(Jesse): This is kinda out-of-band for how the rest of the Auth
          // flow is built, but the 'about-you' view dymically updates itself
          // instead of punting users to a new view.. hence we redirect to the
          // about-you view for all of the below

          // View 2
          if (!aboutYouState.FirstName.HasValue) return "about-you";
          if (!aboutYouState.LastName.HasValue) return "about-you";

          // View 3
          if (!aboutYouState.Gender.HasValue) return "about-you";
          if (!aboutYouState.BirthDate.HasValue) return "about-you";

          // View 4
          if (!aboutYouState.UserName.HasValue) return "about-you";


          // View 5
          IGameData gameData = Injection.Get<IGameData>();
          var user = gameData.GetUserInformation();
          JesseUtils.LoginTracking(user);
          JesseUtils.SessionTracking(user);
          if (gameData.GetUserInformation().Do_avatar_customization) return "avatar-customization";

          return "auth-complete";
        }
    }
}
