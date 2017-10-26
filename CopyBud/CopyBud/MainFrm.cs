using System;
using System.Diagnostics;
using System.Windows.Forms;
using CopyBud.Win32;
using System.Linq;
using CopyBud.Persistence;

namespace CopyBud
{
    public class MainForm : System.Windows.Forms.Form
    {
        private RichTextBox ctlClipboardText;
        private IntPtr _ClipboardViewerNext;
        private const int SC_MINIMIZE = 0xF020;
        private readonly HistoryRepository _historyRepository;

        private void MainFrm_Load(object sender, System.EventArgs e)
        {
            RegisterClipboardViewer();
        }

        private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterClipboardViewer();
        }
        /// <summary>
        /// Show the clipboard contents in the window 
        /// and show the notification balloon if a link is found
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
                    var lastClipboard = (string)iData.GetData(DataFormats.Text);
                    if (!_historyRepository.DoesHistoryExist(lastClipboard))
                    {
                        ctlClipboardText.Text += $"{lastClipboard}{Environment.NewLine}";
                        _historyRepository.AddHistory((string)iData.GetData(DataFormats.Text));
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error has occurred: {ex}");
                return;
            }
            
        }
        public void ClearControls()
        {
            this.ctlClipboardText.Text = "";
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
        public MainForm(HistoryRepository historyRepository)
        {
            this._historyRepository = historyRepository;
            this.ctlClipboardText = new RichTextBox();
            this.ClientSize = new System.Drawing.Size(292, 266);
            InitializeComponent();
        }




        private void InitializeComponent()
        {
            this.ctlClipboardText = new RichTextBox();
            this.SuspendLayout();
            // 
            // ctlClipboardText
            // 
            this.ctlClipboardText.Location = new System.Drawing.Point(0, 0);
            this.ctlClipboardText.Name = "ctlClipboardText";
            this.ctlClipboardText.Size = new System.Drawing.Size(721, 371);
            this.ctlClipboardText.TabIndex = 0;
            this.ctlClipboardText.Text = "";
            try
            {
                var recentHistory = _historyRepository.GetRecentHistory();
                recentHistory.ToList().ForEach(h => this.ctlClipboardText.Text += $"{ h.ClipString} {Environment.NewLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error has occurred: {ex}");
            }
            
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(722, 462);
            this.Controls.Add(this.ctlClipboardText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "History";
            this.FormClosing += new FormClosingEventHandler(this.MainFrm_FormClosing);
            this.Load += new EventHandler(this.MainFrm_Load);
            this.ResumeLayout(false);
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterClipboardViewer();
        }
    }
}