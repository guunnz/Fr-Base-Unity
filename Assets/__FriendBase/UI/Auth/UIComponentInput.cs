using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using LocalizationSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    [ExecuteInEditMode]
    public class UIComponentInput : MonoBehaviour
    {
        public enum InputType
        {
            Text,
            Password,
            Other,
        }

        [Header("Properties")]
        public InputType type;

        public string label;
        public string value;
        public string placeholder;
        public string messageInfo;
        public string messageError;
        public bool interactable = true;

        [Header("References")]
        public TMP_Text labelText;

        public TMP_Text placeholderTextInput;
        public TMP_Text placeholderTextPassword;
        public TMP_InputField inputFieldText;
        public TMP_InputField inputFieldPassword;
        public TMP_Text messageInfoText;
        public TMP_Text messageErrorText;
        public GameObject containerInputText;
        public GameObject containerInputPassword;
        public Image inputTextOutline;
        public Image inputPasswordOutline;
        public List<VerticalLayoutGroup> verticalLayoutGroup;
        private CanvasGroup _canvasGroup;
        [SerializeField] private Color regularColor;
        [SerializeField] private Color errorColor;

        private ILanguage language;

        public void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            language = Injection.Get<ILanguage>();
             
            SetValues();
        }

        public void OnEnable()
        {
            SetValues();
        }

        //public void OnValidate()
        //{
        //    language = Injection.Get<ILanguage>();
        //    SetValues();
        //}

        public void SetLabel(string text)
        {
            label = text;
        }

        public void SetPlaceholder(string text)
        {
            placeholder = text;
        }

        public void SetMessageError(string text)
        {
            messageError = text;
        }

        public void SetValues()
        {
            language = Injection.Get<ILanguage>();

            if (containerInputText != null)
                containerInputText.SetActive(type == InputType.Text);

            if (containerInputPassword != null)
                containerInputPassword.SetActive(type == InputType.Password);

            if (labelText != null)
                language.SetText(labelText, label);

            if (placeholderTextInput != null)
                language.SetText(placeholderTextInput, placeholder);

            if (placeholderTextPassword != null)
                language.SetText(placeholderTextPassword, placeholder);

            if (inputFieldText != null)
                language.SetText(inputFieldText, value);

            if (inputFieldPassword != null)
                language.SetText(inputFieldPassword, value);

            SetError(messageError);

            SetInfo(messageInfo);

            if (_canvasGroup != null)
                _canvasGroup.interactable = interactable;

            UpdateLayout();
        }

        public void SetError(string message)
        {
            messageError = message;

            if (messageErrorText != null)
            {
                if (language == null)
                    language = Injection.Get<ILanguage>();
                language.SetText(messageErrorText, messageError);
                messageErrorText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(messageError));
            }

            // Update outline color
            if (string.IsNullOrEmpty(messageError))
            {
                inputTextOutline.color = regularColor;
                inputPasswordOutline.color = regularColor;
            }
            else
            {
                inputTextOutline.color = errorColor;
                inputPasswordOutline.color = errorColor;
            }

            UpdateLayout();
        }

        public void SetInfo(string message)
        {
            messageInfo = message;

            if (messageInfoText != null)
            {
                language.SetText(messageInfoText, messageInfo);
                messageInfoText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(messageInfo));
            }

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (verticalLayoutGroup == null)
                return;

            foreach (VerticalLayoutGroup layout in verticalLayoutGroup.Where(layout => layout != null))
            {
                // Update Vertical Layout Group to fix content position
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
            }
        }

        public void ClearInput()
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            language.SetText(inputFieldText, "");
        }

        public string GetPassword()
        {
            return inputFieldPassword.text;
        }

        public void ClearPassword()
        {
            if (inputFieldPassword == null)
                return;

            if (language == null)
                language = Injection.Get<ILanguage>();
            language.SetText(inputFieldPassword, "");
        }

        public void OnChange(string text)
        {
            value = text;

            // Clear input status messages
            SetError("");
            SetInfo("");
        }
    }
}