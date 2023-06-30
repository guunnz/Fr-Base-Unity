using UnityEditor;

namespace TargetPlatformTools
{
    [CustomEditor(typeof(TargetSelector))]
    public class TargetSelectorEditor : UnityEditor.Editor
    {
        private static readonly string[] TargetNames = {"Unity Editor", "Android", "IOS", "WebGL"};

        private TargetSelector Selector => target as TargetSelector;

        private void DrawProperty()
        {
            Selector.targets = EditorGUILayout.MaskField($"Target Devices ({Selector.targets})", Selector.targets, TargetNames);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawProperty();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}