using System;
using System.Linq;
using System.Reflection;
using Audio.Music;
using UnityEditor;
using UnityEngine;

namespace Audio.Core
{
    [CustomEditor(typeof(ClipsCollection))]
    public class MusicTrackEditor : UnityEditor.Editor
    {
        private const BindingFlags PublicStaticBFlags = BindingFlags.Static | BindingFlags.Public;

        private readonly Lazy<Func<bool>> isPreviewClipPlaying = new Lazy<Func<bool>>(() =>
        {
            var assembly = typeof(AudioImporter).Assembly;
            var type = assembly.GetType("UnityEditor.AudioUtil");
            var methodInfo =
                type.GetMethod("IsPreviewClipPlaying", PublicStaticBFlags, null, Array.Empty<Type>(), null);
            return () => (bool) methodInfo.Invoke(null, Array.Empty<object>());
        });


        // private readonly Lazy<Action<AudioClip>> playClip = new Lazy<Action<AudioClip>>(() =>
        //{
        //    var assembly = typeof(AudioImporter).Assembly;
        //    var type = assembly.GetType("UnityEditor.AudioUtil");
        //    var methodInfo = type.GetMethod("PlayClip", (BindingFlags) 63, null, Array.Empty<Type>(), null);
        //    return (clip) => methodInfo.Invoke(null, new object[] {clip, 0, false});
        //});

        private readonly Lazy<Action> stopAllClips = new Lazy<Action>(() =>
        {
            var assembly = typeof(AudioImporter).Assembly;
            var type = assembly.GetType("UnityEditor.AudioUtil");
            var methodInfo = type.GetMethod("StopAllPreviewClips", PublicStaticBFlags, null, Array.Empty<Type>(), null);
            return () => methodInfo.Invoke(null, Array.Empty<object>());
        });

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (isPreviewClipPlaying.Value() && GUILayout.Button("■", GUILayout.Height(30), GUILayout.Width(30)))
                stopAllClips.Value();
            var asset = target as ClipsCollection;
            if (asset == null || asset.pairs == null)
            {
                Debug.LogError(asset);
                Debug.LogError(asset?.pairs);
                return;
            }

            for (var i = 0; i < asset.pairs.Count; i++)
            {
                var pair = asset.pairs[i];
                if (pair.clip && string.IsNullOrEmpty(pair.id)) pair.id = pair.clip.name;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                pair.clip = (AudioClip) EditorGUILayout.ObjectField(pair.clip, typeof(AudioClip), false);
                EditorGUILayout.Space(5);
                GUILayout.Label("id:");
                pair.id = EditorGUILayout.TextArea(pair.id);
                asset.pairs[i] = pair;
                EditorGUILayout.Space(5);

                if (GUILayout.Button("X", GUILayout.Height(30), GUILayout.Width(30)))
                {
                    asset.pairs.RemoveAt(i);
                    EditorUtility.SetDirty(asset);
                    EditorGUILayout.EndHorizontal();
                    return; // to avoid weird errors
                }


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (pair.clip)
                {
                    var clipEditor = CreateEditor(pair.clip);
                    GUILayout.Box("", GUILayout.Height(50), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                    var rect = GUILayoutUtility.GetLastRect();
                    clipEditor.OnInteractivePreviewGUI(rect, GUIStyle.none);
                }

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.Space(5);


                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(asset);
            }


            var clips = Selection.objects.OfType<AudioClip>().ToList();

            if (clips.Any())
                if (GUILayout.Button("Add Clips"))
                {
                    var pairs = clips.Select(clip => new PairTrackID
                    {
                        clip = clip,
                        id = clip.name
                    });
                    asset.pairs.AddRange(pairs);
                    AssetDatabase.SaveAssets();
                }

            if (GUILayout.Button("Save")) AssetDatabase.SaveAssets();
        }
    }
}