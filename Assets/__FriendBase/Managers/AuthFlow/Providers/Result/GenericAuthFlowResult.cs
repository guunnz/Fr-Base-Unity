using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

public class GenericAuthFlowResult 
{
    public enum STATE { SUCCEED, ERROR };

    public enum ERROR_TYPE { NONE, CANCELED, TOO_MANY_ATTEMPS, DIFFERENT_PROVIDER, ERROR_SIGNING_WITH_CREDENTIAL, INCORRECT_PASSWORD, ACCOUNT_NOT_FOUND };

    public STATE State { get; private set; }
    public ERROR_TYPE ErrorType { get; private set; }
    public FirebaseUser UserFirebase { get; set; }

    public GenericAuthFlowResult(STATE state, FirebaseUser userFirebase)
    {
        this.State = state;
        this.UserFirebase = userFirebase;
    }

    public GenericAuthFlowResult(STATE state, ERROR_TYPE errorType)
    {
        this.State = state;
        this.ErrorType = errorType;
    }
}
