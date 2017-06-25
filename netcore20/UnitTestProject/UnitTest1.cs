using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [Priority(1)]
        public void TestMethod1()
        {
        }

        [TestMethod]
        [Priority(1)]
        public void TestMethod2()
        {
        }

        [TestMethod]
        [Priority(2)]
        public void TestMethod3()
        {
        }

        [TestMethod]
        [Priority(2)]
        public void TestMethod5()
        {
            Assert.Fail(":(");
        }
    }
}
