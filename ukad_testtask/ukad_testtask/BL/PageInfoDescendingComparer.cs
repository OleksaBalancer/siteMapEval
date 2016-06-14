using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ukad_testtask.Models;

namespace ukad_testtask.BL
{
    public class PageResultDescendingComparer : IComparer<PageResult>
    {
        public int Compare(PageResult x, PageResult y)
        {
            if( (x.MaxResponseTime + x.MinResponseTime)/2 > (y.MaxResponseTime + y.MinResponseTime) / 2)
            {
                return -1;
            }
            if ((x.MaxResponseTime + x.MinResponseTime) / 2 < (y.MaxResponseTime + y.MinResponseTime) / 2)
            {
                return 1;
            }
            return 0;
        }
    }
}