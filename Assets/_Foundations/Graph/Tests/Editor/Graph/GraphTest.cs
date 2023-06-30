using System.Linq;
using Graph.Domain;
using NUnit.Framework;

namespace Graph.Tests.Editor.Graph
{
    public class GraphTest
    {
        private Vertex<string, int> bahia;
        private Edge<string, int> bahiaMedanos;
        private Edge<string, int> bahiaPalta;
        private Edge<string, int> bahiaPehuen;
        private Edge<string, int> bahiaTresa;
        private IGraph<string, int> graph;
        private Vertex<string, int> medanos;
        private Vertex<string, int> palta;
        private Vertex<string, int> pehuen;
        private Edge<string, int> pehuenPalta;
        private Vertex<string, int> tresa;


        [SetUp]
        public void Init()
        {
            graph = new GeneralGraph<string, int>();
        }


        [Test]
        public void BuildGraph_GraphContainsEachPreviousVertex()
        {
            GivenAGraph();
            Assert.IsTrue(graph.Vertices.Contains(bahiaMedanos.previous));
            Assert.IsTrue(graph.Vertices.Contains(bahiaTresa.previous));
            Assert.IsTrue(graph.Vertices.Contains(bahiaPalta.previous));
            Assert.IsTrue(graph.Vertices.Contains(bahiaPehuen.previous));
            Assert.IsTrue(graph.Vertices.Contains(pehuenPalta.previous));
        }

        [Test]
        public void BuildGraph_GraphContainsEachNextVertex()
        {
            GivenAGraph();
            Assert.IsTrue(graph.Vertices.Contains(bahiaMedanos.next));
            Assert.IsTrue(graph.Vertices.Contains(bahiaTresa.next));
            Assert.IsTrue(graph.Vertices.Contains(bahiaPalta.next));
            Assert.IsTrue(graph.Vertices.Contains(bahiaPehuen.next));
            Assert.IsTrue(graph.Vertices.Contains(pehuenPalta.next));
        }

        [Test]
        public void BuildGraph_GraphContainsEachEdge()
        {
            GivenAGraph();
            Assert.IsTrue(graph.Edges.Contains(bahiaMedanos));
            Assert.IsTrue(graph.Edges.Contains(bahiaTresa));
            Assert.IsTrue(graph.Edges.Contains(bahiaPalta));
            Assert.IsTrue(graph.Edges.Contains(bahiaPehuen));
            Assert.IsTrue(graph.Edges.Contains(pehuenPalta));
        }

        [Test]
        public void BuildGraph_NextElemetsAreSame()
        {
            GivenAGraph();
            Assert.AreEqual("medanos", bahiaMedanos.next.element);
            Assert.AreEqual("tresa", bahiaTresa.next.element);
            Assert.AreEqual("palta", bahiaPalta.next.element);
            Assert.AreEqual("pehuen", bahiaPehuen.next.element);
            Assert.AreEqual("palta", pehuenPalta.next.element);
        }

        [Test]
        public void BuildGraph_PreviousElementsAreSame()
        {
            GivenAGraph();
            Assert.AreEqual("bahia", bahiaMedanos.previous.element);
            Assert.AreEqual("bahia", bahiaTresa.previous.element);
            Assert.AreEqual("bahia", bahiaPalta.previous.element);
            Assert.AreEqual("bahia", bahiaPehuen.previous.element);
            Assert.AreEqual("pehuen", pehuenPalta.previous.element);
        }

        [Test]
        public void BuildGraph_EdgesElementsAreSame()
        {
            GivenAGraph();
            Assert.AreEqual(60, bahiaMedanos.element);
            Assert.AreEqual(100, bahiaTresa.element);
            Assert.AreEqual(30, bahiaPalta.element);
            Assert.AreEqual(90, bahiaPehuen.element);
            Assert.AreEqual(60, pehuenPalta.element);
        }

        [Test]
        public void BuildGraph_NextContainsEdgeAsIngoing()
        {
            GivenAGraph();
            Assert.IsTrue(bahiaTresa.next.inGoing.Contains(bahiaTresa));
        }

        [Test]
        public void BuildGraph_PrevContainsEdgeAsOutgoing()
        {
            GivenAGraph();
            Assert.IsTrue(bahiaTresa.previous.outGoing.Contains(bahiaTresa));
        }


        [Test]
        public void RemoveEdge_GraphContainsEachVertex()
        {
            GivenAGraph();
            WhenRemoveBahiaMedanosEdge();
            Assert.IsTrue(graph.Vertices.Contains(bahia));
            Assert.IsTrue(graph.Vertices.Contains(palta));
            Assert.IsTrue(graph.Vertices.Contains(medanos));
            Assert.IsTrue(graph.Vertices.Contains(tresa));
            Assert.IsTrue(graph.Vertices.Contains(pehuen));
        }

