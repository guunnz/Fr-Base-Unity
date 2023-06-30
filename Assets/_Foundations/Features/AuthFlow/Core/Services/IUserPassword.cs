using System;
using UniRx;

namespace AuthFlow
{
    public interface IUserPassword
    {
        IObservable<Unit> ResetPassword(string email);
        IObservable<Unit> UpdatePassword(string newPassword);
    }
}