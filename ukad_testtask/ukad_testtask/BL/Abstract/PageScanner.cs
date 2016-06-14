using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using ukad_testtask.Models;

namespace ukad_testtask.BL.Abstract
{
    public abstract class PageScanner : IPageScanner
    {
        //collection for results
        protected List<PageResult> _processedPages = new List<PageResult>();

        //collection for storing processed links
        protected List<string> _processedLinks = new List<string>();

        //how many requests will be sent to the url being tested
        protected int _measureTriesNum = 4;

        protected object _lockToken = new object();

        //helps to identify when scanning is complete
        protected int _recursionEntersCount = 0;

        //initial url, from which scanning started
        protected string _initialUrl = String.Empty;

        public string InitialUrl { get { return _initialUrl; } }

        //get intermediate results
        public ScanResult GetScanResults()
        {
            lock (_lockToken)
            {
                return new ScanResult() { Pages = _processedPages.Select(item => (PageResult)item.Clone()).ToList(), IsScanDone = CheckIfWorkComplete() };
            }

        }

        public abstract void ProcessPage(string url);

        protected string RemoveQueryString(string url)
        {
            if (url.Contains('?'))
            {
                return url.Substring(0, url.IndexOf("?"));
            }
            return url;
        }

        protected List<string> ExtractLinks(string pageText, string baseURL)
        {
            List<string> links = new List<string>();

            Regex linkRegex = new Regex(@"<a[^>]*\shref\s*=(?:\s*(?:""|')((https:\/\/|http:\/\/)?[^#""'.][^#""'.].*?)(?:""|'))", RegexOptions.IgnoreCase);
            if (linkRegex.IsMatch(pageText))
            {
                foreach (Match match in linkRegex.Matches(pageText))
                {
                    string link = match.Groups[1].Value;
                    //check if not empty and points to resource
                    if (!String.IsNullOrEmpty(link)
                        && !link.Contains("callto:")
                        && !link.Contains("tel:")
                        && !link.Contains("wtai:")
                        && !link.Contains("sms:")
                        && !link.Contains("market:")
                        && !link.Contains("geopoint:")
                        && !link.Contains("ymsgr:")
                        && !link.Contains("msnim:")
                        && !link.Contains("gtalk:")
                        && !link.Contains("skype:")
                        && !link.Contains("sip:")
                        && !link.Contains("mailto:")
                        && !link.Contains("mail:")
                        && !link.Contains("javascript:")
                        )
                    {
                        link = CleanLink(link);

                        //if internal link - add base url
                        if (!link.StartsWith("https://") && !link.StartsWith("http://") && !link.StartsWith("//"))
                        {
                            if (!link.StartsWith("/"))
                            {
                                link = "/" + link;
                            }
                            link = baseURL + link;
                        }

                        //check if already exists before adding
                        if (!links.Contains(link))
                        {
                            links.Add(link);
                        }
                    }
                }
            }
            return links;
        }

        protected string CleanLink(string link)
        {
            //remove query string
            link = RemoveQueryString(link);

            //if begins with 'www.' - remove it
            if (link.StartsWith("http://www.") || link.StartsWith("https://www.") || link.StartsWith("//www."))
            {
                link = link.Replace("www.", String.Empty);
            }

            //if link starts with '//' add protocol
            if (link.StartsWith("//"))
            {
                link = "http:" + link;
            }

            //trim trailing '/'
            if (link.EndsWith("/"))
            {
                link = link.TrimEnd('/');
            }

            return link;
        }

        protected string ExtractBaseURL(string url)
        {
            int index = 0;

            if (url.StartsWith("http://www.") || url.StartsWith("https://www."))
            {
                url = url.Replace("www.", String.Empty);
            }
            if (url.StartsWith("http://"))
            {
                index = 7;
            }
            if (url.StartsWith("https://"))
            {
                index = 8;
            }

            string cleanAddress = url.Substring(index);

            if (cleanAddress.Contains('/'))
            {
                int baseUrlEnd = cleanAddress.IndexOf('/');
                return url.Substring(0, index + baseUrlEnd);
            }
            return url;
        }

        protected List<string> ExtractInternalLinks(List<string> links, string baseUrl)
        {
            return links.Where(x => x.Contains(baseUrl)).ToList();
        }

        protected PageResult MeasurePage(string url, int triesNum)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = ".NET Framework";
            Stopwatch timer = new Stopwatch();

            List<TimeSpan> responseTimes = new List<TimeSpan>();


            for (int i = 0; i < triesNum; i++)
            {
                timer.Start();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                timer.Stop();
                responseTimes.Add(timer.Elapsed);
                response.Dispose();
            }

            return new PageResult() { URL = url, MaxResponseTime = responseTimes.Max().TotalMilliseconds, MinResponseTime = responseTimes.Min().TotalMilliseconds };
        }

        protected abstract bool CheckIfWorkComplete();
        
    }
}