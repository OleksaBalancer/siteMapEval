using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ukad_testtask.Models;

namespace ukad_testtask.Models
{
    public class PageResult: ICloneable
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public double MaxResponseTime { get; set; }
        public double MinResponseTime { get; set; }
        public int EvaluationID { get; set; }

        public Evaluation Evaluation { get; set; }

        public PageResult()
        {

        }

        public object Clone()
        {
            return new PageResult() { URL = URL, MaxResponseTime = MaxResponseTime, MinResponseTime = MinResponseTime };
        }
    }
}