using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINauthFlowGenericInputText : MonoBehaviour
{
    public enum EVENT_TYPE { OnValueChanged, OnEndEdit, OnSelect, OnDeselect };

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Image openEye;
    [SerializeField] private Image closeEye;
    [SerializeField] private TextMeshProUGUI txtTitle;
    
    [SerializeField] private Sprite inputBoxDefault;
    [SerializeField] private Sprite inputBoxOrangeStroke;
    [SerializeField] private Sprite inputBoxYellowBg;

    [SerializeField] private UINauthFlowBoobleController bubbleSmall;
    [SerializeField] private UINauthFlowBoobleController bubbleMedium;

    [SerializeField] private string id;

    public delegate void BubblePressed(UINauthFlowGenericInputText inputText);
    public event BubblePressed OnBubblePressed;

    public delegate void InputFieldEvent(UINauthFlowGenericInputText inputText, EVENT_TYPE eventType);
    public event InputFieldEvent OnInputFieldEvent;

    public InputType InputTextType { get; private set; }
    public bool ShowPassword { get; private set; }

    public enum InputType
    {
        Text,
        Password,
        Mail
    }

    public string GetId()
    {
        return id;
    }

    void Start()
    {
        ShowPassword = false;
    }

    public void SetTitle(string text)
    {
        txtTitle.text = text;
    }

    public string GetText()
    {
        return inputField.text;
    }

    public void ShowBubbleSmall(string text, UINauthFlowBoobleController.IconType iconType)
    {
        HideBubbles();
        bubbleSmall.ShowBubble(text, iconType);
    }

    public void ShowBubbleMedium(string text, UINauthFlowBoobleController.IconType iconType)
    {
        HideBubbles();
        bubbleMedium.ShowBubble(text, iconType);
    }

    public void SetUp(InputType inputTextType, string text, string textPlaceholder)
    {
        InputTextType = inputTextType;
        SetTitle(text);
        HideAll();

        inputField.gameObject.SetActive(true);
        TextMeshProUGUI placeholder = (TextMeshProUGUI)inputField.placeholder;
        placeholder.text = textPlaceholder;
        RefreshPasswordVisibilityIcon();

        if (InputTextType == InputType.Mail)
        {
            inputField.contentType = TMP_InputField.ContentType.EmailAddress;
        }
        if (InputTextType == InputType.Text)
        {
            inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
        }
        inputField.text = "";
        inputField.ForceLabelUpdate();

        SetBoxFieldDefault();
    }

    public void RefreshPasswordVisibilityIcon()
    {
        if (InputTextType != InputType.Password)
        {
            openEye.gameObject.SetActive(false);
            closeEye.gameObject.SetActive(false);
            return;
        }

        if (ShowPassword)
        {
            inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
        }

        inputField.ForceLabelUpdate();

        openEye.gameObject.SetActive(ShowPassword);
        closeEye.gameObject.SetActive(!ShowPassword);
    }

    public void SwitchPasswordVisivility()
    {
        ShowPassword = !ShowPassword;
        RefreshPasswordVisibilityIcon();
    }

    private void HideAll()
    {
        inputField.gameObject.SetActive(false);
        openEye.gameObject.SetActive(false);
        closeEye.gameObject.SetActive(false);
        HideBubbles();
    }

    public void HideBubbles()
    {
        bubbleSmall.Close();
        bubbleMedium.Close();
    }

    public void OnBubblePressDown()
    {
        if (OnBubblePressed!=null)
        {
            OnBubblePressed(this);
        }
    }

    //BOB FIELD
    public void SetBoxFieldDefault()
    {
        inputField.GetComponent<Image>().sprite = inputBoxDefault;
    }

    public void SetBoxFieldYellowBackground()
    {
        inputField.GetComponent<Image>().sprite = inputBoxYellowBg;
    }

    public void SetBoxFieldOrangeStroke()
    {
        inputField.GetComponent<Image>().sprite = inputBoxOrangeStroke;
    }


    public void OnValueChanged()
    {
        if (OnInputFieldEvent!=null)
        {
            OnInputFieldEvent(this, EVENT_TYPE.OnValueChanged);
        }
        Debug.Log("OnValueChanged");
    }

    public void OnEndEdit()
    {
        if (OnInputFieldEvent != null)
        {
            OnInputFieldEvent(this, EVENT_TYPE.OnEndEdit);
        }
        Debug.Log("OnEndEdit");
    }

    public void OnSelect()
    {
        if (OnInputFieldEvent != null)
        {
            OnInputFieldEvent(this, EVENT_TYPE.OnSelect);
        }
        Debug.Log("OnSelect");
    }

    public void OnDeselect()
    {
        if (OnInputFieldEvent != null)
        {
            OnInputFieldEvent(this, EVENT_TYPE.OnDeselect);
        }
        Debug.Log("OnDeselect");
    }
}
