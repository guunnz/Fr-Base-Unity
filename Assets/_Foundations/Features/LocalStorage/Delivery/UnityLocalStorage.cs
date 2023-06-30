// TODO(Jesse): Why???  Just use playerprefs directly?
//
using System;
using LocalStorage.Core;
using UnityEngine;

namespace LocalStorage.Delivery
{
    public class UnityLocalStorage : ILocalStorage
    {
        public void SetString(string key, string value)
        {
            try
            {
                PlayerPrefs.SetString(key, value);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            PlayerPrefs.Save();
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }
    }
}
