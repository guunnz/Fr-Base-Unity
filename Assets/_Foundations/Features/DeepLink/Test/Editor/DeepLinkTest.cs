using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeepLink.Test.Editor
{
    [TestFixture]
    public class DeepLinkTest
    {
        private IDeepLinkProcess deepLinkProcess;

        [SetUp]
        public void Setup()
        {
            deepLinkProcess = new DeepLinkProcess();
        }


        //com.opticPower.friendbase://deepLink?key1=value1;key2=value2;key3=value3
        [TestCase("com.opticPower.friendbase://deepLink")] //empty
        [TestCase("com.opticPower.friendbase://deepLink?key1=value1", "key1", "value1")]
        [TestCase("com.opticPower.friendbase://deepLink?key1=value1&key2=value2&key3=value3", "key1", "value1", "key2", "value2", "key3", "value3")]
        public void ParseDictionaryCorrectly(string deepLink, params string[] tuples)
        {
            //given
            var expected = ZipToPairs(tuples ?? Array.Empty<string>());

            //when
            var actual = deepLinkProcess.GetInfo(deepLink);

            //then
            AssertEqualTupleArrays(expected, actual);
        }

        private static (string, string)[] ZipToPairs(params string[] array)
        {
            if (array.Length <= 0) return Array.Empty<(string, string)>();
            
            var ret = new (string, string)[array.Length / 2];
            for (var i = 0; i < ret.Length; i++)
            {
                ret[i] = (array[i * 2], array[i * 2 + 1]);
            }

            return ret;
        }


        private void AssertEqualTupleArrays(
            IReadOnlyList<(string key, string value)> expected,
            IReadOnlyList<(string key, string value)> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].key, actual[i].key);
                Assert.AreEqual(expected[i].value, actual[i].value);
            }
        }
    }
}