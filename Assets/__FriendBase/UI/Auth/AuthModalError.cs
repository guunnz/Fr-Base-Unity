using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    public class AuthModalError : AbstractUIPanel
    {
        public TMP_Text title;
        public TMP_Text message;
        public Button button;

        private Image _imageOverlay;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _imageOverlay = container.GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public class Template
        {
            public string Title;
            public string Message;
        }

        // Templates
        public readonly Template TemplateDefault = new Template
        {
            Title = "Unexpected error",
            Message = "Don’t worry! Try to login or register again."
        };

        public readonly Template TemplateEmailInUse = new Template
        {
            Title = "Email in use",
            Message =
                "There’s already a Friendbase account associated with this email."
        };

        public readonly Template TemplateAccountBanned = new Template
        {
            Title = "Account banned",
            Message =
                "The account you are trying to use has been <b>permanently suspended</b>."
        };
        public readonly Template TemplateAccountTooMany = new Template
        {
            Title = "Too many attempts",
            Message =
          "Please wait a minute and try again</b>."
        };
        public void Show(string newTitle, string newMessage, Action newButtonEvent)
        {
            title.text = newTitle ?? TemplateDefault.Title;
            message.text = newMessage ?? TemplateDefault.Message;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                newButtonEvent?.Invoke();
                Close();
            });
            Open();
        }

        public override void OnOpen()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.DOFade(1, 0.5f);
            }

            if (_imageOverlay != null)
            {
                _imageOverlay.color =
                    new Color(_imageOverlay.color.r, _imageOverlay.color.g, _imageOverlay.color.b, 0f);
                _imageOverlay.DOFade(0.4f, 0.5f);
            }
        }
    }
}