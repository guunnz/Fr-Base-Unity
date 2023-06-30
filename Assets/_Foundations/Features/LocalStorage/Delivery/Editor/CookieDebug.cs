using System;
using UnityEditor;
using UnityEngine;

namespace LocalStorage.Delivery.Editor
{
    public class CookieDebug : EditorWindow
    {
        [MenuItem(Const.GameName + "/Cookie Debugger")]
        public static void CreateWindow()
        {
            var wind = GetWindow<CookieDebug>();
            wind.titleContent = new GUIContent("Cookie Debug");
            wind.Show();
        }

        string key;
        string value;

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            key = EditorGUILayout.TextField(key);

            if (GUILayout.Button("Get String"))
            {
                value = PlayerPrefs.GetString(key);
            }

            if (GUILayout.Button("Get Int"))
            {
                value = PlayerPrefs.GetInt(key).ToString();
            }

            value = EditorGUILayout.TextField(value);

            if (GUILayout.Button("Save"))
            {
                PlayerPrefs.SetString(key, value);
                Debug.Log($"[{key}]={value}");
                PlayerPrefs.Save();
                Debug.Log("saved");
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
        }
    }
}