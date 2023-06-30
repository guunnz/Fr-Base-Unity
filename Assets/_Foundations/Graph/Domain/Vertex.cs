using System.Collections.Generic;

namespace Graph.Domain
{
    public class Vertex<TV, TE>
    {
        public readonly List<Edge<TV, TE>> inGoing = new List<Edge<TV, TE>>();
        public readonly List<Edge<TV, TE>> outGoing = new List<Edge<TV, TE>>();
        public TV element;

        public Vertex(TV element)
        {
            this.element = element;
        }
    }
}