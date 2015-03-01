namespace WebCrawler
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.web = new System.Windows.Forms.WebBrowser();
            this.bNextPageItem = new System.Windows.Forms.Button();
            this.bParsePageButton = new System.Windows.Forms.Button();
            this.pFetchBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.totalRemainBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // web
            // 
            this.web.Dock = System.Windows.Forms.DockStyle.Fill;
            this.web.Location = new System.Drawing.Point(0, 0);
            this.web.MinimumSize = new System.Drawing.Size(20, 20);
            this.web.Name = "web";
            this.web.Size = new System.Drawing.Size(1354, 596);
            this.web.TabIndex = 0;
            this.web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.web_DocumentCompleted);
            // 
            // bNextPageItem
            // 
            this.bNextPageItem.Location = new System.Drawing.Point(1144, 6);
            this.bNextPageItem.Name = "bNextPageItem";
            this.bNextPageItem.Size = new System.Drawing.Size(132, 33);
            this.bNextPageItem.TabIndex = 1;
            this.bNextPageItem.Text = "下一个ID";
            this.bNextPageItem.UseVisualStyleBackColor = true;
            this.bNextPageItem.Click += new System.EventHandler(this.bNextPageItem_Click);
            // 
            // bParsePageButton
            // 
            this.bParsePageButton.Location = new System.Drawing.Point(995, 6);
            this.bParsePageButton.Name = "bParsePageButton";
            this.bParsePageButton.Size = new System.Drawing.Size(115, 32);
            this.bParsePageButton.TabIndex = 2;
            this.bParsePageButton.Text = "解析页面";
            this.bParsePageButton.UseVisualStyleBackColor = true;
            this.bParsePageButton.Click += new System.EventHandler(this.bParsePageButton_Click);
            // 
            // pFetchBar
            // 
            this.pFetchBar.Location = new System.Drawing.Point(323, 13);
            this.pFetchBar.Name = "pFetchBar";
            this.pFetchBar.Size = new System.Drawing.Size(646, 15);
            this.pFetchBar.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-2, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "剩余ID总数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "获取进度";
            // 
            // totalRemainBox
            // 
            this.totalRemainBox.Location = new System.Drawing.Point(111, 9);
            this.totalRemainBox.Name = "totalRemainBox";
            this.totalRemainBox.Size = new System.Drawing.Size(100, 21);
            this.totalRemainBox.TabIndex = 7;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 596);
            this.Controls.Add(this.totalRemainBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pFetchBar);
            this.Controls.Add(this.bParsePageButton);
            this.Controls.Add(this.bNextPageItem);
            this.Controls.Add(this.web);
            this.Name = "frmMain";
            this.Text = "爬取用户历史记录App";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser web;
        private System.Windows.Forms.Button bNextPageItem;
        private System.Windows.Forms.Button bParsePageButton;
        private System.Windows.Forms.ProgressBar pFetchBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox totalRemainBox;

    }
}

