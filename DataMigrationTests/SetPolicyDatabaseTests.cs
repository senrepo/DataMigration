using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataMigrationTests
{
    [TestClass]
    public class SetPolicyDatabaseTests
    {
        [TestMethod]
        public void TestGetPolicyTerm()
        {
            var setPolicyDatabase = new SetPolicyDatabaseStub();
            Assert.AreEqual(0, setPolicyDatabase.GetPolicyTermStub(null));
            Assert.AreEqual(0, setPolicyDatabase.GetPolicyTermStub(string.Empty));
            Assert.AreEqual(0, setPolicyDatabase.GetPolicyTermStub("12345600"));
            Assert.AreEqual(89, setPolicyDatabase.GetPolicyTermStub("S  123456789"));
        }

    }
}
