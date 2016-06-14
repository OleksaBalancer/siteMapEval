using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ukad_testtask.Models;

namespace ukad_testtask.BL
{
    public class ScanResult
    {
        public List<PageResult> Pages { get; set; }
        public bool IsScanDone { get; set; }

    }
}