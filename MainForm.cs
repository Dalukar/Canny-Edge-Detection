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
			InitializeComponent();
			
		}
		
		private void SafeLog(string text) {
            Action chTxt = new Action(() => {
                LogBox.Text = text;
            });
 
            if (InvokeRequired)
                this.BeginInvoke(chTxt);
            else chTxt();
        }
		
//		int SimpleMotionDetect(IplImage img1, IplImage img2)
//		{
//			//Можно сделать раза в 3 быстрее http://tech.pro/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale
//			int diff = 0;
//			unsafe {
//    			byte* ptr1 = (byte*)img1.ImageData;
//    			byte* ptr2 = (byte*)img2.ImageData;
//    			for (int y = 0; y < img1.Height; y++) {
//        			for (int x = 0; x < img1.Width; x++) {
//            			int offset = (img1.WidthStep * y) + (x * 3);
//            			byte b1 = ptr1[offset + 0];    // B
//            			byte g1 = ptr1[offset + 1];    // G
//            			byte r1 = ptr1[offset + 2];    // R
//            			ptr1[offset + 0] = r1;
//            			ptr1[offset + 1] = g1;
//            			ptr1[offset + 2] = b1;
//            			 byte grayScale1 = 
//               				(byte)((b1 * .11) + //B
//               				(g1 * .59) +  //G
//               				(r1 * .3)); //R
//            			byte b2 = ptr2[offset + 0];    // B
//            			byte g2 = ptr2[offset + 1];    // G
//            			byte r2 = ptr2[offset + 2];    // R
//            			ptr2[offset + 0] = r2;
//            			ptr2[offset + 1] = g2;
//            			ptr2[offset + 2] = b2;
//            			 byte grayScale2 = 
//               				(byte)((b2 * .11) + //B
//               				(g2 * .59) +  //G
//               				(r2 * .3)); //R
//                         diff += Math.Abs(grayScale1 - grayScale2)/100;
//        			}
//    			}
//			}
//			return diff;
//		}
		
		void BrowseButtonClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
   			openFileDialog1.Title = "Выбрать файл";
    		if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
   			{
    			PathText.Text = openFileDialog1.FileName;
   			}
		}
	}
}
