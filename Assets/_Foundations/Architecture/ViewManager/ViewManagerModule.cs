using System;
using System.Collections.Generic;
using Architecture.Context;
using Architecture.Injector.Core;
using Shared.Utils;
using UnityEngine;

namespace Architecture.ViewManager
{
    public enum ViewManagerState
    {
        None = 0,
        PerformingShow = 1,
    }

    public class ViewManagerModule : ScriptModule, IViewManager
    {
        public ViewNodesNavigation config;
        public Canvas canvas;

        [Space(10)] public bool useTransition;
        public GameObject screenClickBLocker;

        private MovementInfo movementCache;
        private ViewManagerState state = ViewManagerState.None;


        private ViewNode current;
        private TypedPool pool;

        private Stack<ViewNode> modalStack = new Stack<ViewNode>();


        // Architecture Callbacks 

        public override void Init()
        {
            pool = new TypedPool(CreateNode);
            Injection.Register((IViewManager) this);
            canvas.gameObject.DestroyChildren(screenClickBLocker);
            config.Init();
        }

        // API

        public void ShowModal<T>(params object[] parameters) where T : ViewNode
        {
            ShowModal(typeof(T), parameters);
        }

        private void ShowModal(Type type, params object[] parameters)
        {
            if (state == ViewManagerState.PerformingShow)
            {
                movementCache.SetGoToNode(type, true, parameters);
                return;
            }

            state = ViewManagerState.PerformingShow;
            var viewNode = pool.Get(type);
            ShowNodeModal(viewNode.SetParameters(parameters));
            state = ViewManagerState.None;

            CheckCache();
        }


        public void Show<T>() where T : ViewNode
        {
            Show(typeof(T));
        }

        private void Show(Type type)
        {
            if (state == ViewManagerState.PerformingShow)
            {
                movementCache.SetGoToNode(type);
                return;
            }

            state = ViewManagerState.PerformingShow;
            var viewNode = pool.Get(type);
            ShowNode(viewNode);
            state = ViewManagerState.None;
            CheckCache();
        }


        public bool GetOut(string outputKey)
        {
            if (!current)
            {
                Debug.LogWarning("Current is null ", current);
                return false;
            }

            var node = config.FromNode(current.GetType(), outputKey);
            if (!node)
            {
                Debug.LogWarning($"{current.GetType().Name} node has no out port \"{outputKey}\"");
                return false;
            }

            if (state == ViewManagerState.PerformingShow)
            {
                Debug.Log("_ making get out cache _");
                movementCache.SetGetOut(outputKey);
                return true; // ???
            }

            state = ViewManagerState.PerformingShow;
            ShowNode(pool.Get(node.GetType()));
            state = ViewManagerState.None;
            CheckCache();
            return true;
        }

        public void GoBack()
        {
            if (modalStack.Count == 0)
            {
                Debug.LogWarning("Going back on empty stack? -> trying to find \"back\" outport");
                GetOut("back");
                return;
            }

            var lastNode = modalStack.Pop();
            ShowPreviousNode(lastNode);
        }


        // Internal Utilities

        private void ShowPreviousNode(ViewNode viewNode)
        {
            viewNode.Focus();
            if (current)
            {
                current.HideView();
            }

            current = viewNode;
            current.RectTransform.anchoredPosition = Vector2.zero;
            viewNode.RectTransform.anchoredPosition = Vector2.zero;
        }

        private void ShowNode(ViewNode viewNode)
        {
            while (modalStack.Count > 0)
            {
                GoBack();
            }
            
            var oldNode = current;
            var newNode = viewNode;
            
            //set current node before any callback to ensure consistency
            current = viewNode;
             
            newNode.ShowView();

            newNode.Focus();
            if (oldNode)
            {
                oldNode.HideView();
            }
            
            current.RectTransform.anchoredPosition = Vector2.zero;
            newNode.RectTransform.anchoredPosition = Vector2.zero;
        }


