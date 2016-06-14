using ukad_testtask.BL.Abstract;

namespace ukad_testtask.BL
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