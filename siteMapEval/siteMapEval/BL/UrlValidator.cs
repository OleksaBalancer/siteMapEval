using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace siteMapEval.BL
{
    public class UrlValidator
    {
        //check if url is valid
        public static bool CheckUrl(string url)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null)
                {
                    return false;
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (UriFormatException)
            {
                return false;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}