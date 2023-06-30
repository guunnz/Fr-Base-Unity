using System;
using System.Collections;
using UniRx;
using UnityEngine.Networking;

namespace Web
{
    public static class RequestExtension
    {
        public static IObservable<UnityWebRequest> ObserveWebRequest(this UnityWebRequest req)
        {
            IEnumerator RequestRoutine()
            {
                yield return req.SendWebRequest();
            }
            return RequestRoutine().ToObservable().Select(_ => req);
        }
    }
}