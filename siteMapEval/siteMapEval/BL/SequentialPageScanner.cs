using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using siteMapEval.BL.Abstract;
using siteMapEval.Models;

namespace siteMapEval.BL
{
    public class SequentialPageScanner : PageScanner
    {      
        //scan page for links and measure them
        public override void ProcessPage(string url)
        {
            _recursionEntersCount++;
            if (String.IsNullOrEmpty(_initialUrl))
            {
                _initialUrl = url; // set initial urrl if not set yet
            }

            if (url.EndsWith("/"))
            {
                url = url.TrimEnd('/');
            }

            WebClient client = new WebClient();
            client.Headers.Add("user-agent", ".NET Framework");
            try
            {
                string pageText = client.DownloadString(url);//get page test
                client.Dispose();

                string baseURL = ExtractBaseURL(url);//get base url

                List<string> links = ExtractLinks(pageText, baseURL); //get all links on page                                
                links = ExtractInternalLinks(links, baseURL); //get internal links
                links = links.Where(x => !_processedLinks.Contains(x)).ToList();//get unprocessed links
                _processedLinks.AddRange(links);

                //measure response times from every link on page
                foreach (string link in links)
                {
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

                //go down the page
                foreach (string link in links)
                {
                    ProcessPage(link);
                }
            }
            catch (WebException)
            {
            }
            _recursionEntersCount--;
        }
         
        protected override bool CheckIfWorkComplete()
        {
            return _recursionEntersCount == 0;
        }
    }

}