        [Test]
        public void RemoveEdge_GraphContainsEachEdgeExceptForTheErasedOne()
        {
            GivenAGraph();
            WhenRemoveBahiaMedanosEdge();
            Assert.IsFalse(graph.Edges.Contains(bahiaMedanos));
            Assert.IsTrue(graph.Edges.Contains(bahiaTresa));
            Assert.IsTrue(graph.Edges.Contains(bahiaPalta));
            Assert.IsTrue(graph.Edges.Contains(bahiaPehuen));
            Assert.IsTrue(graph.Edges.Contains(pehuenPalta));
        }


        [Test]
        public void RemoveEdge_EdgesElementsAreSame()
        {
            GivenAGraph();
            WhenRemoveBahiaMedanosEdge();
            Assert.AreEqual(60, bahiaMedanos.element);
            Assert.AreEqual(100, bahiaTresa.element);
            Assert.AreEqual(30, bahiaPalta.element);
            Assert.AreEqual(90, bahiaPehuen.element);
            Assert.AreEqual(60, pehuenPalta.element);
        }

        [Test]
        public void RemoveEdge_ErasedEdgesMustNotContainsReferencesToVertices()
        {
            GivenAGraph();
            WhenRemoveBahiaMedanosEdge();
            Assert.IsNull(bahiaMedanos.next);
            Assert.IsNull(bahiaMedanos.previous);
        }


        [Test]
        public void RemoveVertex_GraphContainsEachPreviousVertexExceptForTheErasedOne()
        {
            GivenAGraph();
            WhenRemovePehuenVertex();
            Assert.IsTrue(graph.Vertices.Contains(bahiaMedanos.previous));
            Assert.IsTrue(graph.Vertices.Contains(bahiaTresa.previous));
            Assert.IsTrue(graph.Vertices.Contains(bahiaPalta.previous));
        }

        [Test]
        public void RemoveVertex_GraphContainsEachNextVertexExceptForTheErasedOne()
        {
            GivenAGraph();
            WhenRemovePehuenVertex();
            Assert.IsTrue(graph.Vertices.Contains(bahiaMedanos.next));
            Assert.IsTrue(graph.Vertices.Contains(bahiaTresa.next));
            Assert.IsTrue(graph.Vertices.Contains(bahiaPalta.next));
        }

        [Test]
        public void RemoveVertex_ErasedEdgesMustNotContainsReferencesToVertices()
        {
            GivenAGraph();
            WhenRemovePehuenVertex();
            Assert.IsNull(bahiaPehuen.next);
            Assert.IsNull(bahiaPehuen.previous);
            Assert.IsNull(pehuenPalta.next);
            Assert.IsNull(pehuenPalta.previous);
        }

        [Test]
        public void RemoveVertex_GraphContainsEachEdgeExceptForTheErased()
        {
            GivenAGraph();
            WhenRemovePehuenVertex();
            Assert.IsTrue(graph.Edges.Contains(bahiaMedanos));
            Assert.IsTrue(graph.Edges.Contains(bahiaTresa));
            Assert.IsTrue(graph.Edges.Contains(bahiaPalta));
            Assert.IsFalse(graph.Edges.Contains(bahiaPehuen));
            Assert.IsFalse(graph.Edges.Contains(pehuenPalta));
        }


        private void WhenRemovePehuenVertex()
        {
            graph.RemoveVertex(pehuenPalta.previous);
        }


        private void GivenAGraph()
        {
            bahiaMedanos = GivenAnEdge(("bahia", 60, "medanos"));
            bahiaTresa = GivenAnEdge(("bahia", 100, "tresa"));
            bahiaPalta = GivenAnEdge(("bahia", 30, "palta"));
            bahiaPehuen = GivenAnEdge(("bahia", 90, "pehuen"));
            pehuenPalta = GivenAnEdge(("pehuen", 60, "palta"));
            bahia = bahiaMedanos.previous;
            palta = pehuenPalta.next;
            medanos = bahiaMedanos.next;
            tresa = bahiaTresa.next;
            pehuen = pehuenPalta.previous;
        }

        private void WhenRemoveBahiaMedanosEdge()
        {
            graph.RemoveEdge(bahiaMedanos);
        }

        private Edge<string, int> GivenAnEdge((string a, int e, string b) edge)
        {
            var (a, e, b) = edge;
            var va = graph.AddUniqueVertex(a);
            var vb = graph.AddUniqueVertex(b);
            return graph.AddEdge(e, va, vb);
        }
    }
}