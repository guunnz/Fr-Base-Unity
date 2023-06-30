using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using LocalizationSystem;
using UnityEngine;

public abstract class AbstractUIPanel : MonoBehaviour
{
    [SerializeField] protected GameObject container;

    public bool IsOpen { get; private set; }
    protected ILanguage language;

    protected virtual void Start()
    {
        language = Injection.Get<ILanguage>();
    }

    public virtual void Open()
    {
        IsOpen = true;
        container.SetActive(true);
        OnOpen();
    }

    public virtual void Close()
    {
        IsOpen = false;
        container.SetActive(false);
        OnClose();
    }

    public virtual void OnOpen()
    {
        // ..
    }
    
    public virtual void OnClose()
    {
        // ..
    }
}
