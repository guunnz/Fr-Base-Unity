using System;
using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.AboutYou;
using AuthFlow.Actions;
using AuthFlow.AppleLogin;
using AuthFlow.Core.Services;
using AuthFlow.EndAuth.Repo;
using AuthFlow.FacebookLogin;
using AuthFlow.Firebase;
using AuthFlow.ForgotPass;
using AuthFlow.Infrastructure;
using AuthFlow.SetNewPass;
using AuthFlow.Terms.Core.Actions;
using AuthFlow.Terms.Core.Services;
using AuthFlow.Terms.Infrastructure;
using AuthFlow.VerifyEmail;
using UniRx;

namespace AuthFlow
{
    public class AuthFlowModule : IBlockerModule
    { readonly IModule[] subModules =
        {
            new FirebaseModule(),
            new AboutYouModule(),
            new ForgotPassModule(),
            new SetNewPassModule(),
            new VerifyEmailModule(),
            new AppleLoginModule()
        };

        public IObservable<Unit> Init()
        {
            Injection.Register<IAuthStateManager, AuthStateManager>();
            Injection.Register<IAuthWebService, AuthWebService>();


            Injection.Register<IPasswordValidator, PasswordValidator>();
            Injection.Register<IUserAuthRepo, LocalUserAuthRepo>();
            Injection.Register<IUserPassword, SignWithEmail>();
            Injection.Register<ISignWithEmail, SignWithEmail>();

            Injection.Register<IFirebaseAccess, FirebaseAccess>();
            Injection.Register<IFirebaseEventBus, FirebaseAuthStateChange>();


            Injection.Register<ILocalUserInfo, LocalUserInfo>();

            //terms and conds   
            Injection.Register<ITermsWebClient, TermsWebClient>();
            Injection.Register<ITermsService, TermsService>();


            //actions
            Injection.Register<MailVerification>();
            Injection.Register<CheckCookieCredentials>();
            Injection.Register<MarkAsLoggedIn>();

            Injection.Register<AreTermsAccepted>();
            Injection.Register<SetTermsAccepted>();

            foreach (var subModule in subModules)
            {
                subModule.Init();
            }

            return Observable.ReturnUnit();
            //return Injection.Get<IFirebaseAccess>().App.AsUnitObservable();
        }
    }
}