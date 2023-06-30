using Architecture.Context;
using Architecture.Injector.Core;
using CanvasInput.Core.Services;
using CanvasInput.Infrastructure;
using UnityEngine;

namespace CanvasInput
{
    public class CanvasInputModule : IModule
    {
        public void Init()
        {
            if (Input.touchSupported)
            {
                Injection.Register<ICanvasInputSystem, CanvasTouchInput>();
            }
            else
            {
                Injection.Register<ICanvasInputSystem, CanvasClickInput>();
            }
        }
    }
}