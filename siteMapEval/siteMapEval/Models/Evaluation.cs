using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace siteMapEval.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public string InitialURL { get; set; }
        public DateTime Created { get; set; }

        public ICollection<PageResult> Pages { get; set; }

        public Evaluation()
        {

        }
    }
}