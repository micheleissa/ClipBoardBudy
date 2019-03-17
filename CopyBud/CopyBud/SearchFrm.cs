using CopyBud.Persistence;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CopyBud
{
    public partial class SearchFrm : Form
    {
        private readonly HistoryRepository _historyRepository;
        public SearchFrm(HistoryRepository historyRepository)
        {
            this._historyRepository = historyRepository;
            InitializeComponent();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.searchTextBox.Text))
            {
                return;
            }
            this.ResultRichTextBox.Text = "";
            var result = _historyRepository.Search(this.searchTextBox.Text);
            result.ForEach(x => this.ResultRichTextBox.Text += $"{x.ClipString}{Environment.NewLine}");
            if (result.Any())
            {
                foreach (var item in result)
                {
                    resultdgv.Rows.Clear();
                    var index = resultdgv.Rows.Add();
                    resultdgv.Rows[index].Cells["ValueCol"].Value = item.ClipString;
                    resultdgv.Rows[index].Cells["TimestampCol"].Value = item.DateTimeTaken;
                }
            }
            else
            {
                resultdgv.Rows.Clear();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            this.ResultRichTextBox.Text = "";
            this.resultdgv.Rows.Clear();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To be implemented!!");
        }
        private GroupBox searchGroupBox;
        private Button searchButton;
        private TextBox searchTextBox;
        private Label searchLabel;
        private DataGridView resultdgv;
        private RichTextBox ResultRichTextBox;
        private Button clearButton;
        private DataGridViewTextBoxColumn ValueCol;
        private DataGridViewTextBoxColumn TimestampCol;








        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.exportButton = new System.Windows.Forms.Button();
            this.ResultRichTextBox = new System.Windows.Forms.RichTextBox();
            this.searchGroupBox = new System.Windows.Forms.GroupBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.resultdgv = new System.Windows.Forms.DataGridView();
            this.ValueCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimestampCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.searchGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultdgv)).BeginInit();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(386, 430);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 3;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // ResultRichTextBox
            // 
            this.ResultRichTextBox.Location = new System.Drawing.Point(2, 72);
            this.ResultRichTextBox.Name = "ResultRichTextBox";
            this.ResultRichTextBox.Size = new System.Drawing.Size(720, 147);
            this.ResultRichTextBox.TabIndex = 1;
            this.ResultRichTextBox.Text = "";
            // 
            // searchGroupBox
            // 
            this.searchGroupBox.Controls.Add(this.searchButton);
            this.searchGroupBox.Controls.Add(this.searchTextBox);
            this.searchGroupBox.Controls.Add(this.searchLabel);
            this.searchGroupBox.Location = new System.Drawing.Point(2, 3);
            this.searchGroupBox.Name = "searchGroupBox";
            this.searchGroupBox.Size = new System.Drawing.Size(720, 63);
            this.searchGroupBox.TabIndex = 0;
            this.searchGroupBox.TabStop = false;
            this.searchGroupBox.Text = "Search Parameters";
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(11, 30);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(85, 13);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "History contains:";
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(103, 30);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(165, 20);
            this.searchTextBox.TabIndex = 1;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(375, 30);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 2;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(266, 430);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 2;
            this.clearButton.Text = "Clear Result";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // resultdgv
            // 
            this.resultdgv.AllowUserToAddRows = false;
            this.resultdgv.AllowUserToDeleteRows = false;
            this.resultdgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultdgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ValueCol,
            this.TimestampCol});
            this.resultdgv.Location = new System.Drawing.Point(2, 239);
            this.resultdgv.Name = "resultdgv";
            this.resultdgv.ReadOnly = true;
            this.resultdgv.Size = new System.Drawing.Size(720, 175);
            this.resultdgv.TabIndex = 4;
            // 
            // ValueCol
            // 
            this.ValueCol.HeaderText = "Text";
            this.ValueCol.Name = "ValueCol";
            this.ValueCol.ReadOnly = true;
            this.ValueCol.Width = 477;
            // 
            // TimestampCol
            // 
            this.TimestampCol.HeaderText = "Timestamp";
            this.TimestampCol.Name = "TimestampCol";
            this.TimestampCol.ReadOnly = true;
            this.TimestampCol.Width = 200;
            // 
            // SearchFrm
            // 
            this.AcceptButton = this.searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 461);
            this.Controls.Add(this.resultdgv);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.ResultRichTextBox);
            this.Controls.Add(this.searchGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchFrm";
            this.ShowIcon = false;
            this.Text = "Search History";
            this.searchGroupBox.ResumeLayout(false);
            this.searchGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultdgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Button exportButton;


    }
}
