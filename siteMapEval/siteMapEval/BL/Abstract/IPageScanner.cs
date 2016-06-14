namespace siteMapEval.BL.Abstract
{
    public interface IPageScanner
    {
        string InitialUrl { get; }

        ScanResult GetScanResults();

        void ProcessPage(string url);
    }
}
