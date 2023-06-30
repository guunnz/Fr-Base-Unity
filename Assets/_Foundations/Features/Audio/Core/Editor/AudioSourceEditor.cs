using UnityEditor;
using UnityEngine;

namespace Audio.Core
{
    [CustomEditor(typeof(AudioSource), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class AudioSourceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorApplication.isPlaying) return;

            var source = (AudioSource) target;

            if (!source.clip) return;
            if (source.isPlaying)
            {
                if (GUILayout.Button("STOP", GUILayout.Width(100))) source.Stop();
            }
            else
            {
                if (GUILayout.Button("PLAY", GUILayout.Width(100))) source.Play();
            }
        }
    }
}