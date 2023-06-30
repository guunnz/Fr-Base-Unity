using System;
using JetBrains.Annotations;
using UniRx;
using User.Core.Actions;

namespace AuthFlow.SetNewPass.Core.Actions
{
    [UsedImplicitly]
    public class LoginWithNewPass
    {
        readonly ISignWithEmail signWithEmail;
        readonly GetUserInfo getUserInfo;

        public LoginWithNewPass(ISignWithEmail signWithEmail, GetUserInfo getUserInfo)
        {
            this.signWithEmail = signWithEmail;
            this.getUserInfo = getUserInfo;
        }

        public IObservable<Unit> Execute(string newPass)
        {
            return getUserInfo.Execute()
                .Select(info => info.email)
                .SelectMany(mail => signWithEmail.SignInUser(mail, newPass))
                .AsUnitObservable();
        }
    }
}