namespace Coverter
{
	partial class Main
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
			this.btnSelectAndConvert = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// btnSelectAndConvert
			// 
			this.btnSelectAndConvert.Location = new System.Drawing.Point(352, 303);
			this.btnSelectAndConvert.Name = "btnSelectAndConvert";
			this.btnSelectAndConvert.Size = new System.Drawing.Size(75, 23);
			this.btnSelectAndConvert.TabIndex = 0;
			this.btnSelectAndConvert.Text = "Seç";
			this.btnSelectAndConvert.UseVisualStyleBackColor = true;
			this.btnSelectAndConvert.Click += new System.EventHandler(this.btnSelectAndConvert_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(12, 82);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(331, 171);
			this.panel1.TabIndex = 1;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnSelectAndConvert);
			this.Name = "Main";
			this.Text = "Pop Coverter | V1.0";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSelectAndConvert;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Panel panel1;
	}
}

