using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace WindowsFormsApp1.Win32
{
    public class Form1 : System.Windows.Forms.Form
    {
        private RichTextBox ctlClipboardText;
        //private System.ComponentModel.IContainer components;
        private IntPtr _ClipboardViewerNext;

        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        private void frmMain_Load(object sender, System.EventArgs e)
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
            string strText = "clipmon";

            try
            {
                iData = Clipboard.GetDataObject();
            }
            catch (System.Runtime.InteropServices.ExternalException externEx)
            {
                // Copying a field definition in Access 2002 causes this sometimes?
                Debug.WriteLine("InteropServices.ExternalException: {0}", externEx.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            // 
            // Get RTF if it is present
            //
            if (iData.GetDataPresent(DataFormats.Rtf))
            {
                ctlClipboardText.Rtf = (string)iData.GetData(DataFormats.Rtf);

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    strText = "RTF";
                }
            }
            else
            {
                // 
                // Get Text if it is present
                //
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    ctlClipboardText.Text += $"{(string)iData.GetData(DataFormats.Text)}{System.Environment.NewLine}";

                    strText = "Text";

                    Debug.WriteLine((string)iData.GetData(DataFormats.Text));
                }
                else
                {
                    //
                    // Only show RTF or TEXT
                    //
                    ctlClipboardText.Text = "(cannot display this format)";
                }
            }

            //notAreaIcon.Tooltip = strText;

            //if (ClipboardSearch(iData))
            //{
            //    //
            //    // Found some new links
            //    //
            //    System.Text.StringBuilder strBalloon = new System.Text.StringBuilder(100);

            //    foreach (string objLink in _hyperlink)
            //    {
            //        strBalloon.Append(objLink.ToString() + "\n");
            //    }

            //    FormatMenuBuild(iData);
            //    SupportedMenuBuild(iData);
            //    ContextMenuBuild();

            //    if (_hyperlink.Count > 0)
            //    {
            //        notAreaIcon.BalloonDisplay(NotificationAreaIcon.NOTIFYICONdwInfoFlags.NIIF_INFO, "Links", strBalloon.ToString());
            //    }
            //}
        }

        protected override void WndProc(ref Message m)
        {
            switch ((Win32.Msgs)m.Msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case Win32.Msgs.WM_DRAWCLIPBOARD:

                    Debug.WriteLine("WindowProc DRAWCLIPBOARD: " + m.Msg, "WndProc");

                    GetClipboardData();

                    //
                    // Each window that receives the WM_DRAWCLIPBOARD message 
                    // must call the SendMessage function to pass the message 
                    // on to the next window in the clipboard viewer chain.
                    //
                    Win32.User32.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    break;


                //
                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                //
                case Win32.Msgs.WM_CHANGECBCHAIN:
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
                        Win32.User32.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
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
            _ClipboardViewerNext = User32.SetClipboardViewer(this.Handle);
        }

        /// <summary>
        /// Remove this form from the Clipboard Viewer list
        /// </summary>
        private void UnregisterClipboardViewer()
        {
            User32.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
        }
        public Form1()
        {
            //this.components = new System.ComponentModel.Container();
            this.ctlClipboardText = new RichTextBox();
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Text = "Notify Icon Example";
            InitializeComponent();


        }

        protected override void Dispose(bool disposing)
        {
            // Clean up any components being used.
            if (disposing)
                //if (components != null)
                //    components.Dispose();

            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.ctlClipboardText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ctlClipboardText
            // 
            this.ctlClipboardText.Location = new System.Drawing.Point(2, 28);
            this.ctlClipboardText.Name = "ctlClipboardText";
            this.ctlClipboardText.Size = new System.Drawing.Size(270, 195);
            this.ctlClipboardText.TabIndex = 0;
            this.ctlClipboardText.Text = "";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.ctlClipboardText);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterClipboardViewer();
        }
    }
}