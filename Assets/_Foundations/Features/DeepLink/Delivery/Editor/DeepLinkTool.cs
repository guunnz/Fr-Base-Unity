using UnityEditor;
using UnityEngine;

namespace DeepLink.Delivery.Editor
{
    public class DeepLinkTool : EditorWindow
    {
        [MenuItem(Const.GameNameMenu + "Deep Link")]
        private static void ShowWindow()
        {
            var window = GetWindow<DeepLinkTool>();
            window.titleContent = new GUIContent("Deep Link");
            window.Show();
        }

        private void OnGUI()
        {
            const string dlKey = "stored-deep-link";
            const string flagKey = "use-deep-link";

            EditorGUI.BeginChangeCheck();

            var value = PlayerPrefs.GetString(dlKey);
            value = EditorGUILayout.TextField("link", value);
            PlayerPrefs.SetString(dlKey, value);

            var useDl = PlayerPrefs.GetInt(flagKey) == 1;
            useDl = EditorGUILayout.Toggle("use deep link", useDl);
            PlayerPrefs.SetInt(flagKey, useDl ? 1 : 0);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerPrefs.Save();
            }

            if (!EditorApplication.isPlaying && GUILayout.Button("Test Now"))
            {
                PlayerPrefs.SetInt(flagKey,1);
                PlayerPrefs.Save();
                EditorApplication.isPlaying = true;
            }
        }
    }
}