using System;
using System.Windows.Forms;
using CopyBud.Mutex;
using CopyBud.Persistence;

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
            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }
            try
            {
                var ctx = new DataContext();
                var historyRepo = new HistoryRepository(ctx);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CustomApplicationContext(historyRepo));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            SingleInstance.Stop();
        }
    }
}
