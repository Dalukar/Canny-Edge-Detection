/*
 * Created by SharpDevelop.
 * User: VKirgintcev
 * Date: 27.05.2015
 * Time: 8:18
 */
using System;
using System.Windows.Forms;


namespace CannyEdgeDetection
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
