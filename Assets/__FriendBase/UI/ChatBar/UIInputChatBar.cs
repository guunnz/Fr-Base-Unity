using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using LocalizationSystem;
using Socket;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInputChatBar : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    
    [SerializeField] private Image imgChatBar;
    [SerializeField] private Sprite spriteBarPublic;
    [SerializeField] private Sprite spriteBarPrivate;
    [SerializeField] private Canvas canvas;

    protected ILanguage language;
    private ChatManager chatManager;
    private IGameData gameData;

    public const string friendbaseBlackHex = "#000000";
    public const string friendbaseBlueHex = "#33BCD2";
    public const string friendbaseWineHex = "#7A1602";
    public const string friendbaseGreenHex = "#82CB34";
    public const string friendbaseOrangeHex = "#ED4F39";
    const int messagesFieldLimit = 200;

    private Vector2 originalTextfieldPosition;
    private RectTransform inputFieldRectTransform;

    public List<string> colorsHex = new List<string>
        {
            friendbaseBlackHex,
            friendbaseBlueHex,
            friendbaseWineHex,
            friendbaseGreenHex,
            friendbaseGreenHex,
            friendbaseOrangeHex
        };

    void Start()
    {
        inputFieldRectTransform = _inputField.GetComponent<RectTransform>();
        originalTextfieldPosition = inputFieldRectTransform.anchoredPosition;

        RoomJoinManager.OnRoomReady += OnRoomReady;
        gameData = Injection.Get<IGameData>();
        language = Injection.Get<ILanguage>();
        OnShowPublicChat();
        SetMaxCharacterLimit();
        StartCoroutine( CheckKeyboardHeight());

        _inputField.onTouchScreenKeyboardStatusChanged.AddListener(ProcessDonePressed);
    }

    private void OnDestroy()
    {
        _inputField.onTouchScreenKeyboardStatusChanged.RemoveListener(ProcessDonePressed);
    }

    void ProcessDonePressed(TouchScreenKeyboard.Status newStatus)
    {
        Debug.Log("ProcessDonePressed " + newStatus);
        if (newStatus == TouchScreenKeyboard.Status.Done)
        {
            Debug.Log("--ProcessDonePressed");
            SendMessage();
        }
    }

    void SetMaxCharacterLimit()
    {
        _inputField.characterLimit = messagesFieldLimit;
    }

    IEnumerator CheckKeyboardHeight()
    {
        yield return new WaitForSeconds(0.5f);
        float keyboardHeight = 0f;

        while (true)
        {
            if (TouchScreenKeyboard.visible)
            {
                keyboardHeight = GetKeyboardSize() / canvas.scaleFactor;
                //Debug.Log("--------------------------------------------------");
                //Debug.Log("-------- keyboardHeight: " + keyboardHeight);
                //Debug.Log("-------- Screen.height: " + Screen.height);
                //Debug.Log("-------- TouchScreenKeyboard.height: " + TouchScreenKeyboard.area.height);
                //Debug.Log("-------- canvas: " + canvas.pixelRect.height);
                //Debug.Log("-------- canvas: " + Display.main.systemHeight);
                
                Vector2 newPosition = new Vector2(-Screen.width * 0.05f, keyboardHeight + 10);
                inputFieldRectTransform.anchoredPosition = newPosition;
            }
            else
            {
                keyboardHeight = 0f;
            }

            yield return new WaitForSeconds(0.7f);
        }
    }

    public float GetKeyboardSize()
    {
#if UNITY_ANDROID
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var unityPlayer = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);

                float result = Screen.height - Rct.Call<int>("height");
                //Debug.Log("---- KEY SIZE result_001:" + result);
                bool includeInput = true;
                if (includeInput)
                {
                    var dialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
                    var decorView = dialog?.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");

                    if (decorView != null)
                    {
                        var decorHeight = decorView.Call<int>("getHeight");
                        //Debug.Log("---- KEY SIZE decorHeight:" + decorHeight);
                        result += decorHeight;
                    }
                }
                //Debug.Log("---- KEY SIZE result_FINAL:" + result);
                return result;
            }
        }
