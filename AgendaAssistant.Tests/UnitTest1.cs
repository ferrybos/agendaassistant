using System;
using System.Collections;
using AgendaAssistant.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaAssistant.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void WeekDaysTest()
        {
            //1 2 4 8 16 32 64
            var bitArray = new BitArray(BitConverter.GetBytes(127));
            Assert.IsTrue(bitArray[0]);
            Assert.IsTrue(bitArray[1]);
            Assert.IsTrue(bitArray[2]);
            Assert.IsTrue(bitArray[3]);
            Assert.IsTrue(bitArray[4]);
            Assert.IsTrue(bitArray[5]);
            Assert.IsTrue(bitArray[6]);
        }

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
