using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Graph.Editor
{
    public class SerializedGraphEditor : EditorWindow
    {
        private readonly NodesEditor nodesEditor = new NodesEditor();

        private SerializableGraph graph;


        private bool showDefaultEditor;

        private void OnEnable()
        {
            nodesEditor.OnEnable();
        }

        private void OnGUI()
        {
            if (!CheckGraph()) return;

            var graphEditorName = "Graph Editor (" + graph.name + ")";
            EditorGUILayout.LabelField(graphEditorName);
            name = graphEditorName;

            UndoRedo();

            showDefaultEditor = GUILayout.Button("default editor", GUILayout.Width(100)) ^ showDefaultEditor;
            if (showDefaultEditor) DefaultEditor();
            EditorGUILayout.EndToggleGroup();


            nodesEditor.Update();
            nodesEditor.ProcessEvents(Event.current);

            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        [MenuItem(Const.GameNameMenu + "Graph Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<SerializedGraphEditor>();
            window.titleContent = new GUIContent("Graph Editor");
            window.Show();
        }


        private bool CheckGraph()
        {
            var serializableGraphs = Selection.objects.OfType<SerializableGraph>().ToList();
            if (serializableGraphs.Count == 1)
            {
                graph = serializableGraphs[0];
            }
            else
            {
                graph = null;
                name = "Graph Editor";
            }

            return graph != null;
        }


        private void TextField(string oldValue, ref string newValue, Action onChange = null)
        {
            var value = GUILayout.TextField(oldValue);
            if (value != newValue)
            {
                Undo.RecordObject(this, "change text from " + oldValue + " to " + value);
                newValue = value;
                onChange?.Invoke();
                EditorUtility.SetDirty(graph);
            }
        }

        private static string EdgeToString(SerializableGraph.SerializedEdge edge)
        {
            return $"({edge.prev}, {edge.element}, {edge.next})";
        }


        private void ProcessEvents(Event current)
        {
            //
        }

        private void UndoRedo()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<<Undo>>"))
            {
                Undo.PerformUndo();
                graph.Deserialize();
            }

            if (GUILayout.Button("<<Redo>>"))
            {
                Undo.PerformRedo();
                graph.Deserialize();
            }

            EditorGUILayout.EndHorizontal();
        }


        private void DefaultEditor()
        {
            for (var i = 0; i < graph.edges.Count; i++)
            {
                var edge = graph.edges[i];
                EditorGUILayout.BeginHorizontal();
                TextField(edge.element, ref edge.element, graph.Deserialize);
                TextField(edge.next, ref edge.next, graph.Deserialize);
                TextField(edge.prev, ref edge.prev, graph.Deserialize);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RecordObject(graph, $"remove edge {EdgeToString(edge)}");
                    graph.edges.RemoveAt(i);
                    graph.Deserialize();
                    EditorUtility.SetDirty(graph);
                }


                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                Undo.RecordObject(graph, "add edge");
                graph.edges.Add(new SerializableGraph.SerializedEdge
                {
                    element = "",
                    next = "",
                    prev = ""
                });
                EditorUtility.SetDirty(graph);
            }

            if (GUILayout.Button("Refresh")) graph.Deserialize();
        }
    }
}