namespace MidiControl {
	partial class UpdateCheckerGUI {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.btnDownload = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpBox = new System.Windows.Forms.GroupBox();
			this.txtReleaseNotes = new System.Windows.Forms.TextBox();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.grpBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnDownload
			// 
			this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDownload.Location = new System.Drawing.Point(226, 35);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(75, 23);
			this.btnDownload.TabIndex = 0;
			this.btnDownload.Text = "Download";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(307, 35);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// grpBox
			// 
			this.grpBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpBox.Controls.Add(this.txtReleaseNotes);
			this.grpBox.Location = new System.Drawing.Point(12, 12);
			this.grpBox.Name = "grpBox";
			this.grpBox.Size = new System.Drawing.Size(370, 17);
			this.grpBox.TabIndex = 2;
			this.grpBox.TabStop = false;
			this.grpBox.Text = "MIDIControl v2.x.y.x is available!";
			// 
			// txtReleaseNotes
			// 
			this.txtReleaseNotes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtReleaseNotes.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtReleaseNotes.Location = new System.Drawing.Point(3, 16);
			this.txtReleaseNotes.Multiline = true;
			this.txtReleaseNotes.Name = "txtReleaseNotes";
			this.txtReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtReleaseNotes.Size = new System.Drawing.Size(364, 0);
			this.txtReleaseNotes.TabIndex = 3;
			this.txtReleaseNotes.Text = "Release notes pulled from GitHub:\r\n- Lorem\r\n- Ipsum\r\n- Something\r\n- Something els" +
    "e\r\n- idk";
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(15, 35);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(205, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar1.TabIndex = 3;
			this.progressBar1.Value = 50;
			// 
			// UpdateCheckerGUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(390, 66);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.grpBox);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnDownload);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateCheckerGUI";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Updates";
			this.Load += new System.EventHandler(this.UpdateCheckerGUI_Load);
			this.grpBox.ResumeLayout(false);
			this.grpBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox grpBox;
		private System.Windows.Forms.TextBox txtReleaseNotes;
		private System.Windows.Forms.ProgressBar progressBar1;
	}
}