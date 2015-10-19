/*
 * Created by SharpDevelop.
 * User: VKirgintcev
 * Date: 19.10.2015
 * Time: 11:11
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CannyEdgeDetection
{
	/// <summary>
	/// Класс для обнаружения границ.
	/// </summary>
	public class CannyEdgeDetector
	{   
		public double [,] GaussianKernel;
		public CannyEdgeDetector()
		{
			GaussianKernel = CalculateKernel(3, 5.5);
		}
		
		public static double[,] CalculateKernel(int length, double weight) 
		{
		    double[,] Kernel = new double [length, length]; 
		    double sumTotal = 0; 
		
		    int kernelRadius = length / 2; 
		    double distance = 0; 
		
		    double calculatedEuler = 1.0 /  
		    (2.0 * Math.PI * Math.Pow(weight, 2)); 
		
		    for (int filterY = -kernelRadius; 
		         filterY <= kernelRadius; filterY++) 
		    {
		        for (int filterX = -kernelRadius; 
		            filterX <= kernelRadius; filterX++) 
		        {
		            distance = ((filterX * filterX) +  
		                       (filterY * filterY)) /  
		                       (2 * (weight * weight)); 
		
		  
		            Kernel[filterY + kernelRadius,  
		                   filterX + kernelRadius] =  
		                   calculatedEuler * Math.Exp(-distance); 
		
		  
		            sumTotal += Kernel[filterY + kernelRadius,  
		                               filterX + kernelRadius]; 
		        } 
		    } 
		    for (int y = 0; y < length; y++) 
		    { 
		        for (int x = 0; x < length; x++) 
		        { 
		            Kernel[y, x] = Kernel[y, x] *  
		                           (1.0 / sumTotal); 
		        } 
		    } 
		    return Kernel; 
		}
		
		public Bitmap ToGrayscale(Bitmap image)
		{
    		Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
    	                                            	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
   	 		int a = 0;
    		unsafe {
        		byte* imagePointer1 = (byte*)bitmapData1.Scan0;
        		byte* imagePointer2 = (byte*)bitmapData2.Scan0;
        		for(int i = 0; i < bitmapData1.Height; i++) {
            		for(int j = 0; j < bitmapData1.Width; j++) {
                	// write the logic implementation here
                		a = (imagePointer1[0] + imagePointer1[1] + imagePointer1[2])/3;
                		imagePointer2[0] = (byte)a;
                		imagePointer2[1] = (byte)a;
                		imagePointer2[2] = (byte)a;
                		imagePointer2[3] = imagePointer1[3];
                		//4 bytes per pixel
                		imagePointer1 += 4;
                		imagePointer2 += 4;
            		}//end for j
            		//4 bytes per pixel
            		imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4);
            		imagePointer2 += bitmapData1.Stride - (bitmapData1.Width * 4);
        		}//end for i
    		}//end unsafe
    		returnMap.UnlockBits(bitmapData2);
    		image.UnlockBits(bitmapData1);
    		return returnMap;
		}
		
		public string KernelToString()
		{
			string str = "";
			int length = GaussianKernel.GetLength(0);
			if(length == 0)
			{
				return "no kernel";
			}
			for (int i = 0; i < length; i++)
			{
				str += " | ";
				for (int j = 0; j < length; j++)
				{
					str += GaussianKernel[i,j] + " | ";
				}
				str += "\n";
			}
			return str;
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

		
	}
}
