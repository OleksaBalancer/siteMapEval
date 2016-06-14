using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using siteMapEval.BL.Abstract;
using siteMapEval.Models;

namespace siteMapEval.BL
{
    public class ParallelPageScanner : PageScanner
    {
        private BlockingCollection<string> _linksToProcess = new BlockingCollection<string>();
        
        private bool _scanStarted = false;
        
        //generate sitemap
        public override void ProcessPage(string url)
        {
            //start worker thread for each processor
            int processorsCount = Environment.ProcessorCount;

            for (int i = 0; i < processorsCount; i++)
            {
                Task.Run(() => ProcessLinks());
            }

            Scan(url);
        }

        //scan page for links and add them to be processed
        private void Scan(string url)
        {
            _scanStarted = true;
            _recursionEntersCount++;
            
            url = CleanLink(url);

            if (String.IsNullOrEmpty(_initialUrl))
            {
                _initialUrl = url; // set initial url if not set yet
            }

            WebClient client = new WebClient();
            client.Headers.Add("user-agent", ".NET Framework");
            try
            {
                string pageText = client.DownloadString(url);//get page test
                client.Dispose();

                string baseURL = ExtractBaseURL(url);//get base url

                List<string> links = ExtractLinks(pageText, baseURL); //get all links on page   
                links.Add(url);//add current url
                links = ExtractInternalLinks(links, baseURL); //get internal links
                links = links.Where(x => !_processedLinks.Contains(x)).ToList();//get unprocessed links
                _processedLinks.AddRange(links);

                //add links to process
                foreach (string link in links)
                {
                    _linksToProcess.Add(link);
                }

                //go down the page
                foreach (string link in links)
                {
                    Scan(link);
                }
            }
            catch (WebException)
            {
            }
            _recursionEntersCount--;
        }
                
        protected override bool CheckIfWorkComplete()
        {
            return _recursionEntersCount == 0 && _linksToProcess.Count == 0 && _scanStarted == true; //not sure if thread safe but don't know any better, seems to be working
        }

        public void ProcessLinks()
        {
            while (!CheckIfWorkComplete())
            {
                string link = _linksToProcess.Take();

                try
                {
                    PageResult prodessedPage = MeasurePage(link, _measureTriesNum);
                    lock (_lockToken)
                    {
                        _processedPages.Add(prodessedPage);
                    }
                }
                catch (WebException)
                {
                }
            }
        }
    }
}
