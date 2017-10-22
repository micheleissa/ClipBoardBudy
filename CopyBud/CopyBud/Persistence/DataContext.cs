using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DbSet<History> Histories { get; set; }
        public DbSet<SequenceTable> Sequences { get; set; }

        public DataContext() : base("dbConnection")
        {
            Database.SetInitializer<DataContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SequenceTable>()
                            .HasKey(s => s.Name)    
                            .ToTable("sqlite_sequence");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
