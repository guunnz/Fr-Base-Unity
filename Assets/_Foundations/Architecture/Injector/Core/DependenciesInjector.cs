namespace Architecture.Injector.Core
{
    public interface IDependenciesInjector
    {
        T Get<T>() where T : class;
        T ResolveWith<T>(params object[] extraDependencies) where T : class;
        void Register<TInterface, TImplementation>() where TImplementation : class, TInterface;
        void Register<T>(T instance) where T : class;
        void Register<TClass>() where TClass : class;
        void Clear<T>(bool dispose = true) where T : class;
        void ClearAll();
    }
}