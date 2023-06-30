using System.Linq;
using CloudStorage.Infrastructure;
using NUnit.Framework;

namespace CloudStorage.Test.Editor
{
    public class CloudStorageServiceTest
    {
        private CloudStorageService target;

        [SetUp]
        public void Setup()
        {
            target = new CloudStorageService();
        }


        [TestCase(new[]
        {
            "1.2.1",
            "5.0.3",
            "1.2.0",
            "1.3.1",
            "1.2",
            "2.0.1",
            "5.0"
        }, new[]
        {
            "5.0.3",
            "5.0",
            "2.0.1",
            "1.3.1",
            "1.2.1",
            "1.2.0",
            "1.2"
        })]
        [TestCase(new[] {"1.1.1", "12.0.1"}, new[] {"12.0.1", "1.1.1"})] // more than one char 
        [TestCase(new[] {"1.0.1", "1.0.4", "1.0.5", "1.0.3", "1.0.2"},
            new[] {"1.0.5", "1.0.4", "1.0.3", "1.0.2", "1.0.1"})] // more than one char 
        public void SortByVersion(string[] unsorted, string[] sorted)
        {
            //given
            var infos = unsorted.Select(ver => (version: ver, 0)).ToList();

            //when
            CloudStorageService.SortByVersion(infos);

            //then

            for (var i = 0; i < infos.Count; i++) Assert.AreEqual(sorted[i], infos[i].version);
        }
    }
}