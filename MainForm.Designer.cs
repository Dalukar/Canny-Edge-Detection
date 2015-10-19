/*
 * Created by SharpDevelop.
 * User: VKirgintcev
 * Date: 27.05.2015
 * Time: 8:18
 */
namespace CannyEdgeDetection
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.RichTextBox LogBox;
		private System.Windows.Forms.TextBox PathText;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnCalculate;
		private System.Windows.Forms.PictureBox pctWindow1;
		private System.Windows.Forms.PictureBox pctWindow2;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCalculate = new System.Windows.Forms.Button();
			this.LogBox = new System.Windows.Forms.RichTextBox();
			this.PathText = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.pctWindow1 = new System.Windows.Forms.PictureBox();
			this.pctWindow2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pctWindow1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pctWindow2)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCalculate
			// 
			this.btnCalculate.Location = new System.Drawing.Point(373, 41);
			this.btnCalculate.Name = "btnCalculate";
			this.btnCalculate.Size = new System.Drawing.Size(70, 23);
			this.btnCalculate.TabIndex = 0;
			this.btnCalculate.Text = "Calculate";
			this.btnCalculate.UseVisualStyleBackColor = true;
			// 
			// LogBox
			// 
			this.LogBox.BackColor = System.Drawing.SystemColors.Info;
			this.LogBox.Location = new System.Drawing.Point(12, 70);
			this.LogBox.Name = "LogBox";
			this.LogBox.ReadOnly = true;
			this.LogBox.Size = new System.Drawing.Size(431, 468);
			this.LogBox.TabIndex = 1;
			this.LogBox.Text = "";
			// 
			// PathText
			// 
			this.PathText.Location = new System.Drawing.Point(12, 12);
			this.PathText.Name = "PathText";
			this.PathText.Size = new System.Drawing.Size(355, 20);
			this.PathText.TabIndex = 2;
			this.PathText.Text = "C:\\yk42b\\Hi-Res DX Logo.jpg";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(373, 12);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(70, 23);
			this.btnBrowse.TabIndex = 3;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.BrowseButtonClick);
			// 
			// pctWindow1
			// 
			this.pctWindow1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pctWindow1.Location = new System.Drawing.Point(449, 12);
			this.pctWindow1.Name = "pctWindow1";
			this.pctWindow1.Size = new System.Drawing.Size(488, 260);
			this.pctWindow1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pctWindow1.TabIndex = 4;
			this.pctWindow1.TabStop = false;
			// 
			// pctWindow2
			// 
			this.pctWindow2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pctWindow2.Location = new System.Drawing.Point(449, 278);
			this.pctWindow2.Name = "pctWindow2";
			this.pctWindow2.Size = new System.Drawing.Size(488, 260);
			this.pctWindow2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pctWindow2.TabIndex = 5;
			this.pctWindow2.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(950, 550);
			this.Controls.Add(this.pctWindow2);
			this.Controls.Add(this.pctWindow1);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.PathText);
			this.Controls.Add(this.LogBox);
			this.Controls.Add(this.btnCalculate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Canny Edge Detection";
			((System.ComponentModel.ISupportInitialize)(this.pctWindow1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pctWindow2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		}
		}