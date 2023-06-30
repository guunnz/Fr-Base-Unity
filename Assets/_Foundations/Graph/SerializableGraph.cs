using System;
using System.Collections.Generic;
using System.Linq;
using Graph.Domain;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Graph
{
    [CreateAssetMenu(menuName = "Bones/Graph")]
    public class SerializableGraph : ScriptableObject, IGraph<string, string>
    {
        public List<SerializedEdge> edges = new List<SerializedEdge>();

        private readonly IGraph<string, string> delegated = new GeneralGraph<string, string>();


        public void Clear()
        {
            delegated.Clear();
        }

        public IReadOnlyList<Vertex<string, string>> Vertices => delegated.Vertices;

        public IReadOnlyList<Edge<string, string>> Edges => delegated.Edges;

        public Vertex<string, string> AddVertex(string element)
        {
            return delegated.AddVertex(element);
        }

        public Vertex<string, string> AddUniqueVertex(string element)
        {
            return delegated.AddUniqueVertex(element);
        }

        public Edge<string, string> AddEdge(string element, Vertex<string, string> previous,
            Vertex<string, string> next)
        {
            var serializedEdge = new SerializedEdge
            {
                element = element,
                next = next.element,
                prev = next.element
            };
            edges.Add(serializedEdge);
            Dirty();
            return CreateEdge(serializedEdge);
        }

        public void RemoveEdge(Edge<string, string> edge)
        {
            delegated.RemoveEdge(edge);
            Serialize();
            Dirty();
        }

        public void RemoveVertex(Vertex<string, string> vertex)
        {
            delegated.RemoveVertex(vertex);
            Serialize();
            Dirty();
        }

        public void Deserialize()
        {
            Clear();
            foreach (var edge in edges) CreateEdge(edge);
        }

        public void Serialize()
        {
            edges = delegated.Edges.Select(e => new SerializedEdge
            {
                element = e.element,
                next = e.next.element,
                prev = e.previous.element
            }).ToList();
        }

        private Edge<string, string> CreateEdge(SerializedEdge edge)
        {
            var prev = delegated.AddUniqueVertex(edge.prev);
            var next = delegated.AddUniqueVertex(edge.next);
            return delegated.AddEdge(edge.element, prev, next);
        }

        private void Dirty()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        [Serializable]
        public struct SerializedEdge
        {
            public string prev;
            public string element;
            public string next;
        }
    }
}