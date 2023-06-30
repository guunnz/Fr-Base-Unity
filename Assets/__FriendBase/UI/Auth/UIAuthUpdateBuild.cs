using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using Architecture.Injector.Core;
using System;

namespace UI.Auth
{
    public class UIAuthUpdateBuild : AbstractUIPanel
    {
        [SerializeField] private TextMeshProUGUI TxtButtonUpdate;
        [SerializeField] private TextMeshProUGUI TxtDescription;

        protected override void Start()
        {
            base.Start();
            Open();
            Injection.Get<ILoading>().Unload();
        }

        public override void OnOpen()
        {
            language.SetText(TxtButtonUpdate, language.GetTextByKey(LangKeys.UPDATE));
            language.SetText(TxtDescription, language.GetTextByKey(LangKeys.WE_ARE_SORRY) + Environment.NewLine + language.GetTextByKey(LangKeys.UPDATE_GAME));
        }

        public void OnUpdateGameVersion()
        {
#if UNITY_ANDROID
            Application.OpenURL(Constants.GOOGLE_PLAY_URL);
            //Application.OpenURL("market://details?id=YOUR_ID");
#endif

#if (UNITY_IPHONE || UNITY_IOS)
            Application.OpenURL(Constants.APP_STORE_URL);
            //Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
        }
    }
}

