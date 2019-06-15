namespace MT_ListFusen2
{
	partial class FormExportAll
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
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.panelLabel3 = new System.Windows.Forms.Panel();
			this.panelBarBottom = new System.Windows.Forms.Panel();
			this.panelBarRight = new System.Windows.Forms.Panel();
			this.panelBarLeft = new System.Windows.Forms.Panel();
			this.panelBarTop = new System.Windows.Forms.Panel();
			this.buttonReference = new System.Windows.Forms.Button();
			this.textBoxExport = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("メイリオ", 9.75F);
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(42)))));
			this.label1.Location = new System.Drawing.Point(27, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "出力先のフォルダパスを入力してください";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Font = new System.Drawing.Font("メイリオ", 9.75F);
			this.btnCancel.Location = new System.Drawing.Point(319, 114);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 32);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Font = new System.Drawing.Font("メイリオ", 9.75F);
			this.btnOK.Location = new System.Drawing.Point(227, 114);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 32);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "Export";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
			// 
			// panelLabel3
			// 
			this.panelLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(92)))), ((int)(((byte)(55)))));
			this.panelLabel3.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelLabel3.Location = new System.Drawing.Point(4, 4);
			this.panelLabel3.Name = "panelLabel3";
			this.panelLabel3.Size = new System.Drawing.Size(5, 152);
			this.panelLabel3.TabIndex = 0;
			// 
			// panelBarBottom
			// 
			this.panelBarBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(22)))));
			this.panelBarBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBarBottom.Enabled = false;
			this.panelBarBottom.Location = new System.Drawing.Point(4, 156);
			this.panelBarBottom.Name = "panelBarBottom";
			this.panelBarBottom.Size = new System.Drawing.Size(412, 4);
			this.panelBarBottom.TabIndex = 0;
			// 
			// panelBarRight
			// 
			this.panelBarRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(22)))));
			this.panelBarRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelBarRight.Enabled = false;
			this.panelBarRight.Location = new System.Drawing.Point(416, 4);
			this.panelBarRight.Name = "panelBarRight";
			this.panelBarRight.Size = new System.Drawing.Size(4, 156);
			this.panelBarRight.TabIndex = 0;
			// 
			// panelBarLeft
			// 
			this.panelBarLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(22)))));
			this.panelBarLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelBarLeft.Enabled = false;
			this.panelBarLeft.Location = new System.Drawing.Point(0, 4);
			this.panelBarLeft.Name = "panelBarLeft";
			this.panelBarLeft.Size = new System.Drawing.Size(4, 156);
			this.panelBarLeft.TabIndex = 0;
			// 
			// panelBarTop
			// 
			this.panelBarTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(22)))));
			this.panelBarTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelBarTop.Enabled = false;
			this.panelBarTop.Location = new System.Drawing.Point(0, 0);
			this.panelBarTop.Name = "panelBarTop";
			this.panelBarTop.Size = new System.Drawing.Size(420, 4);
			this.panelBarTop.TabIndex = 0;
			// 
			// buttonReference
			// 
			this.buttonReference.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonReference.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonReference.Location = new System.Drawing.Point(28, 103);
			this.buttonReference.Name = "buttonReference";
			this.buttonReference.Size = new System.Drawing.Size(80, 27);
			this.buttonReference.TabIndex = 2;
			this.buttonReference.Text = "参照";
			this.buttonReference.UseVisualStyleBackColor = true;
			this.buttonReference.Click += new System.EventHandler(this.ButtonReference_Click);
			// 
			// textBoxExport
			// 
			this.textBoxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(162)))), ((int)(((byte)(164)))));
			this.textBoxExport.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBoxExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(64)))));
			this.textBoxExport.Location = new System.Drawing.Point(28, 70);
			this.textBoxExport.Name = "textBoxExport";
			this.textBoxExport.Size = new System.Drawing.Size(371, 27);
			this.textBoxExport.TabIndex = 1;
			this.textBoxExport.Text = "Folder Path";
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(64)))));
			this.label6.Dock = System.Windows.Forms.DockStyle.Top;
			this.label6.Font = new System.Drawing.Font("メイリオ", 9.75F);
			this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(194)))), ((int)(((byte)(186)))));
			this.label6.Location = new System.Drawing.Point(9, 4);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(407, 28);
			this.label6.TabIndex = 5;
			this.label6.Text = "  : : メモの一括出力 : :";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormExportAll
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(245)))), ((int)(((byte)(235)))));
			this.ClientSize = new System.Drawing.Size(420, 160);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.buttonReference);
			this.Controls.Add(this.textBoxExport);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.panelLabel3);
			this.Controls.Add(this.panelBarBottom);
			this.Controls.Add(this.panelBarRight);
			this.Controls.Add(this.panelBarLeft);
			this.Controls.Add(this.panelBarTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormExportAll";
			this.Text = "FormExportAll";
			this.Load += new System.EventHandler(this.FormExportAll_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label label1;
		public System.Windows.Forms.Button btnCancel;
		public System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel panelLabel3;
		private System.Windows.Forms.Panel panelBarBottom;
		private System.Windows.Forms.Panel panelBarRight;
		private System.Windows.Forms.Panel panelBarLeft;
		private System.Windows.Forms.Panel panelBarTop;
		private System.Windows.Forms.Button buttonReference;
		private System.Windows.Forms.TextBox textBoxExport;
		private System.Windows.Forms.Label label6;
	}
}