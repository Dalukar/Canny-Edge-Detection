﻿namespace CannyEdgeDetection
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
		private System.Windows.Forms.PictureBox pctWindow3;
		
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
            this.pctWindow3 = new System.Windows.Forms.PictureBox();
            this.pctWindow2 = new System.Windows.Forms.PictureBox();
            this.pctWindow4 = new System.Windows.Forms.PictureBox();
            this.pctWindow5 = new System.Windows.Forms.PictureBox();
            this.pctWindow6 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textGaussKernelSize = new System.Windows.Forms.TextBox();
            this.textWeakThreshold = new System.Windows.Forms.TextBox();
            this.textStrongThreshold = new System.Windows.Forms.TextBox();
            this.textGaussKernelDeviation = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow6)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(342, 40);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(70, 23);
            this.btnCalculate.TabIndex = 0;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.BtnCalculateClick);
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.SystemColors.Info;
            this.LogBox.Location = new System.Drawing.Point(13, 627);
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.Size = new System.Drawing.Size(400, 176);
            this.LogBox.TabIndex = 1;
            this.LogBox.Text = "";
            // 
            // PathText
            // 
            this.PathText.Location = new System.Drawing.Point(12, 12);
            this.PathText.Name = "PathText";
            this.PathText.Size = new System.Drawing.Size(324, 20);
            this.PathText.TabIndex = 2;
            this.PathText.Text = "C:\\yk42b\\Hi-Res DX Logo.jpg";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(342, 12);
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
            this.pctWindow1.Location = new System.Drawing.Point(418, 12);
            this.pctWindow1.Name = "pctWindow1";
            this.pctWindow1.Size = new System.Drawing.Size(400, 260);
            this.pctWindow1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctWindow1.TabIndex = 4;
            this.pctWindow1.TabStop = false;
            // 
            // pctWindow3
            // 
            this.pctWindow3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pctWindow3.Location = new System.Drawing.Point(419, 278);
            this.pctWindow3.Name = "pctWindow3";
            this.pctWindow3.Size = new System.Drawing.Size(400, 260);
            this.pctWindow3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctWindow3.TabIndex = 5;
            this.pctWindow3.TabStop = false;
            // 
            // pctWindow2
            // 
            this.pctWindow2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pctWindow2.Location = new System.Drawing.Point(825, 12);
            this.pctWindow2.Name = "pctWindow2";
            this.pctWindow2.Size = new System.Drawing.Size(400, 260);
            this.pctWindow2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctWindow2.TabIndex = 6;
            this.pctWindow2.TabStop = false;
            // 
            // pctWindow4
            // 
            this.pctWindow4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pctWindow4.Location = new System.Drawing.Point(825, 278);
            this.pctWindow4.Name = "pctWindow4";
            this.pctWindow4.Size = new System.Drawing.Size(400, 260);
            this.pctWindow4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctWindow4.TabIndex = 7;
            this.pctWindow4.TabStop = false;
            // 
            // pctWindow5
            // 
            this.pctWindow5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pctWindow5.Location = new System.Drawing.Point(419, 544);
            this.pctWindow5.Name = "pctWindow5";
            this.pctWindow5.Size = new System.Drawing.Size(400, 260);
            this.pctWindow5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctWindow5.TabIndex = 8;
            this.pctWindow5.TabStop = false;
            // 
            // pctWindow6
            // 
            this.pctWindow6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pctWindow6.Location = new System.Drawing.Point(825, 544);
            this.pctWindow6.Name = "pctWindow6";
            this.pctWindow6.Size = new System.Drawing.Size(400, 260);
            this.pctWindow6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctWindow6.TabIndex = 9;
            this.pctWindow6.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Размер ядра гаусса:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Отклонение гауссианы:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Яркость сильных граней:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Яркость слабых граней:";
            // 
            // textGaussKernelSize
            // 
            this.textGaussKernelSize.Location = new System.Drawing.Point(158, 77);
            this.textGaussKernelSize.Name = "textGaussKernelSize";
            this.textGaussKernelSize.Size = new System.Drawing.Size(100, 20);
            this.textGaussKernelSize.TabIndex = 14;
            this.textGaussKernelSize.Text = "5";
            // 
            // textWeakThreshold
            // 
            this.textWeakThreshold.Location = new System.Drawing.Point(158, 155);
            this.textWeakThreshold.Name = "textWeakThreshold";
            this.textWeakThreshold.Size = new System.Drawing.Size(100, 20);
            this.textWeakThreshold.TabIndex = 15;
            this.textWeakThreshold.Text = "5";
            // 
            // textStrongThreshold
            // 
            this.textStrongThreshold.Location = new System.Drawing.Point(158, 129);
            this.textStrongThreshold.Name = "textStrongThreshold";
            this.textStrongThreshold.Size = new System.Drawing.Size(100, 20);
            this.textStrongThreshold.TabIndex = 16;
            this.textStrongThreshold.Text = "40";
            // 
            // textGaussKernelDeviation
            // 
            this.textGaussKernelDeviation.Location = new System.Drawing.Point(158, 103);
            this.textGaussKernelDeviation.Name = "textGaussKernelDeviation";
            this.textGaussKernelDeviation.Size = new System.Drawing.Size(100, 20);
            this.textGaussKernelDeviation.TabIndex = 17;
            this.textGaussKernelDeviation.Text = "1.4";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1237, 815);
            this.Controls.Add(this.textGaussKernelDeviation);
            this.Controls.Add(this.textStrongThreshold);
            this.Controls.Add(this.textWeakThreshold);
            this.Controls.Add(this.textGaussKernelSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pctWindow6);
            this.Controls.Add(this.pctWindow5);
            this.Controls.Add(this.pctWindow4);
            this.Controls.Add(this.pctWindow2);
            this.Controls.Add(this.pctWindow3);
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
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWindow6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private System.Windows.Forms.PictureBox pctWindow2;
        private System.Windows.Forms.PictureBox pctWindow4;
        private System.Windows.Forms.PictureBox pctWindow5;
        private System.Windows.Forms.PictureBox pctWindow6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textGaussKernelSize;
        private System.Windows.Forms.TextBox textWeakThreshold;
        private System.Windows.Forms.TextBox textStrongThreshold;
        private System.Windows.Forms.TextBox textGaussKernelDeviation;
		}
		}