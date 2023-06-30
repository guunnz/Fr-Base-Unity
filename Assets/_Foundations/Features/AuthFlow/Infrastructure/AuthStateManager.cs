using JetBrains.Annotations;
using LocalStorage.Core;

namespace AuthFlow.Infrastructure
{
    [UsedImplicitly]
    public class AuthStateManager : IAuthStateManager
    {
        ILocalStorage localStorage;

        public AuthStateManager(ILocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        public string Email
        {
            get => localStorage.GetString("stored-email");
            set => localStorage.SetString("stored-email", value);
        }

        public string Password { get; set; }
    }
}