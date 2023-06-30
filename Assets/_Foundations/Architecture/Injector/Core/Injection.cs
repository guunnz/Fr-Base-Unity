using System;
using UnityEngine;

namespace Architecture.Injector.Core
{
    /// <summary>
    /// an static dependencies injector for the application
    /// </summary>
    public static class Injection
    {
        /// <summary>
        /// holds the actual instance of the injector
        /// </summary>
        static readonly ConcreteInjector Injector = new ConcreteInjector();

        /// <summary>
        /// Retrieves an instance of type <see cref="T"/>
        /// if an instance is already cached it will be returned, else, will try to create one
        /// using it's largest constructor that each parameter, recursively, could be injected too.
        /// Also is a requirement that the type was already registered using any overload of the method
        /// <see cref="Register"/>
        /// </summary>
        /// <typeparam name="T">the type of the instance to be retrieved</typeparam>
        /// <returns> an instance of type <see cref="T"/> </returns>
        public static T Get<T>() where T : class
        {
            return Injector.Get<T>();
        }

        /// <summary>
        /// Like on <see cref="Get{T}()"/> but using an out parameter
        /// (useful to avoid calling with generics)
        /// i.e:
        /// 
        /// IMyDependency dependency;
        /// ...
        /// Injection.Get(out dependency);
        /// 
        /// </summary>
        public static void Get<T>(out T t) where T : class
        {
            t = Injector.Get<T>();
        }

        /// <summary>
        /// Like on <see cref="Get{T}(out T)"/>
        /// but with a ref parameter and only inits it if the parameter is null
        /// </summary>
        public static void SafeGet<T>(ref T t) where T : class
        {
            t ??= Injector.Get<T>();
        }

        /// <summary>
        /// Bind an interface with an implementation
        /// so when you call <see cref="Get{T}()"/> with <see cref="TInterface"/>
        /// you will get an instance of <see cref="TImplementation"/>
        /// <see cref="TImplementation"/> must implement <see cref="TInterface"/>
        /// </summary>
        /// <returns>
        /// returns a disposable object, its disposal will remove the binding and erase
        /// any cache by calling <see cref="Clear{T}"/>> with <see cref="TInterface"/>
        /// </returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IDisposable Register<TInterface, TImplementation>() where TImplementation : class, TInterface
            where TInterface : class
        {
            Injector.Register<TInterface, TImplementation>();
            return new RegistrationDisposable<TInterface>();
        }

        /// <summary>
        /// Bind an type with an instance
        /// so when you call <see cref="Get{T}()"/> with <see cref="T"/>
        /// you will get <see cref="instance"/> instance
        /// </summary>
        /// <returns>
        /// returns a disposable object, its disposal will remove the binding and erase
        /// any cache by calling <see cref="Clear{T}"/>> with <see cref="T"/>
        /// </returns>
        public static IDisposable Register<T>(T instance) where T : class
        {
            Injector.Register(instance);
            return new RegistrationDisposable<T>();
        }

        /// <summary>
        /// Regiuster a class type on the injector
        /// so when you call <see cref="Get{T}()"/> with <see cref="TClass"/>
        /// you will get an instance of this class (to be created or using a cache)
        /// </summary>
        /// <returns>
        /// returns a disposable object, its disposal will remove the binding and erase
        /// any cache by calling <see cref="Clear{T}"/>> with <see cref="TClass"/>
        /// </returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IDisposable Register<TClass>() where TClass : class
        {
            Injector.Register<TClass>();
            return new RegistrationDisposable<TClass>();
        }

        public static void Clear<T>() where T : class
        {
            Injector.Clear<T>();
        }

        public static T Create<T>() where T : class
        {
            Injector.Register<T>();
            return Injector.Get<T>();
        }

        /// <summary>
        /// a shorthand to create a presenter
        /// using the injector, but clearing then the cache so you can use multiple instances
        /// and a new presenter is created every time you call the method
        /// </summary>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static TPresenter CreatePresenter<TPresenter, TView>(this TView viewInstance)
            where TView : class where TPresenter : class
        {
            //register presenter and view
            Injector.Register(viewInstance);
            Injector.Register<TPresenter>();
            //use injector to create presenter
            var presenter = Injector.Get<TPresenter>();
            if (presenter == null)
            {
                Debug.LogError($"Fails trying to create presenter {typeof(TPresenter).Name}");
            }
            //dispose view and presenter bindings so we can have multiple instances
            Injector.Clear<TView>(false);
            Injector.Clear<TPresenter>(false);
            //return the presenter instance
            return presenter;
        }

        /// <summary>
        /// ||big nuke||
        /// it clears all the dependencies, calls Dispose() on each IDisposable dependency
        /// and removes the cache
        /// </summary>
        public static void ClearAll()
        {
            Injector.ClearAll();
        }

        class RegistrationDisposable<T> : IDisposable where T : class
        {
            public void Dispose()
            {
                Clear<T>();
            }
        }
    }
}