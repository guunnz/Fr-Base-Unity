using UnityEditor;
using UnityEngine;

namespace EditorShortcuts.Editor
{
    public class ShortcutsEditor : EditorWindow
    {
        [MenuItem(Const.GameNameMenu + "Shortcuts")]
        private static void ShowWindow()
        {
            var window = GetWindow<ShortcutsEditor>();
            window.titleContent = new GUIContent("Shortcuts");
            window.Show();
        }
    }
}