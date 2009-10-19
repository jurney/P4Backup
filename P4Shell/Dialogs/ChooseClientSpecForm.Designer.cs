namespace Perforce
{
	partial class ChooseClientSpecForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.listBoxClientSpecs = new System.Windows.Forms.ListBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.clientSpecUserControl = new Perforce.ClientSpecUserControl();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point( 0, 0 );
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add( this.progressBar );
			this.splitContainer1.Panel1.Controls.Add( this.listBoxClientSpecs );
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add( this.panel1 );
			this.splitContainer1.Size = new System.Drawing.Size( 864, 448 );
			this.splitContainer1.SplitterDistance = 201;
			this.splitContainer1.TabIndex = 0;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point( 12, 415 );
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size( 161, 23 );
			this.progressBar.TabIndex = 3;
			// 
			// listBoxClientSpecs
			// 
			this.listBoxClientSpecs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxClientSpecs.FormattingEnabled = true;
			this.listBoxClientSpecs.IntegralHeight = false;
			this.listBoxClientSpecs.Location = new System.Drawing.Point( 0, 0 );
			this.listBoxClientSpecs.Name = "listBoxClientSpecs";
			this.listBoxClientSpecs.ScrollAlwaysVisible = true;
			this.listBoxClientSpecs.Size = new System.Drawing.Size( 201, 448 );
			this.listBoxClientSpecs.TabIndex = 2;
			this.listBoxClientSpecs.UseTabStops = false;
			this.listBoxClientSpecs.SelectedIndexChanged += new System.EventHandler( this.listBoxClientSpecs_SelectedIndexChanged );
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point( 486, 411 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler( this.buttonOK_Click );
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point( 567, 411 );
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size( 75, 23 );
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler( this.buttonCancel_Click );
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add( this.buttonOK );
			this.panel1.Controls.Add( this.buttonCancel );
			this.panel1.Controls.Add( this.clientSpecUserControl );
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point( 0, 0 );
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size( 659, 448 );
			this.panel1.TabIndex = 66;
			// 
			// clientSpecUserControl
			// 
			this.clientSpecUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientSpecUserControl.Client = null;
			this.clientSpecUserControl.Location = new System.Drawing.Point( -6, -3 );
			this.clientSpecUserControl.Name = "clientSpecUserControl";
			this.clientSpecUserControl.Size = new System.Drawing.Size( 663, 417 );
			this.clientSpecUserControl.TabIndex = 0;
			this.clientSpecUserControl.TabStop = false;
			// 
			// ChooseClientSpecForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 864, 448 );
			this.Controls.Add( this.splitContainer1 );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseClientSpecForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose Your Perforce ClientSpec";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.ChooseClientSpecForm_FormClosing );
			this.Load += new System.EventHandler( this.ChooseClientSpecForm_Load );
			this.splitContainer1.Panel1.ResumeLayout( false );
			this.splitContainer1.Panel2.ResumeLayout( false );
			this.splitContainer1.ResumeLayout( false );
			this.panel1.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox listBoxClientSpecs;
		private ClientSpecUserControl clientSpecUserControl;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panel1;

	}
}