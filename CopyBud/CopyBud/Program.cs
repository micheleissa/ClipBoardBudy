using System;
using System.Linq;
using System.Windows.Forms;
using Persistence;

namespace CopyBud
{
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
            {
            var ctx = new DataContext();
            //ctx.Database.Log = Console.Write;
            var historyRepo = new HistoryRepository(ctx);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CustomApplicationContext(historyRepo));
            }
        }
}
