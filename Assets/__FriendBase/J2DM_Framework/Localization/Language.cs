using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using TMPro;
using LocalStorage.Core;
using Architecture.Injector.Core;

namespace LocalizationSystem
{
    public class Language : ILanguage
    {
        public const string CURRENT_LANGUAGE_STORAGE_KEY = "CurrentLanguage";

        private Dictionary<string, Dictionary<string, string>> _language;
        private Dictionary<string, string> _currentLanguage;
        public string _currentLanguageKey;

        public const string NODE_ITEM = "item";
        public const string NODE_KEY = "key";
        public const string NODE_VALUE = "value";

        private ILocalStorage localStorage;

        public Language()
        {
            localStorage = Injection.Get<ILocalStorage>();
            _currentLanguage = null;
            _language = new Dictionary<string, Dictionary<string, string>>();
        }

        public void AddLanguageFromXML(string lang, string text)
        {
            if (_language.ContainsKey(lang))
            {
                //Duplicate language
                return;
            }

            _language.Add(lang, new Dictionary<string, string>());

            XmlTextReader reader = new XmlTextReader(new StringReader(text));
            while (reader.Read())
            {
                if (reader.Name == NODE_ITEM)
                {
                    //Debug.Log( "Key:" + reader.GetAttribute(NODE_KEY) + " Value:"+ reader.GetAttribute(NODE_VALUE)); 
                    _language[lang][reader.GetAttribute(NODE_KEY)] = reader.GetAttribute(NODE_VALUE);
                }
            }

            if (_currentLanguage == null)
            {
                _currentLanguage = _language[lang];
                _currentLanguageKey = lang;
            }
        }

        public List<string> GetAvailablesLanguages()
        {
            List<string> listLanguages = new List<string>();
            foreach (var lang in _language)
            {
                listLanguages.Add(lang.Key);
            }
            return listLanguages;
        }

        public bool SetCurrentLanguage(string lang)
        {
            if (!_language.ContainsKey(lang))
            {
                //The language does not exist
                return false;
            }

            _currentLanguage = _language[lang];
            _currentLanguageKey = lang;

            localStorage.SetString(CURRENT_LANGUAGE_STORAGE_KEY, lang);

            return true;
        }

        public bool RemoveLanguage(string lang)
        {
            if (!_language.ContainsKey(lang))
            {
                //language to remove does not exist
                return false;
            }

            if (_currentLanguage == _language[lang])
            {
                _currentLanguage = null;
            }

            _language.Remove(lang);

            return true;
        }

        public string GetCurrentLanguage()
        {
            return _currentLanguageKey;
        }

        public string GetTextByKey(string key)
        {
            if (_currentLanguage == null)
            {
                //Utils.SetLogCat ("J2DM_Language", "getTextByKey", "currentLanguage is null");
                return "Error Key: Not set a default language";
            }

            if (!_currentLanguage.ContainsKey(key))
            {
                //Utils.SetLogCat ("J2DM_Language", "getTextByKey", "invalid key:"+key);
                return "Error Key: " + key + " does not exist";
            }

            string text = _currentLanguage[key];
            text = text.Replace("|n", "\n");
            return text;
        }

        public void SetTextByKey(TextMeshProUGUI textObject, string langKey)
        {
            textObject.text = GetTextByKey(langKey);
        }

        public void SetTextByKey(TMP_Text textObject, string langKey)
        {
            textObject.text = GetTextByKey(langKey);
        }

        public void SetTextByKey(TMP_InputField textObject, string langKey)
        {
            textObject.text = GetTextByKey(langKey);
        }

        public void SetText(TextMeshProUGUI textObject, string text)
        {
            textObject.text = text;
        }

        public void SetText(TMP_Text textObject, string text)
        {
            textObject.text = text;
        }

        public void SetText(TMP_InputField textObject, string text)
        {
            textObject.text = text;
        }
    }
}

