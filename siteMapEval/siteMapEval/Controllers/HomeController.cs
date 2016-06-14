using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using siteMapEval.BL;
using siteMapEval.BL.Abstract;
using siteMapEval.DAL.Abstract;
using siteMapEval.Models;

namespace siteMapEval.Controllers
{
    public class HomeController : Controller
    {
        IDataService _dataService;

        IPageScannerCreator _scannerCreator;

        public HomeController(IDataService dataservice, IPageScannerCreator scannerCreator)
        {
            _dataService = dataservice;
            _scannerCreator = scannerCreator;
        }

        public ActionResult Index()
        {
            return View();
        }

        public void StartScanning(string url)
        {
            Session["parser"] = null;
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            if (UrlValidator.CheckUrl(url))
            {
                IPageScanner parser = _scannerCreator.GetPageScanner();

                ThreadPool.QueueUserWorkItem(
                    (x) =>
                    {
                        parser.ProcessPage(x as string);
                    },
                   url);
                Session["parser"] = parser;
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            
        }

        //get generated sitemap during processing
        public JsonResult GetSiteMap()
        {
            IPageScanner parser = Session["parser"] as IPageScanner;
            if (parser != null)
            {
                ScanResult result = parser.GetScanResults();
                List<PageResult> pages = result.Pages;
                pages.Sort(new PageResultDescendingComparer());

                double maxTime = 0;
                if (pages.Count > 0)
                {
                    maxTime = pages.Select(x => x.MaxResponseTime).Max();
                }

                return Json(new PagesJsonResult() { IsScanDone = result.IsScanDone, Pages = pages, MaxTime = maxTime }, JsonRequestBehavior.AllowGet);
            }

            return Json(new PagesJsonResult() { IsScanDone = true }, JsonRequestBehavior.AllowGet);
        }

        public void SaveResults()
        {
            IPageScanner parser = Session["parser"] as IPageScanner;
            if (parser != null)
            {
                ScanResult result = parser.GetScanResults();
                List<PageResult> pages = result.Pages;
                pages.Sort(new PageResultDescendingComparer());

                Evaluation eval = new Evaluation() { InitialURL = parser.InitialUrl, Pages = pages, Created = DateTime.Now };

                _dataService.Insert(eval);
                _dataService.Save();
            }
        }

        //get history of evaluations
        public JsonResult GetEvaluations()
        {
            List<Evaluation> result = _dataService.Get<Evaluation>().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //get evaluated sitemap from history
        public JsonResult GetEvaluationResults(int evaluationId)
        {
            List<PageResult> pages = _dataService.Get<PageResult>(x => x.EvaluationID == evaluationId).ToList();

            pages.Sort(new PageResultDescendingComparer());

            return Json(new PagesJsonResult() { IsScanDone = true, Pages = pages, MaxTime = pages.Select(x => x.MaxResponseTime).Max() }, JsonRequestBehavior.AllowGet);
        }

        class PagesJsonResult
        {
            public bool IsScanDone { get; set; }
            public List<PageResult> Pages { get; set; }
            public double MaxTime { get; set; }
        }
    }
}