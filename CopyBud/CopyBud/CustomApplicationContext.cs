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
        private MainForm mainFrm;
        public CustomApplicationContext()
            {
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
            exitMenuItem.Click += exitItem_Click;
            showHideMenuItem.Click += showItem_Click;
            _notifyIcon.ContextMenuStrip.Items.Add(showHideMenuItem);
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;
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

        private void showItem_Click( object sender, EventArgs e )
            {
            if(mainFrm == null || mainFrm.IsDisposed)
                {
                mainFrm = new MainForm(){Visible = true};
                }
            else
                {
                mainFrm.Visible = true;
                mainFrm.WindowState = mainFrm.LastState;
                }
            }

        private void exitItem_Click( object sender, EventArgs e )
            {
            ExitThread();
            }

        protected override void Dispose(bool disposing)
            {
            //             Clean up any components being used.
            if (disposing)
                {
                _components?.Dispose();
                }
            base.Dispose(disposing);
            }
    }
}
