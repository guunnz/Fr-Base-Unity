using System;
using Architecture.Injector.Core;
using FriendsView.Core.Domain;
using LocalizationSystem;
using TMPro;
using UniRx;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace FriendsView.View
{
    public class UnfriendDialog : MonoBehaviour
    {
        public Button confirmBtn;
        public IObservable<Unit> OnConfirmBtn => confirmBtn.OnClickAsObservable();
        
        public TextMeshProUGUI username;
        public TextMeshProUGUI usernameOk;
        public TextMeshProUGUI wantToUnfriend;
        private ILanguage language;

        private void Start()
        {
            language = Injection.Get<ILanguage>();
        }
        public void SetUnfriendBox(FriendData friendData)
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            language.SetText(wantToUnfriend, string.Format(language.GetTextByKey(LangKeys.PLAYER_DO_YOU_WANT_TO_UNFRIEND), friendData.username)); // Do you want to add //To your friends
            username.SetText(friendData.username);
            usernameOk.SetText(friendData.username);
        }
    }
}