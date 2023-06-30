using System;
using AuthFlow.Domain;

namespace AuthFlow
{
    public interface ISignWithEmail
    {
        IObservable<UserAuthInfo> SignInUser(string email, string password);
        IObservable<UserAuthInfo> SignUpUser(string email, string password);
        
    }
}