using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace CannyEdgeDetection
{
	/// <summary>
	/// Основная форма.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			
		}
        /// <summary>
        /// Запись сообщений в лог.
        /// </summary>
		private void SafeLog(string text) {
            Action chTxt = new Action(() => {
                LogBox.Text += text;
            });
            if (InvokeRequired)
                this.BeginInvoke(chTxt);
            else chTxt();
        }

        /// <summary>
        /// Выбор изображения.
        /// </summary>
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

        /// <summary>
        /// Вызов метода поиска граней и отображение промежуточных результатов.
        /// </summary>
		void BtnCalculateClick(object sender, EventArgs e)
		{
			if(pctWindow1.Image != null){

                // Применение настроек детектора.
                CannyEdgeDetector.gaussKernelSize = Convert.ToInt32(textGaussKernelSize.Text);
                CannyEdgeDetector.gaussKernelDeviation = Convert.ToDouble(textGaussKernelDeviation.Text);
                CannyEdgeDetector.strongThreshold = Convert.ToByte(textStrongThreshold.Text);
                CannyEdgeDetector.weakThreshold = Convert.ToByte(textWeakThreshold.Text);

                // Вызов функции поиска граней.
                CannyEdgeDetector.CalculateEdges((Bitmap)pctWindow1.Image);

                // Отображение результатов.
                pctWindow1.Image = CannyEdgeDetector.originalimg;
                pctWindow2.Image = CannyEdgeDetector.afterGaussImg;
                pctWindow3.Image = CannyEdgeDetector.afterCannyImg;
                pctWindow4.Image = CannyEdgeDetector.afterSupressionImg;
                pctWindow5.Image = CannyEdgeDetector.afterThresholdImg;
                pctWindow6.Image = CannyEdgeDetector.afterBLOBsDetectImg;
			}
		}
	}
}
