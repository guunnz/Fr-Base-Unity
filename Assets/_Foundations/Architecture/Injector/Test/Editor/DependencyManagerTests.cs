using Architecture.Injector.Core;
using NUnit.Framework;

namespace Architecture.Injector.Test.Editor
{
    [TestFixture]
    public class DependencyManagerTests
    {
        [SetUp]
        public void Initialize()
        {
            dependencyManager = new ConcreteInjector();
        }

        private IDependenciesInjector dependencyManager;


        [Test]
        public void RetrieveSimpleClass()
        {
            dependencyManager.Register<StudyCaseA>();
            var studyCase = dependencyManager.Get<StudyCaseA>();
            Assert.NotNull(studyCase);
        }

        [Test]
        public void RetrieveClassWithParameter()
        {
            dependencyManager.Register<StudyCaseA>();
            dependencyManager.Register<StudyCaseB>();
            var b = dependencyManager.Get<StudyCaseB>();
            var a = dependencyManager.Get<StudyCaseA>();

            Assert.NotNull(a);
            Assert.NotNull(b);
            Assert.NotNull(b.caseA);
            Assert.AreEqual(b.caseA, a);
        }

        [Test]
        public void RetrieveClassWithParameterButNotParameter()
        {
            dependencyManager.Register<StudyCaseB>();
            var b = dependencyManager.Get<StudyCaseB>();

            Assert.NotNull(b);
            Assert.Null(b.caseA);
        }

        [Test]
        public void ClearADependency()
        {
            dependencyManager.Register<StudyCaseA>();
            var studyCase1 = dependencyManager.Get<StudyCaseA>();
            dependencyManager.Clear<StudyCaseA>();
            dependencyManager.Register<StudyCaseA>();
            var studyCase2 = dependencyManager.Get<StudyCaseA>();
            //todo check not null for both
            Assert.AreNotSame(studyCase1, studyCase2);
        }

        [Test]
        public void ClearADependencyMustBecomeNull()
        {
            dependencyManager.Register<StudyCaseA>();
            var studyCase1 = dependencyManager.Get<StudyCaseA>();
            dependencyManager.Clear<StudyCaseA>();
            var studyCase2 = dependencyManager.Get<StudyCaseA>();
            Assert.NotNull(studyCase1);
            Assert.Null(studyCase2);
        }

        [Test]
        public void ResolvedDependencyOneWillBecomeAlwaysSameInstance()
        {
            dependencyManager.Register<StudyCaseA>();
            var studyCase1 = dependencyManager.Get<StudyCaseA>();
            dependencyManager.Clear<StudyCaseA>();
            var studyCase2 = dependencyManager.Get<StudyCaseA>();
            Assert.AreNotSame(studyCase1, studyCase2);
        }
    }


    public class StudyCaseC
    {
        public string x = "";

        [Inject]
        public StudyCaseC()
        {
            x = "no_params";
        }

        public StudyCaseC(StudyCaseA caseA)
        {
            x = "case_A";
        }
    }


    public class StudyCaseB : ICaseB
    {
        public StudyCaseA caseA;

        public StudyCaseB(StudyCaseA caseA)
        {
            this.caseA = caseA;
        }

        public StudyCaseB()
        {
        }
    }

    public interface ICaseB
    {
    }

    public class StudyCaseA : ICaseA
    {
    }

    public interface ICaseA
    {
    }
}