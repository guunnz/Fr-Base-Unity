using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] public bool _dontDestroyOnLoad = true;
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject(name: "Instance of " + typeof(T)).AddComponent<T>();
                }
                _instance.GetComponent<GenericSingleton<T>>().Initialize();

                if (_instance.GetComponent<GenericSingleton<T>>()._dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance.gameObject == this.gameObject)
        {
            return;
        }

        if (_instance != null)
        {
            DestroyImmediate(this.gameObject); //Prevent duplicates
            return;
        }
        else
        {
            if (_dontDestroyOnLoad)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }
        Initialize();
    }

    protected virtual void Initialize()
    {
    }
}