using System;
using System.Diagnostics;
using System.Windows.Forms;
using CopyBud.Win32;
using System.Linq;
using CopyBud.Persistence;
using CopyBud.Properties;

namespace CopyBud
{
    public class MainForm : Form
    {
        private RichTextBox ctlClipboardText;
        private IntPtr _ClipboardViewerNext;
        private const int SC_MINIMIZE = 0xF020;
        private readonly HistoryRepository _historyRepository;
        private bool _historyCleared;

    private void MainFrm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Show the clipboard contents in the window 
        /// </summary>
        private void GetClipboardData()
        {
            //
            // Data on the clipboard uses the 
            // IDataObject interface
            //
            IDataObject iData = new DataObject();
            try
            {
                iData = Clipboard.GetDataObject();
                // 
                // Get RTF/Text if it is present
                //
                if (iData.GetDataPresent(DataFormats.Rtf) || iData.GetDataPresent(DataFormats.Text))
                {
                    var lastClipboard = (string)iData.GetData(DataFormats.UnicodeText);
                    if (!_historyRepository.DoesHistoryExist(lastClipboard) && !_historyCleared && lastClipboard != null)
                    {
                        ctlClipboardText.Text += $"{lastClipboard}{Environment.NewLine}";
                        _historyRepository.AddHistory(lastClipboard);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.ErrorStatic, ex));
            }

        }
        public void ClearControls()
        {
            this.ctlClipboardText.Text = "";
            this._historyCleared = true;
        }

        //Taken from https://stackoverflow.com/questions/621577/clipboard-event-c-sharp
        protected override void WndProc(ref Message m)
        {
            switch ((Msgs)m.Msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case Msgs.WM_DRAWCLIPBOARD:

                    Debug.WriteLine("WindowProc DRAWCLIPBOARD: " + m.Msg, "WndProc");

                    GetClipboardData();

                    //
                    // Each window that receives the WM_DRAWCLIPBOARD message 
                    // must call the SendMessage function to pass the message 
                    // on to the next window in the clipboard viewer chain.
                    //
                    User32Wrapper.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    break;


                //
                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                //
                case Msgs.WM_CHANGECBCHAIN:
                    Debug.WriteLine("WM_CHANGECBCHAIN: lParam: " + m.LParam, "WndProc");

                    // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
                    // it should call the SendMessage function to pass the message to the 
                    // next window in the chain, unless the next window is the window 
                    // being removed. In this case, the clipboard viewer should save 
                    // the handle specified by the lParam parameter as the next window in the chain. 

                    //
                    // wParam is the Handle to the window being removed from 
                    // the clipboard viewer chain 
                    // lParam is the Handle to the next window in the chain 
                    // following the window being removed. 
                    if (m.WParam == _ClipboardViewerNext)
                    {
                        //
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain
                        //
                        _ClipboardViewerNext = m.LParam;
                    }
                    else
                    {
                        User32Wrapper.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    }
                    break;
                default:
                    //
                    // Let the form process the messages that we are
                    // not interested in
                    //
                    base.WndProc(ref m);
                    break;

            }

        }

        /// <summary>
        /// Register this form as a Clipboard Viewer application
        /// </summary>
        private void RegisterClipboardViewer()
        {
            _ClipboardViewerNext = User32Wrapper.SetClipboardViewer(this.Handle);
        }

        /// <summary>
        /// Remove this form from the Clipboard Viewer list
        /// </summary>
        private void UnregisterClipboardViewer()
        {
            User32Wrapper.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
        }


        public MainForm(HistoryRepository historyRepository, bool historyCleared = false)
        {
            this._historyRepository = historyRepository;
            this.ctlClipboardText = new RichTextBox();
            this.ClientSize = new System.Drawing.Size(292, 266);
            RegisterClipboardViewer();
            InitializeComponent();
            this._historyCleared = historyCleared;
            try
            {
                var recentHistory = _historyRepository.GetRecentHistory();
                recentHistory.ToList().ForEach(h => this.ctlClipboardText.Text += $"{ h.ClipString} {Environment.NewLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.ErrorStatic,ex));
            }
        }




        private void InitializeComponent()
        {
            this.ctlClipboardText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ctlClipboardText
            // 
            this.ctlClipboardText.Location = new System.Drawing.Point(0, 0);
            this.ctlClipboardText.Name = "ctlClipboardText";
            this.ctlClipboardText.Size = new System.Drawing.Size(306, 371);
            this.ctlClipboardText.TabIndex = 0;
            this.ctlClipboardText.Text = "";
            this.ctlClipboardText.KeyDown += this.ctlClipboardText_KeyDown;
            this.ctlClipboardText.KeyPress += this.ctlClipboardText_KeyPress;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(306, 373);
            this.ControlBox = false;
            this.Controls.Add(this.ctlClipboardText);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "History";
            this.ResumeLayout(false);

        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterClipboardViewer();
        }

        private void ctlClipboardText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Visible = false;
            }
        }

        private void ctlClipboardText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Visible = false;
            }
        }
    }
}