        private void ShowNodeModal(ViewNode newNode)
        {
            newNode.ShowView();
            newNode.Focus();
            if (current)
            {
                modalStack.Push(current);
                current.Unfocus();
            }

            current = newNode;
            current.RectTransform.anchoredPosition = Vector2.zero;
            newNode.RectTransform.anchoredPosition = Vector2.zero;

            newNode.transform.SetAsLastSibling();
            screenClickBLocker.transform.SetAsLastSibling();
        }

        private void CheckCache()
        {
            if (movementCache.UseGetOut(out var outPort))
            {
                movementCache.Consume();
                GetOut(outPort);
                return;
            }

            if (movementCache.UseGoToNode(out var type, out var parameters))
            {
                movementCache.Consume();
                if (movementCache.IsModal())
                {
                    ShowModal(type, parameters);
                }
                else
                {
                    Show(type);
                }
            }
        }


        private INode CreateNode(Type type)
        {
            var prefab = config.Find(type);
            var instance = Instantiate(prefab, canvas.transform);
            instance.transform.SetAsFirstSibling();
            return instance;
        }

        // Unity Callbacks

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // back key on mobile devices
            {
                GoBack();
            }
        }

        private void OnDestroy()
        {
                pool.Clear();
            movementCache.Clear();
        }

        #region Swipe

        // public IObservable<bool> GetOutSwipe(string outputKey, float duration = 1, Vector2 direction = default)
        // {
        //     if (!current) return Observable.Return(false);
        //     var node = config.FromNode(current.GetType(), outputKey);
        //     if (!node) return Observable.Return(false);
        //
        //
        //     if (duration <= 0.001f) return Observable.Return(GetOut(outputKey));
        //
        //     if (direction == default) direction = Vector2.left;
        //     direction.Normalize();
        //
        //     node = (ViewNode) pool.Get(node.GetType());
        //     node.ShowView();
        //     return SwapWindows(current, node, duration, direction)
        //         .ToObservable()
        //         .Do(_ =>
        //         {
        //             if (current)
        //                 current.HideView();
        //             current = node;
        //         })
        //         .Select(_ => node != null);
        // }
        //
        // public IObservable<T> ShowSwipe<T>(float duration = 1, Vector2 direction = default) where T : ViewNode
        // {
        //     if (duration <= 0.001f) return Observable.Return(Show<T>());
        //
        //     if (direction == default) direction = Vector2.left;
        //     direction.Normalize();
        //
        //     var viewNode = pool.Get<T>();
        //     viewNode.ShowView();
        //     return SwapWindows(current, viewNode, duration, direction)
        //         .ToObservable()
        //         .Do(_ =>
        //         {
        //             if (current)
        //                 current.HideView();
        //             current = viewNode;
        //         })
        //         .Select(_ => viewNode);
        // }
        // private IEnumerator SwapWindows(ViewNode currentNode, ViewNode nextNode, float duration, Vector2 direction)
        // {
        //     screenClickBLocker.SetActive(true);
        //     var rect = currentNode.RectTransform.rect;
        //     var nextTargetPosition = Vector2.zero; //nextNode.RectTransform.anchoredPosition;
        //     var nextSourcePosition = nextTargetPosition - direction * rect.size;
        //     var currentSourcePosition = Vector2.zero; //currentNode.RectTransform.anchoredPosition;
        //     var currentTargetPosition = currentSourcePosition + direction * rect.size;
        //
        //     nextNode.transform.position = nextSourcePosition;
        //
        //     var time = duration;
        //     while (time >= 0)
        //     {
        //         time -= Time.deltaTime;
        //         var t = 1 - time / duration;
        //         currentNode.RectTransform.anchoredPosition =
        //             Vector3.Lerp(currentSourcePosition, currentTargetPosition, t);
        //         nextNode.RectTransform.anchoredPosition = Vector3.Lerp(nextSourcePosition, nextTargetPosition, t);
        //         yield return null;
        //     }
        //
        //     screenClickBLocker.SetActive(false);
        // }

        #endregion
    }
}