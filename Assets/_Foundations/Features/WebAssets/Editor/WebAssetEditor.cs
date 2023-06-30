using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WebAssets
{
    [CustomEditor(typeof(WebAsset), true)]
    [CanEditMultipleObjects]
    public class WebAssetEditor : UnityEditor.Editor
    {
        private const string LatestVersionName = "<<latest>>";
        private bool alreadyFetch;
        private List<(string version, string json)> versions = new List<(string version, string json)>();

        private string[] versionsOptions = {LatestVersionName};

        private WebAsset Asset => (WebAsset) target;

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Current Build Version is " + Application.version);


            GUILayout.Space(10);
            GUILayout.Label("Data Type: " + Asset.InfoType.Name);
            GUILayout.Space(10);
            if (GUILayout.Button("Fetch") || !alreadyFetch && versions.Count == 0)
            {
                alreadyFetch = true;
                Fetch();
            }

            DrawVersions();

            if (GUILayout.Button("Save")) Save();

            base.OnInspectorGUI();
        }

        private void DrawVersions()
        {
            if (versions.Count == 0)
            {
                GUILayout.Label("No versions on server");
                return;
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Favourite Version");
            var fav = Asset.favouriteVersion;


            var index = Array.IndexOf(versionsOptions, fav);

            //if index not found or index == 0 (it's latest) then use the first one (that will be the latest)
            index = Math.Max(0, index);
            index = EditorGUILayout.Popup(index, versionsOptions);

            var shortIndex = Mathf.Max(0, index - 1);


            var changes = EditorGUI.EndChangeCheck();
            var loadVersionPressed = GUILayout.Button("Load Version");


            if (changes)
            {
                Asset.favouriteVersion = versionsOptions[index];
                EditorUtility.SetDirty(Asset);
                AssetDatabase.SaveAssets();
            }

            if (loadVersionPressed) SetJson(versions[shortIndex].json);
        }

        private void Fetch()
        {
            versions = WebAssetHelper.FetchAll(Asset);
            var list = new List<string> {LatestVersionName};
            list.AddRange(versions.Select(v => v.version));
            versionsOptions = list.ToArray();
        }

        private void SetJson(string json)
        {
            Debug.Log(json);
            Asset.Json = json;
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }

        private void Save()
        {
            WebAssetHelper.Save(Asset);
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
            Fetch();
        }
    }
}