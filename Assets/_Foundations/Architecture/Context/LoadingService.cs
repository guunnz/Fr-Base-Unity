using System;
using UniRx;
using UnityEngine;

namespace Architecture.Context
{
    /// <summary>
    /// Manages the loading screen
    /// </summary>
    public class LoadingService : ILoadingService
    {
        private readonly ISubject<string> onNewModuleStartLoading = new Subject<string>();
        private readonly ISubject<Unit> onLoadModules = new Subject<Unit>();
        public int ModulesCount { get; private set; }
        public IObservable<string> OnNewModuleStartLoading => onNewModuleStartLoading;
        public IObservable<Unit> OnLoadModules => onLoadModules;

        public void LoadNewModule(string name)
        {
//            Debug.Log("load new module : " + name);
            onNewModuleStartLoading.OnNext(name);
        }

        public void EndLoading()
        {
            Debug.Log("load new module COMPLETED ");
            onNewModuleStartLoading.OnCompleted();
            onLoadModules.OnNext(Unit.Default);
        }

        public void BeginLoading(int modulesCount)
        {
            ModulesCount = modulesCount;
        }
    }

    public interface ILoadingService
    {
        int ModulesCount { get; }
        IObservable<string> OnNewModuleStartLoading { get; }
        IObservable<Unit> OnLoadModules { get; }


    }
}