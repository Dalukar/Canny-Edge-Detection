/*
 * Created by SharpDevelop.
 * User: VKirgintcev
 * Date: 27.05.2015
 * Time: 8:18
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace CannyEdgeDetection
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			CannyEdgeDetector.CalculateKernel(3, 5.5);
// удалить потом
//			CannyEdgeDetector.Kernel = new double [3, 3]; 
//			CannyEdgeDetector.Kernel[0,0] = 0;
//			CannyEdgeDetector.Kernel[1,0] = 1;
//			CannyEdgeDetector.Kernel[2,0] = 0;
//			CannyEdgeDetector.Kernel[0,1] = 1;
//			CannyEdgeDetector.Kernel[1,1] = -4;
//			CannyEdgeDetector.Kernel[2,1] = 1;
//			CannyEdgeDetector.Kernel[0,2] = 0;
//			CannyEdgeDetector.Kernel[1,2] = 1;
//			CannyEdgeDetector.Kernel[2,2] = 0;
			InitializeComponent();
			
		}
		
		private void SafeLog(string text) {
            Action chTxt = new Action(() => {
                LogBox.Text += text;
            });
 
            if (InvokeRequired)
                this.BeginInvoke(chTxt);
            else chTxt();
        }

		void BrowseButtonClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
   			openFileDialog1.Title = "Выбрать файл";
    		if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
   			{
    			PathText.Text = openFileDialog1.FileName;
   			}
    		try{    		
    			pctWindow1.Image = new Bitmap(PathText.Text);
    		}
    		catch(Exception ex){
    			SafeLog("Ошибка: " + ex.Message);
    		}

		}
		void BtnCalculateClick(object sender, EventArgs e)
		{
			SafeLog("Gaussian Kernel: \n");
			SafeLog(CannyEdgeDetector.KernelToString());
			if(pctWindow1.Image != null){
				pctWindow2.Image = CannyEdgeDetector.ToGrayscale(CannyEdgeDetector.ConvolutionFilter((Bitmap) pctWindow1.Image));
			}
		}
		
	}
}
