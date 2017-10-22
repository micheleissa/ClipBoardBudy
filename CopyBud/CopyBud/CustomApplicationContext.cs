using Persistence;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace CopyBud
{
    public class CustomApplicationContext : ApplicationContext
        {
        private IContainer _components;
        private NotifyIcon _notifyIcon;
        private static readonly string IconFileName = "copy.ico";
        private static readonly string DefaultTooltip = "CopyBud";
        private readonly HistoryRepository _historyRepository;
        private MainForm mainFrm;
        public CustomApplicationContext( HistoryRepository historyRepo )
            {
            this._historyRepository = historyRepo;
            InitializeContext();
            }
        private void InitializeContext()
        {
            _components = new Container();
            _notifyIcon = new NotifyIcon(_components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(IconFileName),
                Text = DefaultTooltip,
                Visible = true
            };
            var exitMenuItem = new ToolStripMenuItem("&Exit");
            var showHideMenuItem = new ToolStripMenuItem("&Show");
            var clearHistoryMenuItem = new ToolStripMenuItem("&Clear History");
            exitMenuItem.Click += ExitItem_Click;
            showHideMenuItem.Click += ShowItem_Click;
            clearHistoryMenuItem.Click += ClearHistoryMenuItem_Click;
            _notifyIcon.ContextMenuStrip.Items.Add(showHideMenuItem);
            _notifyIcon.ContextMenuStrip.Items.Add(clearHistoryMenuItem);
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;
            mainFrm = new MainForm(_historyRepository)
            {
                Visible = false
            };
        }

        private void ClearHistoryMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the history? This action is not reversible. ", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result == DialogResult.Yes)
            {
                _historyRepository.DeleteAll();   
                if (mainFrm != null && !mainFrm.IsDisposed)
                {
                    mainFrm.ClearControls();
                }
            }
        }

        private void notifyIcon_MouseUp( object sender, MouseEventArgs e )
            {
            if (e.Button == MouseButtons.Left)
                {
                var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
                }
            }

        private void notifyIcon_DoubleClick( object sender, EventArgs e )
            {
            throw new NotImplementedException();
            }

        private void ShowItem_Click( object sender, EventArgs e )
            {
            if(mainFrm == null || mainFrm.IsDisposed)
                {
                mainFrm = new MainForm(_historyRepository)
                                {
                                Visible = true
                                };
                }
            else
                {
                mainFrm.Visible = true;
                }
            }

        private void ExitItem_Click( object sender, EventArgs e )
            {
            Application.Exit();
            }

        protected override void Dispose(bool disposing)
            {
            if (disposing)
                {
                _components?.Dispose();
                }
            base.Dispose(disposing);
            }
    }
}
