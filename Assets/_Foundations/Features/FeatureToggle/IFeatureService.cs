using System;

namespace FeatureToggle
{
    public interface IFeatureService
    {
        IObservable<bool> this[string featureName] { get; }
    }
}