#else
        return TouchScreenKeyboard.area.height;
#endif
    }

    void OnRoomReady()
    {
        chatManager = CurrentRoom.Instance.chatManager;
        chatManager.OnExitRoom += OnExitRoom;
        chatManager.OnShowPrivateChat += OnShowPrivateChat;
        chatManager.OnShowPublicChat += OnShowPublicChat;
        chatManager.OnSetPublicChatIcon += OnSetPublicChatIcon;
    }

    void OnExitRoom()
    {
        //Destroy
        RoomJoinManager.OnRoomReady -= OnRoomReady;
        chatManager.OnExitRoom -= OnExitRoom;
        chatManager.OnShowPrivateChat -= OnShowPrivateChat;
        chatManager.OnShowPublicChat -= OnShowPublicChat;
        chatManager.OnSetPublicChatIcon -= OnSetPublicChatIcon;
    }

    void OnSetPublicChatIcon()
    {
        OnShowPublicChat();
    }

    public void OnShowPublicChat()
    {
        textMeshProUGUI.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

        imgChatBar.sprite = spriteBarPublic;
        _inputField.text = "";
        TextMeshProUGUI placeholder = (TextMeshProUGUI)_inputField.placeholder;
        language.SetTextByKey(placeholder, LangKeys.LABEL_WRITE_A_MESSAGE_HERE);
        placeholder.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
    }

    public void OnShowPrivateChat(string idUser)
    {
        textMeshProUGUI.GetComponent<RectTransform>().offsetMin = new Vector2(25, 0);

        imgChatBar.sprite = spriteBarPrivate;
        _inputField.text = "";
        TextMeshProUGUI placeholder = (TextMeshProUGUI)_inputField.placeholder;
        language.SetTextByKey(placeholder, LangKeys.LABEL_WRITE_PRIVATE_MESSAGE);
        placeholder.GetComponent<RectTransform>().offsetMin = new Vector2(25, 0);
    }

    public void OnValueChanged(string text)
    {
    }

    public void OnSelect()
    {
        Debug.Log("OnSelect:" + Screen.height);
        //Vector2 newPosition = new Vector2( - Screen.width * 0.05f, (Screen.height/2) / canvas.scaleFactor);
        //inputFieldRectTransform.anchoredPosition = newPosition;
    }

    public void OnDeselect()
    {
        Debug.Log("OnDeselect:");
        //inputFieldRectTransform.anchoredPosition = originalTextfieldPosition;
        //_inputField.text = "";
    }

    void SendMessage()
    {
        if (!string.IsNullOrEmpty(_inputField.text))
        {
            if (chatManager.ChatSelected == ChatManager.ChatType.Public)
            {
                string color = colorsHex[Random.Range(0, colorsHex.Count)];
                SimpleSocketManager.Instance.SendChatMessage(CurrentRoom.Instance.RoomInformation.RoomName,
                                                             CurrentRoom.Instance.RoomInformation.RoomIdInstance,
                                                             _inputField.text,
                                                             gameData.GetUserInformation().UserName,
                                                             color);
            }
            else if (chatManager.ChatSelected == ChatManager.ChatType.Private)
            {
                if (!string.IsNullOrEmpty(chatManager.PrivateUserIdSelected))
                {
                    SimpleSocketManager.Instance.SendPrivateChat(int.Parse(chatManager.PrivateUserIdSelected),
                                                                 gameData.GetUserInformation().UserName,
                                                                 CurrentRoom.Instance.RoomInformation.RoomIdInstance,
                                                                 _inputField.text);
                }
            }
        }
    }

    public void OnEndEdit()
    {
        Debug.Log("OnEndEdit:");
#if UNITY_EDITOR
        //Only if we are on Editor, on mobile we check the ProcessDonePressed(TouchScreenKeyboard.Status newStatus) function
        SendMessage();
#endif
        _inputField.text = "";
        inputFieldRectTransform.anchoredPosition = originalTextfieldPosition;
    }
}
