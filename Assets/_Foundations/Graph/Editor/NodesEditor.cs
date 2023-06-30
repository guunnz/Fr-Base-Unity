using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Graph.Editor
{
    public class NodesEditor
    {
        private readonly List<VisualNode> nodes = new List<VisualNode>();

        private GUIStyle nodeStyle;


        public void OnEnable()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        public void Update()
        {
            foreach (var visualNode in nodes) visualNode.Draw();
        }


        public void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1) ProcessContextMenu(e.mousePosition);
                    break;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            nodes.Add(new VisualNode(mousePosition, 200, 50, nodeStyle));
        }
    }
}