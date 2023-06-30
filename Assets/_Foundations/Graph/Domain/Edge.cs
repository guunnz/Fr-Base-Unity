namespace Graph.Domain
{
    public class Edge<TV, TE>
    {
        public TE element;
        public Vertex<TV, TE> next;
        public Vertex<TV, TE> previous;

        public Edge(TE element, Vertex<TV, TE> previous, Vertex<TV, TE> next)
        {
            this.element = element;
            this.previous = previous;
            this.next = next;
        }
    }
}