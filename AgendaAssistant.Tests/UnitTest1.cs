using System;
using AgendaAssistant.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaAssistant.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CodeStringTest()
        {
            var guid = new Guid("34F6C6BF-9352-48ED-8BA4-E95EA8E1AFBE");
            var code = CodeString.GuidAsCodeString(guid);
            var guid2 = CodeString.CodeStringToGuid(code);
            Assert.AreEqual(guid, guid2);
        }
    }
}
