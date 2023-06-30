using Unity.Usercentrics;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Adinmo;
using Adinmo.Flatbufs;

/// <summary>
/// Demo example.
/// This class contains a complete example of the Usercentrics integration.
/// This is:
/// - Initialization
/// - Update Services
/// - Show First and Second Layer
///
/// It also contains an example of AppTrackingTransparency usage.
/// 
/// Note that in a real integration the initialization should occur in some
/// initial stage of your game, for example the splash screen.
/// </summary>
public class DemoScript : MonoBehaviour
{
    [SerializeField] private Button ShowFirstLayerButton = null;
    [SerializeField] private Button ShowSecondLayerButton = null;
    [SerializeField] private Button ShowAttButton = null;
    [SerializeField] private Text textCheck;

    enum STAUS_ATT { NONE, AUTHORIZED, DENIED };
    private STAUS_ATT statusAtt;

    void Awake()
    {
        statusAtt = STAUS_ATT.NONE;
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }

    private void Start()
    {
        ShowFirstLayerButton.onClick.AddListener(() => { ShowFirstLayer(); });
        ShowSecondLayerButton.onClick.AddListener(() => { ShowSecondLayer(); });
        ShowAttButton.onClick.AddListener(() => { ShowAtt(); });

#if UNITY_IOS
        StartCoroutine(ManageATT());
#else
        StartCoroutine(ManageUC());
#endif

    }

    IEnumerator ManageATT()
    {
        ShowAtt();
        while (statusAtt == STAUS_ATT.NONE)
        {
            yield return null;
        }

        if (statusAtt == STAUS_ATT.AUTHORIZED)
        {
            StartCoroutine(ManageUC());
        }
        else
        {
            UnloadScene();
        }
        yield return null;
    }

    IEnumerator ManageUC()
    {
        InitAndShowConsentManagerIfNeeded();
        while (!Usercentrics.Instance.IsInitialized)
        {
            yield return null;
        }
        ShowFirstLayer();
        yield return null;
    }

    private void InitAndShowConsentManagerIfNeeded()
    {
        Usercentrics.Instance.Initialize((usercentricsReadyStatus) =>
        {
            if (usercentricsReadyStatus.shouldCollectConsent)
            {
                //textCheck.text = "show layer";
                ShowFirstLayer();
            }
            else
            {
                //textCheck.text = "no need";
                UnloadScene();
                UpdateServices(usercentricsReadyStatus.consents);
            }
        },
        (errorMessage) =>
        {
            // Log and collect the error...
        });
    }

    private void ShowAtt()
    {
        AppTrackingTransparency.Instance.PromptForAppTrackingTransparency((status) =>
        {
            switch (status)
            {
                case AuthorizationStatus.AUTHORIZED:
                    statusAtt = STAUS_ATT.AUTHORIZED;
                    break;
                case AuthorizationStatus.DENIED:
                case AuthorizationStatus.NOT_DETERMINED:
                case AuthorizationStatus.RESTRICTED:
                    statusAtt = STAUS_ATT.DENIED;
                    break;
            }
        });

    }

    private void ShowFirstLayer()
    {
        Usercentrics.Instance.ShowFirstLayer(UsercentricsLayout.Sheet, (usercentricsConsentUserResponse) =>
        {
            UpdateServices(usercentricsConsentUserResponse.consents);
        });
    }

    private void ShowSecondLayer()
    {
        Usercentrics.Instance.ShowSecondLayer(true, (usercentricsConsentUserResponse) =>
        {
            UpdateServices(usercentricsConsentUserResponse.consents);
        });
    }

    private void UpdateServices(List<UsercentricsServiceConsent> consents)
    {
        foreach (var serviceConsent in consents)
        {
            switch (serviceConsent.templateId)
            {
                case "XxxXXxXxX":
                    // GoogleAdsFramework.Enabled = service.consent.status;
                    break;
                case "YYyyYyYYY":
                    // AnalyticsFramework.Enabled = service.consent.status;
                    break;
                default:
                    break;
            }
        }

        GetTCString();
        //
    }
    void GetTCString()
    {
        Usercentrics.Instance.GetTCFData((tcfData) =>
        {
            var purposes = tcfData.purposes;
            var specialPurposes = tcfData.specialPurposes;
            var features = tcfData.features;
            var specialFeatures = tcfData.specialFeatures;
            var stacks = tcfData.stacks;
            var vendors = tcfData.vendors;

            // TCString
            var tcString = tcfData.tcString;
            AdinmoManager.SetDataUseConsent(true, tcString);
            UnloadScene();
        });
    }

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(0);
    }
}
