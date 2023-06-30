using System;
using System.Collections.Generic;
using CanvasInput.Core.Services;
using UniRx;
using UnityEngine;

namespace CanvasInput.Infrastructure
{
    public class CanvasTouchInput :  ICanvasInputSystem
    {
        const string NoStoreTapMessage = ", shouldn't be any Tap stored";
        const float TapToDragTime = .5f;
        const bool UpdateDragOnStationary = true;

        static long lastDragId;
        static long NewDragId => ++lastDragId;


        readonly CompositeDisposable disposables = new CompositeDisposable();


        readonly ISubject<InputInfo> dragBegin = new Subject<InputInfo>();
        readonly ISubject<InputInfo> dragCancel = new Subject<InputInfo>();
        readonly ISubject<InputInfo> dragUpdate = new Subject<InputInfo>();
        readonly ISubject<InputInfo> dragEnd = new Subject<InputInfo>();
        readonly ISubject<InputInfo> tap = new Subject<InputInfo>();

        /// <summary>
        ///touch id to info
        /// </summary>
        readonly Dictionary<int, InputInfo> infos = new Dictionary<int, InputInfo>();


        public void Enable()
        {
            infos.Clear();
            disposables.Clear();
            Observable.EveryUpdate().Subscribe(DoUpdate).AddTo(disposables);
            Observable.EveryApplicationFocus().Subscribe(infos.Clear).AddTo(disposables);
            Observable.EveryApplicationPause().Subscribe(infos.Clear).AddTo(disposables);
        }

        public void Disable()
        {
            infos.Clear();
            disposables.Clear();
        }

        void DoUpdate()
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                    BeginTouch(touch);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    UpdateTouch(touch);
                if (touch.phase == TouchPhase.Ended)
                    EndTouch(touch);
                if (touch.phase == TouchPhase.Canceled)
                    CancelTouch(touch);
            }
        }

        void CancelTouch(Touch touch)
        {
            if (!infos.TryGetValue(touch.fingerId, out var info))
            {
                Debug.LogError($"Canceling a touch without fingerId registered {touch.fingerId}");
                return;
            }

            infos.Remove(info.fingerId);
            dragCancel.OnNext(info);
        }

        void EndTouch(Touch touch)
        {
            if (!infos.TryGetValue(touch.fingerId, out var info))
            {
                Debug.LogError($"Ending a touch without fingerId registered {touch.fingerId}");
                return;
            }

            if (info.type == InputStateType.Tap)
            {
                Debug.LogError($"Ending a tap {touch.fingerId} {NoStoreTapMessage}");
                infos.Remove(info.fingerId);
                return;
            }

            info.currentTouch = touch.position;
            if (info.type == InputStateType.Undefined)
            {
                info.type = InputStateType.Tap;
                tap.OnNext(info);
                return;
            }

            if (info.type != InputStateType.Drag)
            {
                Debug.LogError($"Wrong input state type? int:{(int) info.type} {info.type.ToString()} ");
                return;
            }

            dragEnd.OnNext(info);
            infos.Remove(info.fingerId);
        }

        void UpdateTouch(Touch touch)
        {
            if (!infos.TryGetValue(touch.fingerId, out var info))
            {
                Debug.LogError($"Updating a touch without fingerId registered {touch.fingerId}");
                return;
            }

            if (info.startTime + TapToDragTime > Time.time)
            {
                //just wait for the tapToDragTime to be consumed or not and decide if this was a drag or a tap
                return;
            }

            if (info.type == InputStateType.Tap)
            {
                Debug.LogError($"you should not update a tap! {touch.fingerId}{NoStoreTapMessage}");
                return;
            }

            info.currentTouch = touch.position;

            if (info.type == InputStateType.Undefined)
            {
                info.type = InputStateType.Drag;
                dragBegin.OnNext(info);
                infos[info.fingerId] = info;
                return;
            }

            if (info.type != InputStateType.Drag)
            {
                Debug.LogError($"Wrong input state type? int:{(int) info.type} {info.type.ToString()} ");
                return;
            }

            if (touch.phase == TouchPhase.Stationary)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (UpdateDragOnStationary)
                {
                    dragUpdate.OnNext(info);
                }
            }
            else
            {
                dragUpdate.OnNext(info);
            }
        }

        void BeginTouch(Touch touch)
        {
            infos[touch.fingerId] = new InputInfo
            {
                dragId = NewDragId,
                fingerId = touch.fingerId,
                initialTouch = touch.position,
                currentTouch = touch.position,
                startTime = Time.time,
                type = InputStateType.Undefined,
            };
        }

        public IObservable<InputInfo> DragBegin() => dragBegin;
        public IObservable<InputInfo> DragUpdate() => dragUpdate;
        public IObservable<InputInfo> DragEnd() => dragEnd;
        public IObservable<InputInfo> Tap() => tap;
    }
}