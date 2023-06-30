using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;
using Architecture.Injector.Core;
using DG.Tweening;
using System;

namespace Onboarding
{
    public class OnboardingChatPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI TxtTitle;
        [SerializeField] private TextMeshProUGUI TxtChat;
        [SerializeField] private TextMeshProUGUI TxtInput;

        [SerializeField] private GameObject CloseContainer;
        [SerializeField] private GameObject Hand;
        [SerializeField] private TextMeshProUGUI TxtClose;

        private IOnboarding onboardingManager;
        private ILanguage language;

        public bool secondChat = false;

        public void ShowChatPrivate(IOnboarding onboardingManager)
        {
            this.onboardingManager = onboardingManager;
            this.onboardingManager.SetCameraPositionOpenChat();

            language = Injection.Get<ILanguage>();
            StartCoroutine(ShowPrivateChat());
            language.SetText(TxtTitle, "Friendbase");

            Color dataUsernameColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1);
            string dataUsername = "Friendbase";
            TxtChat.text = "";
            string message = language.GetTextByKey(LangKeys.COMMON_LABEL_HELLO);
            TxtChat.text += "<align=left><b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                         dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;

            language.SetTextByKey(TxtInput, LangKeys.CHAT_TAP_TO_START_WRITING);

            CloseContainer.SetActive(true);
            language.SetTextByKey(TxtClose, LangKeys.CLOSE);
            CloseContainer.transform.localScale = new Vector3(0f, 0f, 0f);
            CloseContainer.gameObject.transform.DOScale(1, 0.3f).SetDelay(0.4f).SetEase(Ease.OutExpo);
        }

        public void ShowChat(IOnboarding onboardingManager)
        {
            this.onboardingManager = onboardingManager;
            if (secondChat)
            {
                this.onboardingManager.SetCameraPositionOpenChat2();
            }
            else
            {
                this.onboardingManager.SetCameraPositionOpenChat();
            }

            language = Injection.Get<ILanguage>();

            language.SetText(TxtTitle, language.GetTextByKey(LangKeys.CHAT_CHAT));
            //language.SetTextByKey(TxtChat, LangKeys.COMMON_LABEL_HELLO);
            //language.SetTextByKey(TxtInput, LangKeys.CHAT_TAP_TO_START_WRITING);
            StartCoroutine(ShowPublicChat());
            CloseContainer.SetActive(true);
            language.SetTextByKey(TxtClose, LangKeys.CLOSE);
            CloseContainer.transform.localScale = new Vector3(0f, 0f, 0f);
            CloseContainer.gameObject.transform.DOScale(1, 0.3f).SetDelay(0.4f).SetEase(Ease.OutExpo);
        }

        public IEnumerator ShowPublicChat()
        {
            TxtChat.text += Environment.NewLine + Environment.NewLine;

            Color dataUsernameColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1);
            string dataUsername = "Friendbase";
            string message = language.GetTextByKey(LangKeys.ONBOARDING_CHAT1);
            TxtChat.text += "<align=left><b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                         dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;
            TxtChat.text += Environment.NewLine;
            yield return new WaitForSeconds(1f);
            dataUsernameColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1);
            dataUsername = "Deb";
            message = language.GetTextByKey(LangKeys.ONBOARDING_CHAT2);
            TxtChat.text += "<align=left><b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                         dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;
            TxtChat.text += Environment.NewLine;
            yield return new WaitForSeconds(1f);
            dataUsernameColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1);
            dataUsername = "Melika";
            message = language.GetTextByKey(LangKeys.ONBOARDING_CHAT3);
            TxtChat.text += "<align=left><b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                         dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;

            yield return null;
        }

        public IEnumerator ShowPrivateChat()
        {
            Color dataUsernameColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1);
            string dataUsername = "Friendbase";
            TxtChat.text += Environment.NewLine + Environment.NewLine;
            string message = language.GetTextByKey(LangKeys.COMMON_LABEL_HELLO);
            TxtChat.text += "<align=left><b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                         dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;

            yield return null;
        }

        private void OnEnable()
        {
            Hand.SetActive(true);
        }
        public void Close()
        {
            this.onboardingManager.SetCameraPosition();
            onboardingManager.WaitAndNextStep();
            Hand.SetActive(false);
            gameObject.SetActive(false);
        }

    }
}

