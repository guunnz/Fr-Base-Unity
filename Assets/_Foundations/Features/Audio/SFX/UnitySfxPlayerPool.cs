using System.Collections.Generic;
using Audio.Core;
using UniRx;
using UnityEngine;

namespace Audio.SFX
{
    public class UnitySfxPlayerPool : MonoBehaviour
    {
        [SerializeField] private int initialPoolSize = 5;

        [SerializeField] private int maxPoolSize = 50;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly HashSet<VolumeControl> onUse = new HashSet<VolumeControl>();

        private readonly Queue<VolumeControl> pool = new Queue<VolumeControl>();

        private List<VolumeControl> volumeControlList = new List<VolumeControl>();

        private void Awake()
        {
            FillPool();
        }

        private void OnDestroy()
        {
            disposables.Clear();
            pool.Clear();
            onUse.Clear();
        }

        public void MakeItPlay(VolumeControl control)
        {
            onUse.Add(control);
            control.Play();
            control
                .DoPlay()
                .Do(_ => onUse.Remove(control))
                .Do(OnFree)
                .Subscribe()
                .AddTo(disposables);
        }

        private void OnFree(VolumeControl control)
        {
            control.Stop();
            control.Vol = 1;
            control.gameObject.SetActive(false);
            if (pool.Count + onUse.Count > maxPoolSize)
                Destroy(control.gameObject);
            else
                pool.Enqueue(control);
        }

        private VolumeControl GetControl()
        {
            if (pool.Count < 3) FillPool();

            var control = pool.Dequeue();
            control.gameObject.SetActive(true);
            return control;
        }

        private void FillPool()
        {
            for (var i = 0; i < initialPoolSize; ++i) pool.Enqueue(CreatePlayer());
        }

        private VolumeControl CreatePlayer()
        {
            var go = new GameObject("audio_source");
            go.transform.SetParent(transform);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            var volumeControl = go.AddComponent<SfxVolumeControl>();
            volumeControl.audioSource = source;
            volumeControlList.Add(volumeControl);

            return volumeControl;
        }

        public void Play(AudioClip clip, float vol)
        {
            var control = GetControl();
            control.Vol = vol;
            control.Stop();
            control.gameObject.SetActive(true);
            control.SetClip(clip);
            MakeItPlay(control);
        }

        public List<VolumeControl> GetVolumeControlList()
        {
            return volumeControlList;
        }
    }
}