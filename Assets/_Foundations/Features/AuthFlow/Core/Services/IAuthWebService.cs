using System;

namespace AuthFlow
{
    public interface IAuthWebService
    {
        IObservable<bool> EmailExist(string email);
    }
}