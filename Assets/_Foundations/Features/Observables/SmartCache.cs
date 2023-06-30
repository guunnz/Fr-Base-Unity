using System;
using System.Collections.Generic;
using UniRx;

namespace Observables
{
    public delegate IObservable<TValue> RetrieveHeavy<in TKey, out TValue>(TKey key);

    public interface IObservableValue<T> : IObservable<T>
    {
        void SetValue(T t);
    }

    public class ObservableValue<T> : IObservableValue<T>
    {
        IObservable<T> observable = new Subject<T>();


        public void SetValue(T value)
        {
            if (observable is ISubject<T> subject)
            {
                subject.OnNext(value);
                observable = Observable.Empty(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return observable.Subscribe(observer);
        }
    }


    public class SmartCache<TKey, TValue> : ISmartCache<TKey, TValue>
    {
        readonly RetrieveHeavy<TKey, TValue> heavyMethod;


        readonly IDictionary<TKey, IObservableValue<TValue>> cache = new Dictionary<TKey, IObservableValue<TValue>>();

        readonly ICollection<IDisposable> disposables = new CompositeDisposable();


        public SmartCache(RetrieveHeavy<TKey, TValue> heavyMethod)
        {
            this.heavyMethod = heavyMethod;
        }

        IObservable<TValue> Get(TKey key)
        {
            if (cache.TryGetValue(key, out var observable))
            {
                return observable;
            }

            var cacheObservable = new ObservableValue<TValue>();
            cache[key] = cacheObservable;

            heavyMethod(key)
                .Subscribe(cacheObservable.SetValue)
                .AddTo(disposables);

            return cacheObservable;
        }

        public IObservable<TValue> this[TKey key] => Get(key);

        public void Dispose()
        {
            disposables.Clear();
        }
    }

    public interface ISmartCache<in TKey, out TValue> : IDisposable
    {
        IObservable<TValue> this[TKey key] { get; }
    }

    public static class SmartCacheFactory
    {
        public static ISmartCache<TKey, TValue> SmartCache<TKey, TValue>( this RetrieveHeavy<TKey, TValue> heavyMethod)
        {
            return new SmartCache<TKey, TValue>(heavyMethod);
        }
    }
}