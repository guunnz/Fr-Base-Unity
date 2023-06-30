using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace FeatureToggle
{
    [UsedImplicitly]
    public class LocalFeatureService : IFeatureService
    {
        private const string FeaturesFileName = "FeatureFlags";
        private readonly Dictionary<string, ISubject<bool>> subjects = new Dictionary<string, ISubject<bool>>();
        private ScriptableFeatureService cloudWebAsset;
        private Dictionary<string, bool> dict; //null if not ready


        public LocalFeatureService()
        {
            LoadInfo();
        }

        public IObservable<bool> this[string featureName] => ReadValue(featureName);

        private void LoadInfo()
        {
            LoadFile();
            //todo: update webAsset
            ReadAsset();
        }

        private void ReadAsset()
        {
            dict = new Dictionary<string, bool>();
            foreach (var infoEnableFeature in cloudWebAsset.info.enableFeatures) dict[infoEnableFeature] = true;
            foreach (var pair in subjects)
            {
                pair.Value.OnNext(FeatureTrueOnDict(pair.Key));
                pair.Value.OnCompleted();
            }

            subjects.Clear();
        }

        private void LoadFile()
        {
            cloudWebAsset = Resources.Load<ScriptableFeatureService>(FeaturesFileName);
        }

        private IObservable<bool> ReadValue(string featureName)
        {
            if (dict != null) return Observable.Return(FeatureTrueOnDict(featureName));

            if (!subjects.TryGetValue(featureName, out var subject))
            {
                subject = new Subject<bool>();
                subjects[featureName] = subject;
            }

            return subject;
        }

        private bool FeatureTrueOnDict(string featureName)
        {
            return dict.TryGetValue(featureName, out var value) && value;
        }
    }
}