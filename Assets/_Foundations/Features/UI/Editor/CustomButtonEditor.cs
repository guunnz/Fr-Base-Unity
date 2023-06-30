using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Editor
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : UnityEditor.Editor
    {
        private readonly HashSet<Object> relatedObjects = new HashSet<Object>();

        private SerializedProperty backgroundColor;
        private SerializedProperty labelColor;
        private SerializedProperty label;
        private SerializedProperty iconKey;
        private SerializedProperty iconImage;
        private SerializedProperty buttonLabel;
        private SerializedProperty background;
        private IconsCollection collection;
        private string[] iconKeysOptions;


        private Vector2 iconsScroll;

        private void OnEnable()
        {
            collection = Resources.Load<IconsCollection>("Icons");
            iconKeysOptions = collection.Keys();


            backgroundColor = serializedObject.FindProperty(nameof(CustomButton.backgroundColor));
            labelColor = serializedObject.FindProperty(nameof(CustomButton.textColor));
            label = serializedObject.FindProperty(nameof(CustomButton.label));
            iconKey = serializedObject.FindProperty(nameof(CustomButton.iconKey));
            iconImage = serializedObject.FindProperty(nameof(CustomButton.iconImage));
            buttonLabel = serializedObject.FindProperty(nameof(CustomButton.buttonLabel));
            background = serializedObject.FindProperty(nameof(CustomButton.background));
            Refresh();
            ApplyChanges();
        }

        private void OnDisable()
        {
            relatedObjects.Clear();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("My Custom button");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(backgroundColor);
            EditorGUILayout.PropertyField(labelColor);
            EditorGUILayout.PropertyField(label);
            IconKeyProperty();
            EditorGUILayout.PropertyField(iconImage);
            EditorGUILayout.PropertyField(buttonLabel);
            EditorGUILayout.PropertyField(background);

            if (EditorGUI.EndChangeCheck())
            {
                Refresh();
                ApplyChanges();
            }
        }

        private void IconKeyProperty()
        {
            //EditorGUILayout.PropertyField(iconKey);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Icon : "+iconKey.stringValue);
            GUILayout.Space(5);
            var value = iconKey.stringValue;
            var index = collection.GetKeyIndex(value);
            if (index < 0) index = 0;
            index = EditorGUILayout.Popup(index, iconKeysOptions);
            iconKey.stringValue = collection.GetKeyByIndex(index);
            GUILayout.Space(5);

            if (GUILayout.Button("Icons", "box", GUILayout.Width(45), GUILayout.Height(25)))
            {
                Selection.activeObject = collection;
            }

            EditorGUILayout.EndHorizontal();
            iconsScroll = EditorGUILayout.BeginScrollView(iconsScroll);
            EditorGUILayout.BeginHorizontal();

            var sprites = collection.Sprites();
            int i = 0;
            foreach (var sprite in sprites)
            {
                if (GUILayout.Button(sprite.texture, GUILayout.MaxHeight(40), GUILayout.MaxWidth(40)))
                {
                    iconKey.stringValue = collection.GetKeyByIndex(i);
                }
                i++;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private void ApplyChanges()
        {
            serializedObject.ApplyModifiedProperties();

            foreach (var relatedObject in relatedObjects)
            {
                if (relatedObject) EditorUtility.SetDirty(relatedObject);
            }
        }

        private void Refresh()
        {
            if (background.objectReferenceValue is Graphic bgImage)
            {
                bgImage.color = backgroundColor.colorValue;

                relatedObjects.Add(bgImage);
            }

            if (buttonLabel.objectReferenceValue is TextMeshProUGUI text)
            {
                text.color = labelColor.colorValue;
                text.text = label.stringValue;

                relatedObjects.Add(text);
            }

            if (iconImage.objectReferenceValue is Image image)
            {
                image.sprite = collection.FindSprite(iconKey.stringValue);
                relatedObjects.Add(image);
            }
        }
    }
}