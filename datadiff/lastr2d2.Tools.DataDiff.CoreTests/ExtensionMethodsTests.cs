using LastR2D2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LastR2D2.Tools.DataDiff.CoreTests
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "quickbrownfoxjumpsoverthelazydog"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "LastR2D2.Tools.DataDiff.Core.ExtensionMethods.BreakIntoList(System.String,System.Int32)"), TestMethod]
        public void BreakIntoListTest()
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