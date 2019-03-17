using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;

namespace CopyBud.Persistence
{
    public class DataContext : DbContext
    {
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<SequenceTable> Sequences { get; set; }

        public DataContext() : base("dbConnection")
        {
            Database.SetInitializer<DataContext>(null);
            DbInterception.Add(new SqliteInterceptor());
            
            #if DEBUG
                this.Database.Log = (l) => Debug.WriteLine(l);
            #endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SequenceTable>()
                            .HasKey(s => s.Name)    
                            .ToTable("sqlite_sequence");

            modelBuilder.Entity<History>().HasKey(h => h.Id);
            modelBuilder.Entity<History>()
                            .Property(h => h.Id)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
