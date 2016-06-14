using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using siteMapEval.Models;

namespace siteMapEval.BL
{
    public class ScanResult
    {
        public List<PageResult> Pages { get; set; }
        public bool IsScanDone { get; set; }

    }
}