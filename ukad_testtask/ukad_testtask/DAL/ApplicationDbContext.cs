using System.Data.Entity;
using ukad_testtask.Models;

namespace ukad_testtask.DAL
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