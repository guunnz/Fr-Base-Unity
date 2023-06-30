using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using DeepLink.Core;
using Shared.Utils;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DeepLink.Delivery
{
    public class DeepLinkTestView : ViewNode
    {
        public RectTransform container;
        public StringWidget stringWidget;
        public Button refresh;
        private IDeepLinkService deepLinkService;

        private readonly CompositeDisposable disposables = new CompositeDisposable();


        protected override void OnInit()
        {
            Injection.Get(out deepLinkService);
        }

        protected override void OnShow()
        {
            PrintLines(new[] {"Waiting For Deep Link"});
            FetchDeepLink();
            refresh.OnClickAsObservable().Subscribe(_ =>
            {
                Debug.Log("A");
                disposables.Clear();
                PrintLines(new[] {"Refreshing...", Application.absoluteURL});
                FetchDeepLink();
            });
        }

        protected override void OnHide()
        {
            disposables.Clear();
        }

        private void FetchDeepLink()
        {
            deepLinkService
                .OnDeepLink()
                .Subscribe(ReadDeepLink)
                .AddTo(disposables);
        }

        private void ReadDeepLink(DeepLinkInfo info)
        {
            container.gameObject.DestroyChildren();
            stringWidget.Instantiate("<<Deep Link Info>>", container);
            PrintLines(GetLines(info));
        }

        private IEnumerable<string> GetLines(DeepLinkInfo info)
        {
            var infoDeepLinkEntries = info.deepLinkEntries;
            yield return "<<Deep Link Info>>";
            var pairs = infoDeepLinkEntries.ToList();
            foreach (var label in pairs.Select(pair => $"{pair.Key} = {pair.Value}"))
            {
                yield return label;
            }
        }

        private void PrintLines(IEnumerable<string> lines)
        {
            container.gameObject.DestroyChildren();
            foreach (var line in lines)
            {
                stringWidget.Instantiate(line, container).gameObject.SetActive(true);
            }
        }
    }
}