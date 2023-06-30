using System;
using System.Collections;
using Architecture.Context;
using AuthFlow;
using Firebase;
using Firebase.Auth;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace ConnectingToServer
{
    public class ConnectingToServerModule : IBlockerModule
    {
        const string restTestEndpoint = "https://friendbase-staging.fly.dev/";

        public IObservable<Unit> Init()
        {
            return TestInternetConnection().ToObservable().First().ObserveOnMainThread();
        }

        IEnumerator TestInternetConnection()
        {
            
            // yield return new WaitForSeconds(2); // testing

            const int attempts = 20;
            for (int i = 0; i < attempts; i++)
            {
                var webRequest = UnityWebRequest.Get(restTestEndpoint);
                var req = webRequest.SendWebRequest();
                while (!req.isDone)
                {
                    yield return null;
                }

                Debug.Log(webRequest.result + " <<<<<<<<<<-------- web request result");

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    yield break;
                }

                yield return new WaitForSeconds(1);
            }
            
            // if you are here -> then you need to nuke
            JesseUtils.Nuke();
            
        }
    }
}