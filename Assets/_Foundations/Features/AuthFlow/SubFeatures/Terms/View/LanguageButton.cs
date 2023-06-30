using System;
using AuthFlow.Terms.Presentation;
using UI;
using UI.Auth;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Terms.View
{
    public class LanguageButton : MonoBehaviour, ILanguageButton
    {
        [SerializeField] string currentKey = "en";
        [SerializeField] Button button;
        //[SerializeField] Image icon;
        [SerializeField] StringWidget label;
        [SerializeField] LanguageInfo languageInfo;
        [SerializeField] UIAuthTermsView termsManager;


        void ApplyKey()
        {
            //icon.sprite = languageInfo.GetSprite(currentKey);
            label.Value = languageInfo.GetLabelName(currentKey);
        }

        void Awake()
        {
            ApplyKey();
        }

        public void SetLanguage()
        {
            //termsManager.SetLanguage(currentKey);
        }

        public IObservable<Unit> OnCLick => button.OnClickAsObservable();
        public string Key => currentKey;


#if UNITY_EDITOR
        void OnValidate() => ApplyKey();
#endif
    }
}