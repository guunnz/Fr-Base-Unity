using System;
using System.Collections.Generic;
using DeepLink.Core;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace DeepLink.Infrastructure
{
    [UsedImplicitly]
    public class DeepLinkService : IDeepLinkService
    {
        private readonly IReactiveProperty<DeepLinkInfo> deepLinkInfo = new ReactiveProperty<DeepLinkInfo>();

        private readonly IDeepLinkProcess deepLinkProcess;

        private IObservable<DeepLinkInfo> PublicObservable => deepLinkInfo.Where(info => info != null);

        public DeepLinkService(IDeepLinkProcess deepLinkProcess)
        {
            this.deepLinkProcess = deepLinkProcess;
        }

        private IObservable<DeepLinkInfo> CreateObservable()
        {
            /*
             * if already there is a value to emit, merge the observable with a Return to
             * emit the current value on subscription
             * else, just return the observable, and the value will be emit eventually
             */
            if (deepLinkInfo.HasValue && deepLinkInfo.Value != null)
            {
                return Observable
                    .Return(deepLinkInfo.Value)
                    .Merge(PublicObservable);
            }

            return PublicObservable;
        }

        public IObservable<DeepLinkInfo> OnDeepLink()
        {
            var absoluteURL = Application.absoluteURL;

#if UNITY_EDITOR


            const string dlKey = "stored-deep-link";
            const string flagKey = "use-deep-link";

            if (PlayerPrefs.GetInt(flagKey) == 1)
            {
                absoluteURL = PlayerPrefs.GetString(dlKey);
            }

#endif


            //deep link example: "com.opticpower.friendbase://friendbaseDl?key1=value1&key2=value2&key3=value3"
            if (!string.IsNullOrEmpty(absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link now
                ListenDeepLinkActive(absoluteURL);
            }
            else
            {
                Application.deepLinkActivated -= ListenDeepLinkActive;
                Application.deepLinkActivated += ListenDeepLinkActive;
            }

            return CreateObservable();
        }

        public DeepLinkInfo GetDeepLinkInfo()
        {
#if UNITY_EDITOR


            const string dlKey = "stored-deep-link";
            const string flagKey = "use-deep-link";

            if (PlayerPrefs.GetInt(flagKey) == 1)
            {
                var absoluteURL = PlayerPrefs.GetString(dlKey);
                if (!deepLinkInfo.HasValue || deepLinkInfo.Value == null)
                {
                    ListenDeepLinkActive(absoluteURL);
                }
            }

#endif
            
            return (deepLinkInfo.HasValue ? deepLinkInfo.Value : null) ?? EmptyInfo();
        }

        static DeepLinkInfo EmptyInfo()
        {
            return new DeepLinkInfo(new Dictionary<string, string>());
        }

        private void ListenDeepLinkActive(string url)
        {
            var infos = deepLinkProcess.GetInfo(url);
            var dict = new Dictionary<string, string>();
            foreach (var (key, value) in infos)
            {
                dict[key] = value;
            }

            var potentialInfo = new DeepLinkInfo(dict);
            if (potentialInfo.ValidDeepLink)
            {
                deepLinkInfo.Value = potentialInfo;
            }
        }
    }
}