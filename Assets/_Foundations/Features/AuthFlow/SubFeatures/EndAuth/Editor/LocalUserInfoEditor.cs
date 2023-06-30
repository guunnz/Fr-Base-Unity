using System;
using AuthFlow.EndAuth.Repo;
using LocalStorage.Delivery;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace AuthFlow.EndAuth.Editor
{
    public class LocalUserInfoEditor : EditorWindow
    {
        [MenuItem(Const.GameNameMenu + "Local User Info")]
        public static void OpenWindow()
        {
            var wind = GetWindow<LocalUserInfoEditor>();
            wind.titleContent = new GUIContent("Local User Info");
            wind.minSize = Vector2.one * 50;
            wind.Show();
        }

        Vector2 scroll;

        readonly LocalUserInfo userInfo = new LocalUserInfo(new UnityLocalStorage());

        void OnGUI()
        {
            var keys = userInfo.GetKeys();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var key in keys)
            {
                var value = userInfo[key];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(key);
                string newValue = value;
                if (value != null)
                {
                    if (value.Length > 20 || value.Contains("\n"))
                    {
                        newValue = EditorGUILayout.TextArea(value, GUILayout.MaxWidth(300));
                    }
                    else
                    {
                        newValue = EditorGUILayout.TextField(value);
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (newValue != value)
                {
                    userInfo[key] = newValue;
                }
            }

            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Refresh"))
            {
                userInfo.Refresh();
            }
        }
    }
}
