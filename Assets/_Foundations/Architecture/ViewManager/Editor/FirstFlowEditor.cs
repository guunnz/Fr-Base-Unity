// using System;
// using Tools.Collections;
// using UnityEditor;
// using UnityEngine;
//
// namespace Architecture.ViewManager.Editor
// {
//     [CustomEditor(typeof(ViewFirstFlow))]
//     public class FirstFlowEditor : UnityEditor.Editor
//     {
//         private string[] names;
//         private ViewFirstFlow Flow => (ViewFirstFlow) target;
//
//         private void OnEnable()
//         {
//         }
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//
//             EditorGUI.BeginChangeCheck();
//             EditorGUILayout.BeginHorizontal();
//             if (Flow.viewManagerModule)
//             {
//                 names = Flow.viewManagerModule.GetTypesNames().ToArray();
//                 var lineHeight = GUILayout.Height(25);
//                 if (names.Length > 0)
//                 {
//                     GUILayout.Label("First View ",  lineHeight,GUILayout.MinWidth(90));
//                     GUILayout.Space(5);
//                     var index = names.Index(Flow.firstViewName) ?? 0;
//                     index = EditorGUILayout.Popup(index, names,  GUILayout.MaxWidth(200), lineHeight);
//                     Flow.firstViewName = names[index];
//                 }
//                 else
//                 {
//                     GUILayout.Label("Graph is empty");
//                 }
//
//                 GUILayout.Space(5);
//                 if (GUILayout.Button("Edit Graph",  GUILayout.MinWidth(90), lineHeight))
//                 {
//                     Selection.activeObject = Flow.viewManagerModule.config;
//                 }
//             }
//             else
//             {
//                 Flow.viewManagerModule = FindObjectOfType<ViewManagerModule>();
//             }
//
//             EditorGUILayout.EndHorizontal();
//             if (EditorGUI.EndChangeCheck())
//             {
//                 EditorUtility.SetDirty(Flow);
//             }
//         }
//     }
// }