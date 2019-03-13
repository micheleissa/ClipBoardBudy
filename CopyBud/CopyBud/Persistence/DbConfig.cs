using System.Data.Entity;
using System.IO;

namespace CopyBud.Persistence
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            var executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = (Path.GetDirectoryName(executable));
            var intecptor = new System.Data.Entity.Infrastructure.Interception.DatabaseLogger(Path.Combine(path, "DataAccessLogOutput.txt"));
            this.AddInterceptor(intecptor);
        }
    }
}
