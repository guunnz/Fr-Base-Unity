using Architecture.Injector.Core;
using Audio.SFX;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Audio.Components
{
    [RequireComponent(typeof(Button))]
    public class SfxButton : MonoBehaviour
    {
        public string sfxKey;

        [Range(0, 1)] public float vol;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            var button = GetComponent<Button>();
            button
                .OnClickAsObservable()
                .Do(_ => Injection.Get<ISfxPlayer>().Play(sfxKey, vol))
                .Subscribe()
                .AddTo(disposables);
        }
    }
}