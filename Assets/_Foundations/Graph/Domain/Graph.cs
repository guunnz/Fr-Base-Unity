using System.Collections.Generic;

namespace Graph.Domain
{
    public interface IGraph<TV, TE>
    {
        IReadOnlyList<Vertex<TV, TE>> Vertices { get; }
        IReadOnlyList<Edge<TV, TE>> Edges { get; }
        void Clear();
        Vertex<TV, TE> AddVertex(TV element);

        Vertex<TV, TE> AddUniqueVertex(TV element);
        Edge<TV, TE> AddEdge(TE element, Vertex<TV, TE> previous, Vertex<TV, TE> next);

        void RemoveEdge(Edge<TV, TE> edge);
        void RemoveVertex(Vertex<TV, TE> vertex);
    }
}