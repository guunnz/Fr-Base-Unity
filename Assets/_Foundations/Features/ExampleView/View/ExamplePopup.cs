using Architecture.Injector.Core;
using Architecture.ViewManager;
using TMPro;
using Tools.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ExampleView.View
{
    public class ExamplePopup : ViewNode
    {
        public Button button;
        public TextMeshProUGUI label;

        public RectTransform content;

        private readonly CompositeDisposable disposables = new CompositeDisposable();


        private Vector2[] directions = new[]
        {
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, 1),
        };

        protected override void OnShow()
        {
            var vm = Injection.Get<IViewManager>();

            var blackboard = Parameters.AsBlackboard();

            if (blackboard.TryGet<string>(out var text))
            {
                label.text = text;
            }

            if (blackboard.TryGet<Vector3>(out var pos))
            {
                content.position = pos;
            }

            int counter = 0;

            if (blackboard.TryGet<int>(out var c))
            {
                counter = c;
            }

            button.OnClickAsObservable().Subscribe(_ =>
            {
                var div = counter / 5;
                var index = div % 4;
                var dir = directions[index];

                var newPos = content.position + (Vector3) (dir * new Vector2(25, 25));
                var newLabel = label.text + " +1";

                vm.ShowModal<ExamplePopup>(newLabel, newPos, counter + 1);
            }).AddTo(disposables);
        }

        protected override void OnHide()
        {
            disposables.Clear();
        }
    }
}