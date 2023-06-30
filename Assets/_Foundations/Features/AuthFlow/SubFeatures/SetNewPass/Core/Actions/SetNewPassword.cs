using System;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.SetNewPass.Core.Actions
{
    [UsedImplicitly]
    public class SetNewPassword
    {
        readonly IUserPassword userPass;

        public SetNewPassword(IUserPassword userPass)
        {
            this.userPass = userPass;
        }


        public IObservable<Unit> Execute(string newPass)
        {
            return userPass.UpdatePassword(newPass);
        }
    }
}