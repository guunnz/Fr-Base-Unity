using FoundationEditorTools;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ClientTest : EditorWindow
    {
        private string body;

        private string content;
        private string url;

        private void OnGUI()
        {
            url = EditorGUILayout.TextField("url", url);
            body = EditorGUILayout.TextField("body", body);
            EditorGUILayout.Space(10);
            GUILayout.Label(content);
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Get")) EditorWebClient.Get(url, text => content = text);

            if (GUILayout.Button("Post")) EditorWebClient.Post(url, body, text => content = text);

            if (GUILayout.Button("Clear")) content = "-";
        }

        [MenuItem("Tester/Client Tester")]
        private static void ShowWindow()
        {
            var window = GetWindow<ClientTest>();
            window.titleContent = new GUIContent("Web Client On Editor Tester");
            window.Show();
        }
    }
}