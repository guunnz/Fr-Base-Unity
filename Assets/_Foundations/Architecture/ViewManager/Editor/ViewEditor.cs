using Shared.Utils;
using UnityEditor;
using UnityEngine;

namespace Architecture.ViewManager.Editor
{
    [CustomEditor(typeof(ViewNode), true)]
    public class ViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (((ViewNode) target).gameObject.scene.IsValid())
            {
                base.OnInspectorGUI();
                return;
            }

            if (GUILayout.Button("Spawn"))
            {
                var manager = FindObjectOfType<ViewManagerModule>();
                if (manager && manager.canvas && manager.canvas.transform)
                {
                    manager.canvas.gameObject.DestroyChildren();
                    var canvasTransform = manager.canvas.transform;
                    var instance = Instantiate(target as ViewNode, canvasTransform);
                    Selection.activeGameObject = instance.gameObject;
                }
            }

            EditorGUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
}