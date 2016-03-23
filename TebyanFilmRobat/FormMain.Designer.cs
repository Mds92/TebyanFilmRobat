namespace TebyanFilmRobat
{
	partial class FormMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.label1 = new System.Windows.Forms.Label();
			this.maskedTextBoxCategoryId = new System.Windows.Forms.MaskedTextBox();
			this.textBoxLogs = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.maskedTextBoxFromPage = new System.Windows.Forms.MaskedTextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.maskedTextBoxToPage = new System.Windows.Forms.MaskedTextBox();
			this.buttonStop = new System.Windows.Forms.Button();
			this.buttonStart = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(515, 9);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "کد گروه:";
			// 
			// maskedTextBoxCategoryId
			// 
			this.maskedTextBoxCategoryId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.maskedTextBoxCategoryId.Location = new System.Drawing.Point(424, 6);
			this.maskedTextBoxCategoryId.Mask = "00000000";
			this.maskedTextBoxCategoryId.Name = "maskedTextBoxCategoryId";
			this.maskedTextBoxCategoryId.Size = new System.Drawing.Size(85, 20);
			this.maskedTextBoxCategoryId.TabIndex = 1;
			this.maskedTextBoxCategoryId.Text = "2935";
			// 
			// textBoxLogs
			// 
			this.textBoxLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLogs.BackColor = System.Drawing.SystemColors.ControlLight;
			this.textBoxLogs.Location = new System.Drawing.Point(0, 33);
			this.textBoxLogs.Multiline = true;
			this.textBoxLogs.Name = "textBoxLogs";
			this.textBoxLogs.ReadOnly = true;
			this.textBoxLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLogs.Size = new System.Drawing.Size(576, 328);
			this.textBoxLogs.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(371, 9);
			this.label2.Name = "label2";
			this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label2.Size = new System.Drawing.Size(47, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "از صفحه:";
			// 
			// maskedTextBoxFromPage
			// 
			this.maskedTextBoxFromPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.maskedTextBoxFromPage.Location = new System.Drawing.Point(333, 6);
			this.maskedTextBoxFromPage.Mask = "000";
			this.maskedTextBoxFromPage.Name = "maskedTextBoxFromPage";
			this.maskedTextBoxFromPage.Size = new System.Drawing.Size(32, 20);
			this.maskedTextBoxFromPage.TabIndex = 2;
			this.maskedTextBoxFromPage.Text = "1";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(280, 9);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label3.Size = new System.Drawing.Size(47, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "تا صفحه:";
			// 
			// maskedTextBoxToPage
			// 
			this.maskedTextBoxToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.maskedTextBoxToPage.Location = new System.Drawing.Point(242, 6);
			this.maskedTextBoxToPage.Mask = "000";
			this.maskedTextBoxToPage.Name = "maskedTextBoxToPage";
			this.maskedTextBoxToPage.Size = new System.Drawing.Size(32, 20);
			this.maskedTextBoxToPage.TabIndex = 3;
			this.maskedTextBoxToPage.Text = "1";
			// 
			// buttonStop
			// 
			this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStop.BackColor = System.Drawing.SystemColors.ControlLight;
			this.buttonStop.Enabled = false;
			this.buttonStop.Location = new System.Drawing.Point(127, 4);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.buttonStop.Size = new System.Drawing.Size(43, 23);
			this.buttonStop.TabIndex = 5;
			this.buttonStop.Text = "توقف";
			this.buttonStop.UseVisualStyleBackColor = false;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// buttonStart
			// 
			this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStart.BackColor = System.Drawing.SystemColors.ControlLight;
			this.buttonStart.Location = new System.Drawing.Point(176, 4);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.buttonStart.Size = new System.Drawing.Size(43, 23);
			this.buttonStart.TabIndex = 4;
			this.buttonStart.Text = "شروع";
			this.buttonStart.UseVisualStyleBackColor = false;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(576, 361);
			this.Controls.Add(this.buttonStop);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.maskedTextBoxToPage);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.maskedTextBoxFromPage);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxLogs);
			this.Controls.Add(this.maskedTextBoxCategoryId);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.Text = "Tebyan Film Link Robot";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MaskedTextBox maskedTextBoxCategoryId;
		private System.Windows.Forms.TextBox textBoxLogs;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.MaskedTextBox maskedTextBoxFromPage;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.MaskedTextBox maskedTextBoxToPage;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.Button buttonStart;
	}
}

