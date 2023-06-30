using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AbstractProvider
{
    public enum OPERATION_TYPE { Login, Register, Link }

    public abstract ProviderType TypeProvider { get; }

    public abstract void LoginWithMailAndPassword(string email, string password, Action<GenericAuthFlowResult> authFlowCallback);
    public abstract void LoginSSO(Action<GenericAuthFlowResult> authFlowCallback, OPERATION_TYPE operationType);
    public abstract void LogOut();
}
