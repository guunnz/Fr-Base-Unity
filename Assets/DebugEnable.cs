using UnityEngine;
using System.Diagnostics;
using System.Reflection;

public class DebugEnable : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Get the parent object of this object
        GameObject parentObject = transform.parent.gameObject;

        // Check if the parent object is active
        if (parentObject.activeSelf)
        {
            // Get the call stack and find the method that enabled the object
            StackTrace stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                if (method != null && method.ReflectedType != null && method.ReflectedType.Name != "DebugEnable")
                {
                    UnityEngine.Debug.Log("My parent object " + parentObject.name + " was enabled by " + method.ReflectedType.Name);
                    break;
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("My parent object " + parentObject.name + " is disabled.");
        }
    }
}
