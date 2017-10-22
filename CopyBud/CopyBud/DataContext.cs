using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CopyBud
{
    public class DataContext : DbContext
    {
        public DbSet<History> histories { get; set; }

        public DataContext() : base("dbConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
