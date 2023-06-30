using System;
using UniRx;
using UnityEngine;

namespace Architecture.Context
{
    /// <summary>
    /// defines a 'regular' module with an Init method
    /// </summary>
    public interface IModule
    {
        void Init();
    }

    /// <summary>
    /// defines a blocker module, a module that requires an async initialization
    /// but needs to wait until it ends to continue initializtion
    /// </summary>
    public interface IBlockerModule
    {
        IObservable<Unit> Init();
    }

    /// <summary>
    /// a 'regular' module that is a MonoBehaviour
    /// the GameObject should live on the scene and being referenced as well
    /// </summary>
    public abstract class ScriptModule : MonoBehaviour, IModule
    {
        public abstract void Init();
    }
}