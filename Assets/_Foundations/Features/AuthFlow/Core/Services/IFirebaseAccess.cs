using System;
using Firebase;

namespace AuthFlow.Core.Services
{
    public interface IFirebaseAccess
    {
        IObservable<FirebaseApp> App { get; }
    }
}