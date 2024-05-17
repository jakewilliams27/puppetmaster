namespace PuppetMasterInjector
{
    partial class PuppetMasterInjectorWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InjectBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.gameDirPath = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.gameDirChooser = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // InjectBtn
            // 
            this.InjectBtn.Enabled = false;
            this.InjectBtn.Location = new System.Drawing.Point(12, 380);
            this.InjectBtn.Name = "InjectBtn";
            this.InjectBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.InjectBtn.Size = new System.Drawing.Size(465, 58);
            this.InjectBtn.TabIndex = 0;
            this.InjectBtn.Text = "Start Towerfall";
            this.InjectBtn.UseVisualStyleBackColor = true;
            this.InjectBtn.Click += new System.EventHandler(this.InjectBtn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(74, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 45);
            this.label1.TabIndex = 1;
            this.label1.Text = "Puppet Master Injector";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 147);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(465, 216);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // gameDirPath
            // 
            this.gameDirPath.BackColor = System.Drawing.SystemColors.Window;
            this.gameDirPath.Enabled = false;
            this.gameDirPath.ForeColor = System.Drawing.SystemColors.MenuText;
            this.gameDirPath.Location = new System.Drawing.Point(14, 113);
            this.gameDirPath.Name = "gameDirPath";
            this.gameDirPath.Size = new System.Drawing.Size(339, 23);
            this.gameDirPath.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(359, 113);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Select Game Dir";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PuppetMasterInjectorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.gameDirPath);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InjectBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PuppetMasterInjectorWindow";
            this.Text = "PuppetMaster Injector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PuppetMasterInjectorWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button InjectBtn;
        private Label label1;
        private RichTextBox richTextBox1;
        private TextBox gameDirPath;
        private Button button2;
        private FolderBrowserDialog gameDirChooser;
    }
}