using siteMapEval.BL.Abstract;

namespace siteMapEval.BL
{
    public class PageScannerCreator: IPageScannerCreator
    {
        public IPageScanner GetPageScanner()
        {
            //return new PageScanner();
            return new ParallelPageScanner();
        }
    }
}