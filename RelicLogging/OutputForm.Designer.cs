namespace RelicLogging
{
    partial class OutputForm
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
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonCopyToClipboard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOutput.Location = new System.Drawing.Point(12, 12);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(615, 268);
            this.textBoxOutput.TabIndex = 0;
            this.textBoxOutput.WordWrap = false;
            // 
            // buttonCopyToClipboard
            // 
            this.buttonCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCopyToClipboard.Location = new System.Drawing.Point(12, 286);
            this.buttonCopyToClipboard.Name = "buttonCopyToClipboard";
            this.buttonCopyToClipboard.Size = new System.Drawing.Size(109, 23);
            this.buttonCopyToClipboard.TabIndex = 1;
            this.buttonCopyToClipboard.Text = "Copy To Clipboard";
            this.buttonCopyToClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyToClipboard.Click += new System.EventHandler(this.buttonCopyToClipboard_Click);
            // 
            // OutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 321);
            this.Controls.Add(this.buttonCopyToClipboard);
            this.Controls.Add(this.textBoxOutput);
            this.Name = "OutputForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Output";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.OutputForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OutputForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonCopyToClipboard;


    }
}