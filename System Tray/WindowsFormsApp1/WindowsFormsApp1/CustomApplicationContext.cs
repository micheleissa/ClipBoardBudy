using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class CustomApplicationContext : ApplicationContext
    {
        private static readonly string IconFileName = "copy.ico";
        private static readonly string DefaultTooltip = "Route HOST entries via context menu";
        private System.ComponentModel.IContainer components;
        private NotifyIcon notifyIcon;


        public CustomApplicationContext()
        {
            InitializeContext();
        }

        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(IconFileName),
                Text = DefaultTooltip,
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private Form form;
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            //notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Show &Details", showDetailsItem_Click));
            //notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Help/About", showHelpItem_Click));
//            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Exit", exitItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Show", showItem_Click));
        }

        private void showItem_Click(object sender, EventArgs e)
            {
            if(form == null)
                {
                form = new Form {Visible = true};
                }
            else
                {
                form.Activate();
                }
            }
        private void exitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }
        public ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            return ToolStripMenuItemWithHandler(displayText, 0, 0, eventHandler);
        }
        private ToolStripMenuItem ToolStripMenuItemWithHandler(
            string displayText, int enabledCount, int disabledCount, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }

            item.ToolTipText = (enabledCount > 0 && disabledCount > 0) ?
                                                 string.Format("{0} enabled, {1} disabled", enabledCount, disabledCount)
                         : (enabledCount > 0) ? string.Format("{0} enabled", enabledCount)
                         : (disabledCount > 0) ? string.Format("{0} disabled", disabledCount)
                         : "";
            return item;
        }

        /// <summary>
		/// When the application context is disposed, dispose things like the notify icon.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }
        /// <summary>
        /// If we are presently showing a form, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            // before we exit, let forms clean themselves up.
            //if (introForm != null) { introForm.Close(); }
            //if (detailsForm != null) { detailsForm.Close(); }

            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();

        }
    }
}
