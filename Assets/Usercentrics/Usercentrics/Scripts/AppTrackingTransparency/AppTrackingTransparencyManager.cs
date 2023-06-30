
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Events;

public enum AuthorizationStatus
{
    AUTHORIZED,
    DENIED,
    NOT_DETERMINED,
    RESTRICTED
}

public class AppTrackingTransparencyManager
{

    #if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void requestForAppTrackingTransparency(TrackingTransparencyDelegate callback);
    #endif

    public static UnityAction<AuthorizationStatus> AttCallback;
    private delegate void TrackingTransparencyDelegate(int status);

    [MonoPInvokeCallback(typeof(TrackingTransparencyDelegate))]
    private static void delegateMessageReceived(int number)
    {
        switch (number)
        {
            case 0:
                AttCallback?.Invoke(AuthorizationStatus.NOT_DETERMINED);
                break;
            case 1:
                AttCallback?.Invoke(AuthorizationStatus.RESTRICTED);
                break;
            case 2:
                AttCallback?.Invoke(AuthorizationStatus.DENIED);
                break;
            case 3:
                AttCallback?.Invoke(AuthorizationStatus.AUTHORIZED);
                break;
            default:
                break;
        }
    }

    public static void RequestForAppTrackingTransparency()
    {
        #if UNITY_IPHONE
        {
            requestForAppTrackingTransparency(delegateMessageReceived);
        }
        #endif
    }
}