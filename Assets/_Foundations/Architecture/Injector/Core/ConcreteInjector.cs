using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Architecture.Injector.Core
{
    public class ConcreteInjector : IDependenciesInjector
    {
        static readonly object[] EmptyExtras = new object[0];
        readonly Dictionary<Type, Type> implementations = new Dictionary<Type, Type>();
        readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();

        object[] externalDependencies = EmptyExtras;

        public T Get<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        public T ResolveWith<T>(params object[] extraDependencies) where T : class
        {
            externalDependencies = extraDependencies;
            var dependency = Resolve(typeof(T)) as T;
            externalDependencies = EmptyExtras;
            return dependency;
        }

        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            implementations[typeof(TInterface)] = typeof(TImplementation);
        }

        public void Register<T>(T instance) where T : class
        {
            implementations[typeof(T)] = typeof(T);
            instances[typeof(T)] = instance;
        }

        public void Register<TClass>() where TClass : class
        {
            implementations[typeof(TClass)] = typeof(TClass);
        }

        

        public void Clear<T>(bool dispose = true) where T : class
        {
            if (dispose && instances.TryGetValue(typeof(T), out var instance) && instance is IDisposable disposable)
            {
                disposable.Dispose();
            }

            implementations.Remove(typeof(T));
            instances.Remove(typeof(T));
        }


        public void ClearAll()
        {
            instances.Values.OfType<IDisposable>().ToList().ForEach(d => d.Dispose());
            implementations.Clear();
            instances.Clear();
        }

        bool TryGetInstance(Type type, out object instance)
        {
            var hasInstance = instances.TryGetValue(type, out instance);
            if (hasInstance) return true;

            foreach (var extraDependency in externalDependencies)
                if (extraDependency.GetType() == type)
                {
                    instance = extraDependency;
                    return true;
                }

            return false;
        }

        object Resolve(Type type)
        {
            if (TryGetInstance(type, out var instance)) return instance;

            if (!IsRegistered(type)) return null;

            var constructor = GetBestConstructor(type);
            if (constructor == null) return null;

            instance = InvokeConstructor(constructor);
            instances[type] = instance;
            return instance;
        }

        object InvokeConstructor(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            var arguments = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                arguments[i] = Resolve(parameter.ParameterType);
            }

            return constructor.Invoke(arguments);
        }

        ConstructorInfo GetBestConstructor(Type lowType)
        {
            if (!implementations.TryGetValue(lowType, out var type)) type = lowType;

            var constructors = type.GetConstructors();

            var maxParams = -1;
            ConstructorInfo maxConstructor = null;

            foreach (var constructor in constructors)
            {
                if (!CanInitialize(constructor)) continue;

                if (constructor.GetParameters().Length > maxParams)
                {
                    maxConstructor = constructor;
                    maxParams = constructor.GetParameters().Length;
                }
            }

            return maxConstructor;
        }

        bool CanInitialize(ConstructorInfo constructor)
        {
            foreach (var t in constructor.GetParameters())
                if (!IsRegistered(t.ParameterType))
                    return false;

            return true;
        }

        bool IsRegistered(Type type)
        {
            return implementations.ContainsKey(type);
        }

        bool HasAttribute<T>(MemberInfo info) where T : Attribute
        {
            return info.GetCustomAttribute<T>() != null;
        }
    }
}