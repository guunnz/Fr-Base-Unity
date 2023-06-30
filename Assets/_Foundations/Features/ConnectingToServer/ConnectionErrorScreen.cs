 using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;
using Architecture.Injector.Core;
using AuthFlow;
using System.Collections;
using System.Net.NetworkInformation;

namespace ConnectingToServer
{
    public class ConnectionErrorScreen : MonoBehaviour
    {
        [SerializeField] public Button tryAgainButton;
        [SerializeField] public Button logoutButton;
        [SerializeField] public TextMeshProUGUI logoutText;
        [SerializeField] public TextMeshProUGUI tryAgainText;

        public TextMeshProUGUI OopsText;
        public ILanguage language;

        private int invisibleButtonCount;

        private IEnumerator Start()
        {
            ILoading loading = Injection.Get<ILoading>();
            yield return null;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!loading.isloading())
                {
                    loading.Load();
                }

                AuthFlow.JesseUtils.StartGame();
                yield break;
            }

            loading.Unload();
            yield return null;
            invisibleButtonCount = 0;
            language = Injection.Get<ILanguage>();
            string msg = language.GetTextByKey(LangKeys.COMMON_LABEL_OOPS_WERE_SO_SORRY) + Environment.NewLine + Environment.NewLine + language.GetTextByKey(LangKeys.COMMON_LABEL_THERE_WAS_AN_UNEXPECTED_ERROR) + Environment.NewLine + Environment.NewLine + language.GetTextByKey(LangKeys.PLEASE_ENTER_FRIENDBASE_AGAIN);
            language.SetText(OopsText, msg);

            language.SetText(logoutText, LangKeys.MAIN_LOG_OUT);
            language.SetText(tryAgainText, language.GetTextByKey(LangKeys.TRY_AGAIN_ERROR));
        }

        void OnEnable()
        {
            

            tryAgainButton.onClick.AddListener(() =>
            {
                Clear();
                AuthFlow.JesseUtils.StartGame();
            });
        }

        void Clear()
        {
            tryAgainButton.onClick.RemoveAllListeners();
        }

        public void OnLogout()
        {
            JesseUtils.Logout();
        }


        public void ClickInvisibleButton()
        {
            invisibleButtonCount++;
            if (invisibleButtonCount >= 5)
            {
                logoutButton.gameObject.SetActive(true);
            }
        }

        void OnDisable()
        {
            Clear();
        }
    }
}