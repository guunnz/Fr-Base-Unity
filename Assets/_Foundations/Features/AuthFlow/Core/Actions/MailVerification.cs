using System;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.Actions
{
    [UsedImplicitly]
    public class MailVerification
    {
        readonly IAuthWebService webService;
        readonly IAuthStateManager stateManager;

        public MailVerification(IAuthWebService webService, IAuthStateManager stateManager)
        {
            this.webService = webService;
            this.stateManager = stateManager;
        }

        public IObservable<bool> ExecuteValidEmail()
        {
            var email = stateManager.Email;
            return webService.EmailExist(email).ObserveOnMainThread();
        }
    }
}