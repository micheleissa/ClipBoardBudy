using System.Data.Entity;
using System.IO;

namespace CopyBud.Persistence
{
    public class DbConfig : DbConfiguration
    {
        private readonly bool _isTest;
        public DbConfig(bool isTest = false)
        {
            this._isTest = isTest;
        }

        public DbConfig()
        {
            var executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = (Path.GetDirectoryName(executable));
            if (!this._isTest)
            {
                var intecptor = new System.Data.Entity.Infrastructure.Interception.DatabaseLogger(Path.Combine(path, "DataAccessLogOutput.txt"));
                this.AddInterceptor(intecptor);
            }
        }
    }
}
