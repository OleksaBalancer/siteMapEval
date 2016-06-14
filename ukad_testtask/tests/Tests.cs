using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ukad_testtask.BL;

namespace tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void ProcessingPageTest()
        {
            string url = "http://makelikeatree.org/";

            SequentialPageScanner parser = new SequentialPageScanner();

            parser.ProcessPage(url);

            Assert.AreEqual(10,parser.GetScanResults().Pages.Count);            
        }
    }
}
