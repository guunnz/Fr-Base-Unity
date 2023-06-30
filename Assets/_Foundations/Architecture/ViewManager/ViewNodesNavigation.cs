using System;
using System.Collections.Generic;
using System.Linq;
using Graph._3rdparty.xNode.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Architecture.ViewManager
{
    [CreateAssetMenu(menuName = "Bones/View Nodes Navigation")]
    public class ViewNodesNavigation : NodeGraph
    {
        [FormerlySerializedAs("elements")] public List<ViewNode> views;

        public List<ViewNode> popups;

        private Dictionary<Type, ViewNodeVisualization> _nodeToView;

        private Dictionary<Type, ViewNodeVisualization> NodeToView =>
            _nodeToView ??= CreateDictionary(nodes.OfType<ViewNodeVisualization>());


        private void OnValidate()
        {
            UpdateModel();
        }

        public void Init()
        {
#if UNITY_EDITOR
            /*
             * on unity, try to init nodes on execution and not on demand
             * to throw errors early
             */
            var dummy = NodeToView;
#endif
        }

        private Dictionary<Type, ViewNodeVisualization> CreateDictionary(IEnumerable<ViewNodeVisualization> viewNodes)
        {
            var dict = new Dictionary<Type, ViewNodeVisualization>();
            foreach (ViewNodeVisualization node in viewNodes)
            {
                if (node.prefab == null)
                {
                    Debug.LogWarning("There is a node without prefab here! ", node);
                    continue;
                    // throw new Exception("Node without prefab");
                }

                var type = node.prefab.GetType();
//                Debug.Log("Adding node of type: " + type, node);
                dict[type] = node;
            }

            Debug.Log(dict.Count + " views");

            return dict;
        }

        public List<string> Names =>
            nodes.OfType<ViewNodeVisualization>().Select(n => n.prefab.GetType().Name).ToList();

        public ViewNode FromNode(Type type, string outputKey)
        {
            if (NodeToView.TryGetValue(type, out var nodeSource))
                for (var i = 0; i < nodeSource.outputs.Length; i++)
                {
                    var t = nodeSource.outputs[i];
                    if (string.Equals(t, outputKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var port = nodeSource.GetPort("outputs " + i);
                        if (port.Connection != null && port.Connection.node != null)
                            if (port.Connection.node is ViewNodeVisualization nodeDestination)
                                return nodeDestination.prefab;
                    }
                }

            return null;
        }


        public ViewNode Find(Type type)
        {
            return views.Union(popups).First(t => t.GetType() == type);
        }

        public void ValidateNode(ViewNodeVisualization node)
        {
            if (node.prefab && !views.Contains(node.prefab)) views.Add(node.prefab);

            UpdateModel();
        }

        private void UpdateModel()
        {
            var viewNodes = Prefabs.ToList();
            for (var i = 0; i < views.Count; i++)
                if (!viewNodes.Contains(views[i]))
                    views.RemoveAt(i);
        }

        private IEnumerable<ViewNode> Prefabs
        {
            get { return nodes.OfType<ViewNodeVisualization>().Select(n => n.prefab).Union(popups); }
        }

        public void AddAll(List<ViewNode> prefabs, Vector2 viewPosition, Vector2 nodeSize)
        {
            var pos = viewPosition;

            foreach (var viewNode in prefabs)
            {
                var node = AddNode<ViewNodeVisualization>();
                node.prefab = viewNode;
                node.name = viewNode.GetType().Name;
                node.position = pos;
                pos += Vector2.right * nodeSize.x * .5f;
            }
        }

        [Serializable]
        public struct PrefabWithNode
        {
            public ViewNode prefab;
            public ViewNodeVisualization node;
        }

        public Type GetTypeOfName(string typeName)
        {
            return Prefabs.Select(p => p.GetType()).FirstOrDefault(t => t.Name == typeName);
        }
    }
}