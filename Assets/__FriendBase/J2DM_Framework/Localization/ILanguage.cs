using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LocalizationSystem
{
    public interface ILanguage
    {
        void AddLanguageFromXML(string lang, string text);
        bool SetCurrentLanguage(string lang);
        bool RemoveLanguage(string lang);
        string GetCurrentLanguage();
        string GetTextByKey(string key);
        void SetTextByKey(TextMeshProUGUI textObject, string langKey);
        void SetTextByKey(TMP_Text textObject, string langKey);
        void SetTextByKey(TMP_InputField textObject, string langKey);
        void SetText(TextMeshProUGUI textObject, string text);
        void SetText(TMP_Text textObject, string text);
        void SetText(TMP_InputField textObject, string text);
        List<string> GetAvailablesLanguages();
    }
}