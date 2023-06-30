using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;
using Architecture.Injector.Core;

public class UINauthFlowGenericLoginButton : MonoBehaviour
{
    private enum ButtonType { LOGIN, REGISTER, CONVERT_GUEST}
    
    [SerializeField] private ProviderType providerType;
    [SerializeField] private TextMeshProUGUI textButton;
    [SerializeField] private ButtonType buttonType;

    public delegate void PressButton(ProviderType providerType);
    public event PressButton OnPressButton;
    private ILanguage language;

    public ProviderType GetProviderType()
    {
        return providerType;
    }

    void Start()
    {
        language = Injection.Get<ILanguage>();
        SetAutomaticText();
    }

    public void SetAutomaticText()
    {
        switch (providerType)
        {
            case ProviderType.GOOGLE:
                if (buttonType==ButtonType.LOGIN)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_LOGIN_GOOGLE);
                }
                else if (buttonType == ButtonType.REGISTER)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_JOIN_GOOGLE);
                }
                break;
            case ProviderType.FACEBOOK:
                if (buttonType == ButtonType.LOGIN)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_LOGIN_FACEBOOK);
                }
                else if (buttonType == ButtonType.REGISTER)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_JOIN_FACEBOOK);
                }
                break;
            case ProviderType.APPLE:
                if (buttonType == ButtonType.LOGIN)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_LOGIN_APPLE);
                }
                else if (buttonType == ButtonType.REGISTER)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_JOIN_APPLE);
                }
                break;
            case ProviderType.EMAIL:
                if (buttonType == ButtonType.LOGIN)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_LOGIN_EMAIL);
                }
                else if (buttonType == ButtonType.REGISTER)
                {
                    language.SetTextByKey(textButton, LangKeys.NAUTH_JOIN_EMAIL);
                }
                break;
        }
    }

    public void SetText(string text)
    {
        language.SetText(textButton, text);
    }

    public void OnClickButton()
    {
        if (OnPressButton!=null)
        {
            OnPressButton(providerType);
        }
    }
}
