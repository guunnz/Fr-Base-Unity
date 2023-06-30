using Shared.Utils;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Editor
{
    [CustomEditor(typeof(ShapedImage), true)]
    [CanEditMultipleObjects]
    public class ShapedImageEditor : ImageEditor
    {
        private SerializedProperty amount;
        private SerializedProperty cornerDivisions;
        private SerializedProperty desiredCornerRadius;

        protected override void OnEnable()
        {
            base.OnEnable();
            cornerDivisions = serializedObject.FindProperty("cornerDivisions");
            amount = serializedObject.FindProperty("amount");
            desiredCornerRadius = serializedObject.FindProperty("desiredCornerRadius");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.Separator();

            var modified = false;
            modified |= EditorGUILayout.PropertyField(cornerDivisions);
            modified |= EditorGUILayout.PropertyField(amount);
            modified |= EditorGUILayout.PropertyField(desiredCornerRadius);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("REPLACE IT"))
            {
                var shapedImage = ((ShapedImage) target);
                var sprite = shapedImage.sprite;
                var color = shapedImage.color;
                var go = shapedImage.gameObject;
                Undo.RecordObjects(new[] {target, go}, "replacement");
                shapedImage.SmartDestroy();
                var image = go.AddComponent<Image>();
                image.sprite = sprite;
                image.color = color;
                

            }
        }
    }
}