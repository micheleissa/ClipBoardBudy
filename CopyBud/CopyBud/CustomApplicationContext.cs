using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CopyBud.Persistence;
using CopyBud.Properties;

namespace CopyBud
{
    public class CustomApplicationContext : ApplicationContext
    {
        private IContainer _components;
        private NotifyIcon _notifyIcon;
        private static readonly string IconFileName = "copy.ico";
        private static readonly string DefaultTooltip = "CopyBud";
        private readonly HistoryRepository _historyRepository;
        private bool _isHistoryCleared;
        private MainForm _mainFrm;
        private SearchFrm _searchFrm;
        private bool _isShownFlag;
        public CustomApplicationContext(HistoryRepository historyRepo)
        {
            this._isShownFlag = false;
            this._historyRepository = historyRepo;
            InitializeContext();
        }
        private void InitializeContext()
        {
            _mainFrm = new MainForm(_historyRepository)
            {
                Visible = false
            };
            _components = new Container();
            _notifyIcon = new NotifyIcon(_components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(IconFileName),
                Text = DefaultTooltip,
                Visible = true
            };
            var exitMenuItem = new ToolStripMenuItem("&Exit");
            var showHideMenuItem = new ToolStripMenuItem("&Show / Hide");
            var clearHistoryMenuItem = new ToolStripMenuItem("&Clear History");
            var searchHistoryMenuItem = new ToolStripMenuItem("&Find");
            searchHistoryMenuItem.Click += SearchHistoryItem_Click;
            exitMenuItem.Click += ExitItem_Click;
            showHideMenuItem.Click += ShowItem_Click;
            clearHistoryMenuItem.Click += ClearHistoryMenuItem_Click;
            _notifyIcon.ContextMenuStrip.Items.Add(showHideMenuItem);
            _notifyIcon.ContextMenuStrip.Items.Add(searchHistoryMenuItem);
            _notifyIcon.ContextMenuStrip.Items.Add(clearHistoryMenuItem);
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        private void SearchHistoryItem_Click(object sender, EventArgs e)
        {
            if (_searchFrm == null || _searchFrm.IsDisposed)
            {
                _searchFrm = new SearchFrm(_historyRepository) { Visible = true };
            }
            else
            {
                _searchFrm.Visible = true;
            }
        }

        private void ClearHistoryMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Resources.ClearHistory, Resources.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _historyRepository.DeleteAll();
                    _isHistoryCleared = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Resources.ErrorStatic, ex));
                }

                if (_mainFrm != null && !_mainFrm.IsDisposed)
                {
                    _mainFrm.ClearControls();

                }
            }
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowItem_Click(object sender, EventArgs e)
        {
            if (_mainFrm != null && _isShownFlag && _mainFrm.Visible)
            {
                _mainFrm.Visible = false;
            }
            else
            {
                if (_mainFrm == null || _mainFrm.IsDisposed)
                {
                    _mainFrm = new MainForm(_historyRepository, _isHistoryCleared)
                    {
                        Visible = true
                    };
                }
                else
                {
                    _mainFrm.Visible = true;
                }
            }
            this._isShownFlag = !this._isShownFlag;
        }

        private void ExitItem_Click(object sender, EventArgs e)
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
