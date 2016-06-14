using System.Data.Entity;
using siteMapEval.Models;

namespace siteMapEval.DAL
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext() : base("PagesEvaluationsDB")
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<PageResult> PageResults { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        

    }
}