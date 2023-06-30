using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using UniRx;
using UnityEngine;

namespace Architecture.Context
{
    /// <summary>
    /// For the initialization pipeline, this class is prop. Here, the initializations will actually be executed.
    /// Here are declared the different lists of modules (blocking or regular) -> (IObservable or void)
    /// on the context awake the modules will be initialized
    /// the init order will be:
    /// <see cref="beforeGameModules"/>
    /// <see cref="scriptModulesUnblockable"/>
    /// <see cref="blockingGameModules"/>
    /// <see cref="afterGameModules"/>
    /// <see cref="scriptModulesLockables"/>
    /// </summary>
    /// //
    public class GameContext : MonoBehaviour
    {
        [SerializeField, Tooltip("Were Loaded On The Beginning")]
        List<ScriptModule> scriptModulesUnblockable;

        [SerializeField, Tooltip("Were Loaded On The End")]
        List<ScriptModule> scriptModulesLockables;

        readonly BlockingGameModules blockingGameModules = new BlockingGameModules();
        readonly GameModules beforeGameModules = new BeforeGameModules();
        readonly GameModules afterGameModules = new AfterGameModules();

        readonly LoadingService loadingService = new LoadingService();


        void Awake()
        {
            blockingGameModules.Declare();
            beforeGameModules.Declare();
            afterGameModules.Declare();

            Injection.Register<ILoadingService>(loadingService);

            InitList(beforeGameModules, nameof(beforeGameModules));

            loadingService.BeginLoading(blockingGameModules.Count + 1);

            InitList(scriptModulesUnblockable, nameof(scriptModulesUnblockable));

            InitBlockingModules().ToObservable().Subscribe();
        }

        void InitGameModules()
        {
            InitList(afterGameModules, nameof(afterGameModules));
            InitList(scriptModulesLockables, nameof(scriptModulesLockables));
        }

        void InitList<T>(IList<T> list, string listName) where T : IModule
        {
            var listCount = list.Count;
            for (var i = 0; i < listCount; ++i)
            {
                var module = list[i];
                if (module != null)
                {
                    module.Init();
                }
                else
                {
                    Debug.LogError($"Error -> {listName}[{i}] is null", this);
                }
            }
        }

        IEnumerator InitBlockingModules()
        {
            yield return null;
            foreach (var blockerModule in blockingGameModules)
            {
                loadingService.LoadNewModule(GetModuleName(blockerModule));
                yield return blockerModule.Init().ToYieldInstruction();
                yield return null;
            }

            yield return null;
            loadingService.LoadNewModule("Game Services");
            yield return null;
            InitGameModules();
            loadingService.EndLoading();
        }

        static string GetModuleName(IBlockerModule blockerModule)
        {
            return blockerModule.GetType().Name.Replace("Module", "");
        }
    }
}