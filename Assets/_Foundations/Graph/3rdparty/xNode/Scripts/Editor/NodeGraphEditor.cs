﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Graph._3rdparty.xNode.Scripts.Editor
{
    /// <summary> Base class to derive custom Node Graph editors from. Use this to override how graphs are drawn in the editor. </summary>
    [CustomNodeGraphEditor(typeof(NodeGraph))]
    public class
        NodeGraphEditor : NodeEditorBase<NodeGraphEditor, NodeGraphEditor.CustomNodeGraphEditorAttribute, NodeGraph>
    {
        /// <summary> Are we currently renaming a node? </summary>
        protected bool isRenaming;

        [Obsolete("Use window.position instead")]
        public Rect position
        {
            get => window.position;
            set => window.position = value;
        }

        public virtual void OnGUI()
        {
        }

        /// <summary> Called when opened by NodeEditorWindow </summary>
        public virtual void OnOpen()
        {
        }

        /// <summary> Called when NodeEditorWindow gains focus </summary>
        public virtual void OnWindowFocus()
        {
        }

        /// <summary> Called when NodeEditorWindow loses focus </summary>
        public virtual void OnWindowFocusLost()
        {
        }

        public virtual Texture2D GetGridTexture()
        {
            return NodeEditorPreferences.GetSettings().gridTexture;
        }

        public virtual Texture2D GetSecondaryGridTexture()
        {
            return NodeEditorPreferences.GetSettings().crossTexture;
        }

        /// <summary>
        ///     Return default settings for this graph type. This is the settings the user will load if no previous settings
        ///     have been saved.
        /// </summary>
        public virtual NodeEditorPreferences.Settings GetDefaultPreferences()
        {
            return new NodeEditorPreferences.Settings();
        }

        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
        public virtual string GetNodeMenuName(Type type)
        {
            //Check if type has the CreateNodeMenuAttribute
            Node.CreateNodeMenuAttribute attrib;
            if (NodeEditorUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.menuName;
            return NodeEditorUtilities.NodeDefaultPath(type);
        }

        /// <summary> The order by which the menu items are displayed. </summary>
        public virtual int GetNodeMenuOrder(Type type)
        {
            //Check if type has the CreateNodeMenuAttribute
            Node.CreateNodeMenuAttribute attrib;
            if (NodeEditorUtilities.GetAttrib(type, out attrib)) // Return custom path
                return attrib.order;
            return 0;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu)
        {
            var pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            var nodeTypes = NodeEditorReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            for (var i = 0; i < nodeTypes.Length; i++)
            {
                var type = nodeTypes[i];

                //Get node context menu path
                var path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type
                Node.DisallowMultipleNodesAttribute disallowAttrib;
                var disallowed = false;
                if (NodeEditorUtilities.GetAttrib(type, out disallowAttrib))
                {
                    var typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                // Add node entry to context menu
                if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                else
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        var node = CreateNode(type, pos);
                        NodeEditorWindow.current.AutoConnect(node);
                    });
            }

            menu.AddSeparator("");
            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0)
                menu.AddItem(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(pos));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            menu.AddItem(new GUIContent("Preferences"), false, () => NodeEditorReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }

        /// <summary> Returned gradient is used to color noodles </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual Gradient GetNoodleGradient(NodePort output, NodePort input)
        {
            var grad = new Gradient();

            // If dragging the noodle, draw solid, slightly transparent
            if (input == null)
            {
                var a = GetTypeColor(output.ValueType);
                grad.SetKeys(
                    new[] {new GradientColorKey(a, 0f)},
                    new[] {new GradientAlphaKey(0.6f, 0f)}
                );
            }
            // If normal, draw gradient fading from one input color to the other
            else
            {
                var a = GetTypeColor(output.ValueType);
                var b = GetTypeColor(input.ValueType);
                // If any port is hovered, tint white
                if (window.hoveredPort == output || window.hoveredPort == input)
                {
                    a = Color.Lerp(a, Color.white, 0.8f);
                    b = Color.Lerp(b, Color.white, 0.8f);
                }

                grad.SetKeys(
                    new[] {new GradientColorKey(a, 0f), new GradientColorKey(b, 1f)},
                    new[] {new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f)}
                );
            }

            return grad;
        }

        /// <summary> Returned float is used for noodle thickness </summary>
        /// <param name="output"> The output this noodle comes from. Never null. </param>
        /// <param name="input"> The output this noodle comes from. Can be null if we are dragging the noodle. </param>
        public virtual float GetNoodleThickness(NodePort output, NodePort input)
        {
            return NodeEditorPreferences.GetSettings().noodleThickness;
        }

        public virtual NoodlePath GetNoodlePath(NodePort output, NodePort input)
        {
            return NodeEditorPreferences.GetSettings().noodlePath;
        }

        public virtual NoodleStroke GetNoodleStroke(NodePort output, NodePort input)
        {
            return NodeEditorPreferences.GetSettings().noodleStroke;
        }

        /// <summary> Returned color is used to color ports </summary>
        public virtual Color GetPortColor(NodePort port)
        {
            return GetTypeColor(port.ValueType);
        }

        /// <summary>
        ///     The returned color is used to color the background of the door.
        ///     Usually used for outer edge effect
        /// </summary>
        public virtual Color GetPortBackgroundColor(NodePort port)
        {
            return Color.gray;
        }

        /// <summary> Returns generated color for a type. This color is editable in preferences </summary>
        public virtual Color GetTypeColor(Type type)
        {
            return NodeEditorPreferences.GetTypeColor(type);
        }

        /// <summary> Override to display custom tooltips </summary>
        public virtual string GetPortTooltip(NodePort port)
        {
            var portType = port.ValueType;
            var tooltip = "";
            tooltip = portType.PrettyName();
            if (port.IsOutput)
            {
                var obj = port.node.GetValue(port);
                tooltip += " = " + (obj != null ? obj.ToString() : "null");
            }

            return tooltip;
        }

        /// <summary> Deal with objects dropped into the graph through DragAndDrop </summary>
        public virtual void OnDropObjects(Object[] objects)
        {
            if (GetType() != typeof(NodeGraphEditor)) Debug.Log("No OnDropObjects override defined for " + GetType());
        }

        /// <summary> Create a node and save it in the graph asset </summary>
        public virtual Node CreateNode(Type type, Vector2 position)
        {
            Undo.RecordObject(target, "Create Node");
            var node = target.AddNode(type);
            Undo.RegisterCreatedObjectUndo(node, "Create Node");
            node.position = position;
            if (node.name == null || node.name.Trim() == "") node.name = NodeEditorUtilities.NodeDefaultName(type);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target))) AssetDatabase.AddObjectToAsset(node, target);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            NodeEditorWindow.RepaintAll();
            return node;
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual Node CopyNode(Node original)
        {
            Undo.RecordObject(target, "Duplicate Node");
            var node = target.CopyNode(original);
            Undo.RegisterCreatedObjectUndo(node, "Duplicate Node");
            node.name = original.name;
            AssetDatabase.AddObjectToAsset(node, target);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            return node;
        }

        /// <summary> Return false for nodes that can't be removed </summary>
        public virtual bool CanRemove(Node node)
        {
            // Check graph attributes to see if this node is required
            var graphType = target.GetType();
            var attribs = Array.ConvertAll(
                graphType.GetCustomAttributes(typeof(NodeGraph.RequireNodeAttribute), true),
                x => x as NodeGraph.RequireNodeAttribute);
            if (attribs.Any(x => x.Requires(node.GetType())))
                if (target.nodes.Count(x => x.GetType() == node.GetType()) <= 1)
                    return false;
            return true;
        }

        /// <summary> Safely remove a node and all its connections. </summary>
        public virtual void RemoveNode(Node node)
        {
            if (!CanRemove(node)) return;

            // Remove the node
            Undo.RecordObject(node, "Delete Node");
            Undo.RecordObject(target, "Delete Node");
            foreach (var port in node.Ports)
            foreach (var conn in port.GetConnections())
                Undo.RecordObject(conn.node, "Delete Node");
            target.RemoveNode(node);
            Undo.DestroyObjectImmediate(node);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeGraphEditorAttribute : Attribute,
            INodeEditorAttrib
        {
            public string editorPrefsKey;
            private readonly Type inspectedType;

            /// <summary> Tells a NodeGraphEditor which Graph type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            /// <param name="editorPrefsKey">Define unique key for unique layout settings instance</param>
            public CustomNodeGraphEditorAttribute(Type inspectedType, string editorPrefsKey = "xNode.Settings")
            {
                this.inspectedType = inspectedType;
                this.editorPrefsKey = editorPrefsKey;
            }

            public Type GetInspectedType()
            {
                return inspectedType;
            }
        }
    }
}