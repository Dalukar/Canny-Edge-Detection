/*
 * Created by SharpDevelop.
 * User: VKirgintcev
 * Date: 19.10.2015
 * Time: 11:11
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CannyEdgeDetection
{
	/// <summary>
	/// Класс для обнаружения границ.
	/// </summary>
	public static class CannyEdgeDetector
	{   
		static public double [,] Kernel;
		
		public static void CalculateKernel(int length, double weight) 
		{
		    Kernel = new double [length, length]; 
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
		            Kernel[y, x] = Kernel[y, x] *  (1.0 / sumTotal); 
		        } 
		    } 
		}
		
		public static Bitmap ToGrayscale(Bitmap image)
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
		
		public static string KernelToString()
		{
			string str = "";
			if(Kernel == null)
			{
				return "no kernel";
			}
			int length = Kernel.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				str += " | ";
				for (int j = 0; j < length; j++)
				{
					str += Kernel[i,j] + " | ";
				}
				str += "\n";
			}
			return str;
		}
		
		public static Bitmap ConvolutionFilter(this Bitmap sourceBitmap,   
                                              double factor = 1,  
                                                   int bias = 0)  
{ 
    BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, 
                            sourceBitmap.Width, sourceBitmap.Height), 
                                              ImageLockMode.ReadOnly,  
                                        PixelFormat.Format32bppArgb); 

   
    byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height]; 

   
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length); 
    sourceBitmap.UnlockBits(sourceData); 

   
    double blue = 0.0; 
    double green = 0.0; 
    double red = 0.0; 

   
    int filterWidth = Kernel.GetLength(1); 
    int filterHeight = Kernel.GetLength(0); 

   
    int filterOffset = (filterWidth-1) / 2; 
    int calcOffset = 0; 

   
    int byteOffset = 0; 

   
    for (int offsetY = filterOffset; offsetY <  
        sourceBitmap.Height - filterOffset; offsetY++) 
    {
        for (int offsetX = filterOffset; offsetX <  
            sourceBitmap.Width - filterOffset; offsetX++) 
        {
            blue = 0; 
            green = 0; 
            red = 0; 

   
            byteOffset = offsetY *  
                         sourceData.Stride +  
                         offsetX * 4; 

   
            for (int filterY = -filterOffset;  
                filterY <= filterOffset; filterY++) 
            { 
                for (int filterX = -filterOffset; 
                    filterX <= filterOffset; filterX++) 
                { 

   
                    calcOffset = byteOffset +  
                                 (filterX * 4) +  
                                 (filterY * sourceData.Stride); 

   
                    blue += (double  )(pixelBuffer[calcOffset]) * 
                            Kernel[filterY + filterOffset,  
                                                filterX + filterOffset]; 

   
                    green += (double  )(pixelBuffer[calcOffset + 1]) * 
                             Kernel[filterY + filterOffset,  
                                                filterX + filterOffset]; 

   
                    red += (double  )(pixelBuffer[calcOffset + 2]) * 
                           Kernel[filterY + filterOffset,  
                                              filterX + filterOffset]; 
                } 
            } 

   
            blue = factor * blue + bias; 
            green = factor * green + bias; 
            red = factor * red + bias; 

   
            blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue)); 
            green = (green > 255 ? 255 : (green < 0 ? 0 : green)); 
            red = (red > 255 ? 255 : (red < 0 ? 0 : blue)); 

   
            resultBuffer[byteOffset] = (byte)(blue); 
            resultBuffer[byteOffset + 1] = (byte)(green); 
            resultBuffer[byteOffset + 2] = (byte)(red); 
            resultBuffer[byteOffset + 3] = 255; 
        }
    }

   
    Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height); 

   
    BitmapData resultData = resultBitmap.LockBits(new Rectangle (0, 0, 
                             resultBitmap.Width, resultBitmap.Height), 
                                              ImageLockMode.WriteOnly, 
                                         PixelFormat.Format32bppArgb); 

   
    Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length); 
    resultBitmap.UnlockBits(resultData); 

   
    return resultBitmap; 
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
