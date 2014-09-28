using Microsoft.VisualStudio.TestTools.UnitTesting;
using lastr2d2.Tools.DataDiff.Core;

namespace lastr2d2.Tools.DataDiff.CoreTests
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void BreakIntoList_Test()
        {
            var source = "quickbrownfoxjumpsoverthelazydog";
            var count = 5;
            var pieces = source.BreakIntoList(count);
            Assert.IsNotNull(pieces);
            Assert.AreEqual(pieces.Count, 7);
            Assert.AreEqual(pieces[0], "quick");
            Assert.AreEqual(pieces[6], "og");
        }
    }
}
