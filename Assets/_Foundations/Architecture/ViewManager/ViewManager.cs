using System;
using UnityEngine;

namespace Architecture.ViewManager
{
    public interface IViewManager
    {
        void Show<T>() where T : ViewNode;
        void ShowModal<T>(params object[] parameters) where T : ViewNode;
        bool GetOut(string outPort);

        //IObservable<T> ShowSwipe<T>(float duration = .1f, Vector2 direction = default) where T : ViewNode;
        //IObservable<bool> GetOutSwipe(string outPort, float duration = .1f, Vector2 direction = default);

        void GoBack();
    }

    public static class ViewManagerExtension
    {
        public static void DebugGetOut(this IViewManager vm, string outPort)
        {
            if (!vm.GetOut(outPort))
            {
#if UNITY_EDITOR
                var exception = new Exception("can't perform get-out on port " + outPort);
                Debug.LogError(exception);
                throw exception;
#endif
            }
        }
    }
}