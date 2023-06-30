using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using ExampleView.Presentation;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/*
 * The view component, that holds all the Unity references must live
 * on a View directory inside its feature dir
 * Features/MyFeature/View
 * if this is an specific game feature use __GameName/Features/MyGameFeature/View
 * but, if we are using an cross-game feature it can be stored on foundations to be consumed
 * on future games : _Foundations/Features/MyGeneralFeature/View
 */
namespace ExampleView.View
{
    /// <summary>
    /// To create a new view in the system, you need to inherit from <see cref="ViewNode"/> 
    /// </summary>
    public class ExampleView : ViewNode, IExampleScreen //<--- implement the interface that presentation provides
    {
        ///<summary>
        ///an unity UI button, we can liston on those click events         
        ///</summary>
        [SerializeField] private Button goNextView;

        /// <summary>
        /// <see cref="UniRx"/> provides us an implementation
        /// of the event subject to call on next and subscribe when required
        /// </summary>
        private readonly ISubject<Unit> onGoToNextView = new Subject<Unit>();

        /// <summary>
        /// we can store all disposables on a single structure to make just one Clear
        /// and forget about memory leaks
        /// </summary>
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        protected override void OnInit()
        {
            //creating a presenter sending self as the view, and then using the Dependency Manager send the requested dependencies
            var presenter = new ExamplePresenter(this, Injection.Get<IViewManager>());
            
        }


        /// <summary>
        /// View Manager provides us a way to know when a node is shown by
        /// override the <see cref="ViewNode.OnShow"/> method
        /// </summary>
        protected override void OnShow()
        {
            goNextView
                .OnClickAsObservable()
                .Subscribe(onGoToNextView) // ISubject work as an Observer and as an Observable 😉
                .AddTo(disposables);
        }

        protected override void OnHide() => disposables.Clear();
        protected override void OnDispose() => disposables.Clear();
        public IObservable<Unit> OnGoToNextView => onGoToNextView;
    }
}