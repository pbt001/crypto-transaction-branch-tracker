namespace CryptoBranchTracker.WFA.Forms
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.flpContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNewBranch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flpContainer
            // 
            this.flpContainer.Location = new System.Drawing.Point(12, 42);
            this.flpContainer.Name = "flpContainer";
            this.flpContainer.Size = new System.Drawing.Size(776, 396);
            this.flpContainer.TabIndex = 0;
            // 
            // btnNewBranch
            // 
            this.btnNewBranch.Location = new System.Drawing.Point(13, 13);
            this.btnNewBranch.Name = "btnNewBranch";
            this.btnNewBranch.Size = new System.Drawing.Size(75, 23);
            this.btnNewBranch.TabIndex = 1;
            this.btnNewBranch.Text = "New Branch";
            this.btnNewBranch.UseVisualStyleBackColor = true;
            this.btnNewBranch.Click += new System.EventHandler(this.btnNewBranch_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnNewBranch);
            this.Controls.Add(this.flpContainer);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crypto Transaction Branch Tracker";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpContainer;
        private System.Windows.Forms.Button btnNewBranch;
    }
}