using System.Collections.Generic;
using System.Linq;

namespace Graph.Domain
{
    public class GeneralGraph<TV, TE> : IGraph<TV, TE>
    {
        private readonly List<Edge<TV, TE>> edges = new List<Edge<TV, TE>>();
        private readonly List<Vertex<TV, TE>> vertices = new List<Vertex<TV, TE>>();

        private readonly Queue<Edge<TV, TE>> auxQueue = new Queue<Edge<TV, TE>>();

        public void Clear()
        {
            vertices.ForEach(v => v.inGoing.Clear());
            vertices.ForEach(v => v.outGoing.Clear());
            edges.ForEach(e => e.next = null);
            edges.ForEach(e => e.previous = null);
            edges.Clear();
            vertices.Clear();
        }

        public IReadOnlyList<Vertex<TV, TE>> Vertices => vertices;
        public IReadOnlyList<Edge<TV, TE>> Edges => edges;

        public Vertex<TV, TE> AddVertex(TV element)
        {
            var vertex = new Vertex<TV, TE>(element);
            vertices.Add(vertex);
            return vertex;
        }


        public Vertex<TV, TE> AddUniqueVertex(TV element)
        {
            var vert = vertices.FirstOrDefault(v => v.element.Equals(element));
            return vert ?? AddVertex(element);
        }

        public Edge<TV, TE> AddEdge(TE element, Vertex<TV, TE> previous, Vertex<TV, TE> next)
        {
            var edge = new Edge<TV, TE>(element, previous, next);
            edges.Add(edge);
            previous.outGoing.Add(edge);
            next.inGoing.Add(edge);
            return edge;
        }

        public void RemoveEdge(Edge<TV, TE> edge)
        {
            var prev = edge.previous;
            var next = edge.next;
            prev.outGoing.Remove(edge);
            next.inGoing.Remove(edge);
            edge.previous = null;
            edge.next = null;
            edges.Remove(edge);
        }

        public void RemoveVertex(Vertex<TV, TE> vertex)
        {
            auxQueue.Clear();
            vertex.inGoing.ForEach(auxQueue.Enqueue);
            vertex.outGoing.ForEach(auxQueue.Enqueue);
            vertex.inGoing.Clear();
            vertex.outGoing.Clear();

            while (auxQueue.Any()) RemoveEdge(auxQueue.Dequeue());

            vertices.Remove(vertex);
        }
    }
}