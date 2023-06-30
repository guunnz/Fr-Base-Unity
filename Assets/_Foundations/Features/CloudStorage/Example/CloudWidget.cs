using System;
using Architecture.Injector.Core;
using CloudStorage.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CloudStorage.Example
{
    [Serializable]
    public struct ExampleData1
    {
        public string information;

        public ExampleData1(string information)
        {
            this.information = information;
        }

        public override string ToString()
        {
            return information;
        }
    }

    [Serializable]
    public struct ExampleData2
    {
        public string info;

        public ExampleData2(string info)
        {
            this.info = info;
        }

        public override string ToString()
        {
            return info;
        }
    }

    public class CloudWidget : MonoBehaviour
    {
        public TMP_InputField key1;
        public TMP_InputField value1;
        public Button save;
        public Button load;

        private ICloudStorage cloudStorage;


        private void Start()
        {
            cloudStorage = Injection.Get<ICloudStorage>();
            save.OnClickAsObservable().Subscribe(_ => Save());
            load.OnClickAsObservable().Subscribe(_ => Load());
        }

        private void Save()
        {
            if (!enabled) return;
            enabled = false;
            cloudStorage
                .Save(new ExampleData1(value1.text), key1.text)
                .Subscribe(_ => enabled = true, WatchError);
        }

        private void WatchError(Exception exception)
        {
            Debug.LogError(exception.Message);
        }

        private void Load()
        {
            if (!enabled) return;
            enabled = false;
            cloudStorage
                .Fetch<ExampleData1>(key1.text)
                .Do(ed => value1.text = ed.ToString())
                .Subscribe(_ => enabled = true);
        }
    }
}