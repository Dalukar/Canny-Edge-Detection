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
        static public double[,] gaussKernel;
        static double [,] gradientDirections;
        static public Bitmap originalimg;
        static public Bitmap afterGaussImg;
        static public Bitmap afterCannyImg;
        static public Bitmap afterSupressionImg;
        //static Bitmap GradientX;
        //static Bitmap GradientY;
        static double[,] CannyKernelX = new double[3, 3] {{-0.25,0,0.25},
                                                          {-0.5,0,0.5}, 
                                                          {-0.25,0,0.25} };
        static double[,] CannyKernelY = new double[3, 3] {{ 0.25, 0.5, 0.25},
                                                          { 0, 0, 0}, 
                                                          {-0.25,-0.5,-0.25} };
		
        static CannyEdgeDetector()
        {
            CalculateGaussKernel(5, 1.4);
        }
		public static void CalculateGaussKernel(int length, double weight) 
		{
		    gaussKernel = new double [length, length]; 
		    double sumTotal = 0; 
		
		    int kernelRadius = length / 2; 
		    double distance = 0; 
		
		    double calculatedEuler = 1.0 /  
		    (2.0 * Math.PI * Math.Pow(weight, 2)); 
		
		    for (int filterY = -kernelRadius; filterY <= kernelRadius; filterY++) 
		    {
		        for (int filterX = -kernelRadius; filterX <= kernelRadius; filterX++) 
		        {
		            distance = ((filterX * filterX) + (filterY * filterY)) / (2 * (weight * weight));
                    gaussKernel[filterY + kernelRadius, filterX + kernelRadius] = calculatedEuler * Math.Exp(-distance); 
		            sumTotal += gaussKernel[filterY + kernelRadius, filterX + kernelRadius]; 
		        } 
		    } 
		    for (int y = 0; y < length; y++) 
		    { 
		        for (int x = 0; x < length; x++) 
		        { 
		            gaussKernel[y, x] = gaussKernel[y, x] *  (1.0 / sumTotal); 
		        } 
		    } 
		}
		public static Bitmap CalculateEdges(Bitmap sourceBitmap)
        {
			originalimg = ToGrayscale(sourceBitmap);
			afterGaussImg = ApplyFilter(originalimg, gaussKernel);
            Bitmap ResultBitmapX = ApplyFilter(afterGaussImg, CannyKernelX);
            Bitmap ResultBitmapY = ApplyFilter(afterGaussImg, CannyKernelY);
            afterCannyImg = BlendBitmapsAndGenerateGrdient(ResultBitmapX, ResultBitmapY);
            afterSupressionImg = NonMaximumSuppression(afterCannyImg);
            return afterCannyImg;
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
                		a = (imagePointer1[0] + imagePointer1[1] + imagePointer1[2])/3;  
                		imagePointer2[0] = (byte)a;
                        imagePointer2[1] = (byte)a;
                        imagePointer2[2] = (byte)a;
                        imagePointer2[3] = imagePointer1[3];
                		//4 bytes per pixel
                		imagePointer1 += 4;
                		imagePointer2 += 4;
            		}
        		}
    		}
    		returnMap.UnlockBits(bitmapData2);
    		image.UnlockBits(bitmapData1);
    		return returnMap;
		}

        public static Bitmap NonMaximumSuppression (Bitmap image)
        {
            Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0 + bitmapData1.Stride + 4;
                byte* imagePointer2 = (byte*)bitmapData2.Scan0 + bitmapData2.Stride + 4;
                for (int i = 1; i < bitmapData1.Height-1; i++)
                {
                    for (int j = 1; j < bitmapData1.Width-1; j++)
                    {
                        imagePointer2[0] = imagePointer1[0];
                        imagePointer2[1] = imagePointer1[1];
                        imagePointer2[2] = imagePointer1[2];
                        double grad = gradientDirections[i, j];
                        if ((grad >= Math.PI * 3/8 && (imagePointer1[-bitmapData1.Stride] > imagePointer1[0] || imagePointer1[bitmapData1.Stride] > imagePointer1[0])) ||
                            (grad > Math.PI * 1/8 && gradientDirections[i, j] < Math.PI * 3/8 && (imagePointer1[-bitmapData1.Stride - 4] > imagePointer1[0] || imagePointer1[bitmapData1.Stride + 4] > imagePointer1[0])) ||
                            (grad <= Math.PI * 1/8 && (imagePointer1[-4] > imagePointer1[0] || imagePointer1[4] > imagePointer1[0])))
                        {
                            imagePointer2[0] = 0;
                            imagePointer2[1] = 0;
                            imagePointer2[2] = 0;
                        }
                        else
                        {
                            imagePointer2[0] = imagePointer1[0];
                            imagePointer2[1] = imagePointer1[1];
                            imagePointer2[2] = imagePointer1[2];
                        }
                        imagePointer2[3] = 255;
                        //4 bytes per pixel
                        imagePointer1 += 4;
                        imagePointer2 += 4;
                    }
                    imagePointer1 += 8;
                    imagePointer2 += 8;
                }
            }
            returnMap.UnlockBits(bitmapData2);
            image.UnlockBits(bitmapData1);
            return returnMap;
        }


        public static Bitmap BlendBitmapsAndGenerateGrdient(Bitmap image1, Bitmap image2)
        {
            Bitmap returnMap = new Bitmap(image1.Width, image1.Height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData1 = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bitmapData2 = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = returnMap.LockBits(new Rectangle(0, 0, image1.Width, image1.Height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;
                byte* imagePointer2 = (byte*)bitmapData2.Scan0;
                byte* resultPointer = (byte*)resultData.Scan0;

                double blue = 0.0;
                double green = 0.0;
                double red = 0.0;
                gradientDirections = new double[bitmapData1.Height, bitmapData1.Width];
                for (int i = 0; i < bitmapData1.Height; i++)
                {
                    for (int j = 0; j < bitmapData1.Width; j++)
                    {
                    	blue = (imagePointer2[0]  + imagePointer1[0]);
                    	green = (imagePointer2[1] + imagePointer1[1]);
                    	red = (imagePointer2[2] + imagePointer1[2]);

                        gradientDirections[i, j] = Math.Atan(imagePointer2[0] / (imagePointer1[0] == 0 ? 0.01 : imagePointer1[0])); // Считаем по красному, все равно черно-белая картинка.

                        blue = blue > 255 ? 255 : blue;
                        green = green > 255 ? 255 : green;
                        red = red > 255 ? 255 : red;

                        resultPointer[0] = (byte)blue;
                        resultPointer[1] = (byte)green;
                        resultPointer[2] = (byte)red;
                        resultPointer[3] = 255;
                        //4 bytes per pixel
                        imagePointer1 += 4;
                        imagePointer2 += 4;
                        resultPointer += 4;
                    }
                    imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4);
                    imagePointer2 += bitmapData2.Stride - (bitmapData1.Width * 4);
                    resultPointer += resultData.Stride - (resultData.Width * 4);
                }
            }
            returnMap.UnlockBits(bitmapData2);
            image1.UnlockBits(bitmapData1);
            image2.UnlockBits(bitmapData1);
            return returnMap;
        }

        static Bitmap GenerateGradients(this Bitmap sourceBitmap, double[,] kernel, double factor = 1, int bias = 0)  
		{
    		BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
			                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length); 
    		sourceBitmap.UnlockBits(sourceData); 
    		
    		double blue = 0.0; 
    		double green = 0.0; 
    		double red = 0.0;

            int filterWidth = kernel.GetLength(1);
            int filterHeight = kernel.GetLength(0); 

   
    		int filterOffset = (filterWidth-1) / 2; 
    		int calcOffset = 0; 

   
    		int byteOffset = 0; 

   
    		for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
    		{
        		for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++) 
        		{
            		blue = 0; 
            		green = 0; 
            		red = 0; 

            		byteOffset = offsetY * sourceData.Stride + offsetX * 4; 

            		for (int filterY = -filterOffset; filterY <= filterOffset; filterY++) 
            		{ 
                		for (int filterX = -filterOffset; filterX <= filterOffset; filterX++) 
                		{ 
                    		calcOffset = byteOffset +  (filterX * 4) + (filterY * sourceData.Stride);

                            blue += (double)(pixelBuffer[calcOffset]) * kernel[filterY + filterOffset, filterX + filterOffset];
                            green += (double)(pixelBuffer[calcOffset + 1]) * kernel[filterY + filterOffset, filterX + filterOffset]; 
                    		red += (double)(pixelBuffer[calcOffset + 2]) * kernel[filterY + filterOffset, filterX + filterOffset]; 
                		} 
            		} 

            		blue = Math.Abs(factor * blue + bias); 
            		green = Math.Abs(factor * green + bias); 
            		red = Math.Abs(factor * red + bias); 

            		blue = blue > 255 ? 255 : blue; 
            		green = green > 255 ? 255 : green; 
            		red = red > 255 ? 255 : red;

            		resultBuffer[byteOffset] = (byte)(blue); 
            		resultBuffer[byteOffset + 1] = (byte)(green); 
            		resultBuffer[byteOffset + 2] = (byte)(red); 
            		resultBuffer[byteOffset + 3] = 255; 
        		}
    		}

   
    		Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height); 

    		BitmapData resultData = resultBitmap.LockBits(new Rectangle (0, 0, resultBitmap.Width, resultBitmap.Height), 
    		                                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

    		Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length); 
    		resultBitmap.UnlockBits(resultData); 

    		return resultBitmap; 
		}

		public static string GaussKernelToString()
		{
			string str = "";
			if(gaussKernel == null)
			{
				return "no Gauss kernel";
			}
			int length = gaussKernel.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				str += " | ";
				for (int j = 0; j < length; j++)
				{
					str += gaussKernel[i,j] + " | ";
				}
				str += "\n";
			}
			return str;
		}
		
		static Bitmap ApplyFilter(this Bitmap sourceBitmap, double[,] kernel, double factor = 1, int bias = 0)  
		{
    		BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
			                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length); 
    		sourceBitmap.UnlockBits(sourceData); 
    		
    		double blue = 0.0; 
    		double green = 0.0; 
    		double red = 0.0;

            int filterWidth = kernel.GetLength(1);
            int filterHeight = kernel.GetLength(0); 

   
    		int filterOffset = (filterWidth-1) / 2; 
    		int calcOffset = 0; 

   
    		int byteOffset = 0; 

   
    		for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
    		{
        		for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++) 
        		{
            		blue = 0; 
            		green = 0; 
            		red = 0; 

            		byteOffset = offsetY * sourceData.Stride + offsetX * 4; 

            		for (int filterY = -filterOffset; filterY <= filterOffset; filterY++) 
            		{ 
                		for (int filterX = -filterOffset; filterX <= filterOffset; filterX++) 
                		{ 
                    		calcOffset = byteOffset +  (filterX * 4) + (filterY * sourceData.Stride);

                            blue += (double)(pixelBuffer[calcOffset]) * kernel[filterY + filterOffset, filterX + filterOffset];
                            green += (double)(pixelBuffer[calcOffset + 1]) * kernel[filterY + filterOffset, filterX + filterOffset]; 
                    		red += (double)(pixelBuffer[calcOffset + 2]) * kernel[filterY + filterOffset, filterX + filterOffset]; 
                		} 
            		} 

            		blue = Math.Abs(factor * blue + bias); 
            		green = Math.Abs(factor * green + bias); 
            		red = Math.Abs(factor * red + bias); 

            		blue = blue > 255 ? 255 : blue; 
            		green = green > 255 ? 255 : green; 
            		red = red > 255 ? 255 : red;

            		resultBuffer[byteOffset] = (byte)(blue); 
            		resultBuffer[byteOffset + 1] = (byte)(green); 
            		resultBuffer[byteOffset + 2] = (byte)(red); 
            		resultBuffer[byteOffset + 3] = 255; 
        		}
    		}

   
    		Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height); 

    		BitmapData resultData = resultBitmap.LockBits(new Rectangle (0, 0, resultBitmap.Width, resultBitmap.Height), 
    		                                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

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
