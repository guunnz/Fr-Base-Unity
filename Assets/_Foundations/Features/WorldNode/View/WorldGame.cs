using System;
using System.Collections;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using DeviceInput.Core.Actions;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace WorldNode.View
{
    public class WorldGame : ViewNode, IPauseManager
    {
        [SerializeField] float banishDuration = 1;
        [SerializeField] Image cover;

        [FormerlySerializedAs("quitView")] [SerializeField]
        GameObject pauseView;

        public string scenarioName;

        readonly ISubject<Unit> onEnterPause = new Subject<Unit>();
        readonly ISubject<Unit> onExitPause = new Subject<Unit>();

        protected override void OnInit()
        {
            Injection.Get(out ListenKeyPress _);
        }

        protected override void OnShow()
        {
            Injection.Register<IPauseManager>(this);
            SceneManager.LoadScene(scenarioName, LoadSceneMode.Additive);
            pauseView.SetActive(false);

            // backButton.onClick
            //     .AsObservable()
            //     .Merge(listenKeyPress.Execute(KeyCode.Escape))
            //     .Subscribe(OnPause)
            //     .AddTo(showDisposables);


            SetActiveCover(true)
                .ToObservable()
                .Subscribe()
                .AddTo(showDisposables);
        }

        void OnPause()
        {
            var doPause = pauseView.activeSelf;
            pauseView.SetActive(!doPause);
            var subject = doPause ? onEnterPause : onExitPause;
            
            subject.OnNext(Unit.Default);
            subject.OnCompleted();
        }

        /// <summary>
        /// Banishes the cover to avoid weird things on rendering
        /// while loading the scenario
        /// by default it takes 2 seconds
        /// </summary>
        IEnumerator SetActiveCover(bool active)
        {
            var alphaDestination = active ? 0f : 1f;
            var color = cover.color;

            var initCol = color;
            initCol.a = 1 - alphaDestination;
            var endCol = color;
            endCol.a = alphaDestination;
            var t = 1f;
            cover.gameObject.SetActive(true);
            while (t > 0)
            {
                t -= Time.deltaTime * (1f / banishDuration);
                cover.color = Color.Lerp(endCol, initCol, t);
                yield return null;
            }

            cover.gameObject.SetActive(active);
        }

        protected override void OnHide()
        {
            SceneManager.UnloadSceneAsync(scenarioName);
            pauseView.SetActive(false);
        }

        public bool IsPause
        {
            get => pauseView.activeSelf;
            set => pauseView.SetActive(value);
        }

        public IObservable<Unit> OnEnterPause => onEnterPause;
        public IObservable<Unit> OnExitPause => onExitPause;
    }

    public interface IPauseManager
    {
        bool IsPause { get; set; }
        IObservable<Unit> OnEnterPause { get; }
        IObservable<Unit> OnExitPause { get; }
    }
}