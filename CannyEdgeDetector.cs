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
using System.Collections.Generic;

namespace CannyEdgeDetection
{
	/// <summary>
	/// Класс для обнаружения границ.
	/// </summary>
	public static class CannyEdgeDetector
	{
		static public int gaussKernelSize = 5; // размер матрицы для сглаживания, должен быть нечетный
		static public double gaussKernelDeviation = 1.4; // ослабление, хз как правильно называется
		static public byte strongThreshold = 40; // граница яркости для сильных граней
        static public byte weakThreshhol = 5; // граница яркости для слабых граней
        static public Bitmap originalimg; // картинки с промежуточными этапами
        static public Bitmap afterGaussImg;
        static public Bitmap afterCannyImg;
        static public Bitmap afterSupressionImg;
        static public Bitmap afterThresholdImg;
        static public Bitmap afterBLOBsDetectImg;
        static double [,] gaussKernel; // матрица для сглаживания
        static double [,] GradientX; // матрицы градиентов по осям
        static double [,] GradientY;
        static double [,] gradientDirections; // матрица направлений градиентов
        static double [,] CannyKernelX = new double[3, 3]{{-1, 0, 1}, // матрицы для вычисления градиентов
                                                          {-2, 0, 2}, 
                                                          {-1, 0, 1}};
        static double[,] CannyKernelY = new double[3, 3] {{ 1, 2, 1},
                                                          { 0, 0, 0}, 
                                                          {-1,-2,-1}};
        /// <summary>
        /// Вычисление границ на изображении, также сохраняет промежуточные результаты в соответствующие переменные класса
        /// </summary>
        public static Bitmap CalculateEdges(Bitmap sourceBitmap)
        {
			CalculateGaussKernel(gaussKernelSize, gaussKernelDeviation);
			originalimg = ToGrayscale(sourceBitmap);
			afterGaussImg = ApplyFilter(originalimg, gaussKernel);
            afterCannyImg = GenerateGradients(afterGaussImg);
            afterSupressionImg = NonMaximumSuppression(afterCannyImg);
            afterThresholdImg = DoubleTrheshold(afterSupressionImg, strongThreshold, weakThreshhol);
            afterBLOBsDetectImg = FindBLOBs(afterThresholdImg);
            return afterThresholdImg;
        }

        /// <summary>
        /// Расчет матрицы для сглаживания
        /// </summary>
		static void CalculateGaussKernel(int length, double weight) 
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

        /// <summary>
        /// Конвертация изображения в оттенки серого
        /// </summary>
		static Bitmap ToGrayscale(Bitmap image)
		{
            // Создание изображения для воврата и получение доступа к пикселям
            Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int gray = 0;
    		unsafe {

                // получение указателей на пиксели
        		byte* imagePointer1 = (byte*)bitmapData1.Scan0;
        		byte* imagePointer2 = (byte*)bitmapData2.Scan0;

                //перебор всех пикселей на изображении
        		for(int i = 0; i < bitmapData1.Height; i++) {
            		for(int j = 0; j < bitmapData1.Width; j++) {

                        // усреднение цвета и запись его на все каналы выходного пикселя, альфа канал всегда остается = 255
                        gray = (imagePointer1[0] + imagePointer1[1] + imagePointer1[2]) / 3;  
                		imagePointer2[0] = (byte)gray;
                        imagePointer2[1] = (byte)gray;
                        imagePointer2[2] = (byte)gray;
                        imagePointer2[3] = 255;
                		imagePointer1 += 4;
                		imagePointer2 += 4;
            		}
        		}
    		}
    		returnMap.UnlockBits(bitmapData2);
    		image.UnlockBits(bitmapData1);
    		return returnMap;
		}

        /// <summary>
        /// применение фильтра с указанной матрицей 
        /// </summary>
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
        
        /// <summary>
        /// Создание матриц градиентов и их направлений, возвращает изображение с суммой градиентов по осям
        /// </summary>
		static Bitmap GenerateGradients(Bitmap sourceBitmap)  
		{
    		BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
			                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);        	
    		byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];  
    		byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length); 
    		sourceBitmap.UnlockBits(sourceData);  

    		GradientX = new double[sourceBitmap.Height, sourceBitmap.Width];
            GradientY = new double[sourceBitmap.Height, sourceBitmap.Width];
            gradientDirections = new double[sourceBitmap.Height, sourceBitmap.Width];
    		
            int filterWidth = CannyKernelX.GetLength(1);
            int filterHeight = CannyKernelX.GetLength(0); 
    		int filterOffset = (filterWidth-1) / 2; 
    		int calcOffset = 0; 
    		double gradX;
    		double gradY;
    		int byteOffset = 0; 

    		for (int offsetX = filterOffset; offsetX < sourceBitmap.Height - filterOffset; offsetX++)
    		{
        		for (int offsetY = filterOffset; offsetY < sourceBitmap.Width - filterOffset; offsetY++) 
        		{
            		gradX = 0; 
            		gradY = 0; 

            		byteOffset = offsetX * sourceData.Stride + offsetY * 4; 

            		for (int filterY = -filterOffset; filterY <= filterOffset; filterY++) 
            		{ 
                		for (int filterX = -filterOffset; filterX <= filterOffset; filterX++) 
                		{ 
                    		calcOffset = byteOffset +  (filterY * 4) + (filterX * sourceData.Stride);
                            gradX += (double)(pixelBuffer[calcOffset]) * CannyKernelX[filterX + filterOffset, filterY + filterOffset];
                            gradY += (double)(pixelBuffer[calcOffset]) * CannyKernelY[filterX + filterOffset, filterY + filterOffset];
                		} 
            		} 
            		// Считаем по красному, все равно картинка черно-белая.
            		GradientX[offsetX, offsetY] = gradX;
            		GradientY[offsetX, offsetY] = gradY;
            		gradientDirections[offsetX, offsetY] = Math.Atan(gradY / (gradX == 0 ? 0.01 : gradX)); 

            		double pixelValue = (Math.Abs(gradX) + Math.Abs(gradY))/2;
                    pixelValue = pixelValue > 255 ? 0 : pixelValue;

            		resultBuffer[byteOffset] = (byte)(pixelValue); 
            		resultBuffer[byteOffset + 1] = (byte)(pixelValue); 
            		resultBuffer[byteOffset + 2] = (byte)(pixelValue); 
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
        
        /// <summary>
        /// Удаление точек, находящихся вне локальных максимумов
        /// </summary>
        static Bitmap NonMaximumSuppression (Bitmap image)
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
                        if (((grad >= Math.PI * 3/8 || grad <= -Math.PI * 3/8) &&
                             		(imagePointer1[-bitmapData1.Stride] > imagePointer1[0] || imagePointer1[bitmapData1.Stride] > imagePointer1[0])) ||
                             ((grad > Math.PI * 1/8 && grad < Math.PI * 3/8) &&
	                              	(imagePointer1[-bitmapData1.Stride + 4] > imagePointer1[0] || imagePointer1[bitmapData1.Stride - 4] > imagePointer1[0])) ||
                             ((grad <= Math.PI * 1/8 && grad >= -Math.PI * 1/8) &&
                              		(imagePointer1[-4] > imagePointer1[0] || imagePointer1[4] > imagePointer1[0])) ||
                             ((grad < -Math.PI * 1/8 && grad > -Math.PI * 3/8) &&
                              		(imagePointer1[-bitmapData1.Stride - 4] > imagePointer1[0] || imagePointer1[bitmapData1.Stride + 4] > imagePointer1[0])))
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

        /// <summary>
        /// Удаление нечетких границ и разбиение оставшихся на сильные и слабые
        /// </summary>
        static Bitmap DoubleTrheshold(Bitmap image, byte strongThreshold, byte weakThreshhol)
		{
            Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		unsafe {
        		byte* imagePointer1 = (byte*)bitmapData1.Scan0;
        		byte* imagePointer2 = (byte*)bitmapData2.Scan0;
        		byte pixelValue;
        		for(int i = 0; i < bitmapData1.Height; i++) {
            		for(int j = 0; j < bitmapData1.Width; j++) {
        				if(imagePointer1[0] > strongThreshold)
        				{
        					pixelValue = 255;
        				}
        				else if(imagePointer1[0] > weakThreshhol)
        				{
        					pixelValue = 50;
        				}
        				else
        				{
        					pixelValue = 0;
        				}
                		imagePointer2[0] = pixelValue;
                        imagePointer2[1] = pixelValue;
                        imagePointer2[2] = pixelValue;
                        imagePointer2[3] = 255;
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

        /// <summary>
        /// Поиск и отображение BLOB'ов на изображении 
        /// </summary>
        static Bitmap FindBLOBs(Bitmap image)
		{
            Bitmap imageCopy = new Bitmap(image);
    		BitmapData bitmapData = imageCopy.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            List<List<int[]>> BLOBs = new List<List<int[]>>();
    		unsafe {
        		byte* imagePointer = (byte*)bitmapData.Scan0;
        		for(int i = 0; i < bitmapData.Height; i++) {
            		for(int j = 0; j < bitmapData.Width; j++) {
                        if (imagePointer[0] != 0)
                        {
                            List<int[]> BLOB = new List<int[]>();
                            if(FindConnectedPixels(imagePointer, i, j, bitmapData.Height, bitmapData.Width, BLOB))
                            {
                                BLOBs.Add(BLOB);
                            }
                        }
                		//4 bytes per pixel
                		imagePointer += 4;
            		}
        		}
                imagePointer = (byte*)bitmapData.Scan0;
                int pointOffset;
                foreach(List<int[]> BLOB in BLOBs)
                {
                    foreach(int[] point in BLOB)
                    {
                        pointOffset = (point[1] * 4) + (point[0] * bitmapData.Width * 4);
                        imagePointer[pointOffset] = 255;
                        imagePointer[pointOffset + 1] = 255;
                        imagePointer[pointOffset + 2] = 255;
                    }
                }
    		}
    		imageCopy.UnlockBits(bitmapData);
            return imageCopy;
		}

        /// <summary>
        /// Метод рекурсивно набирает пиксели в указанный массив BLOBа, возвращает true если BLOB сильный.
        /// </summary>
        unsafe static bool FindConnectedPixels(byte* pointer, int x, int y, int height, int width, List<int[]> BLOB)
        {
            bool isStrong = pointer[0] == 255;
            BLOB.Add(new int[] { x, y });
            pointer[0] = 0;
            pointer[1] = 0;
            pointer[2] = 0;
            int calcOffset = 0; 
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    calcOffset = (j * 4) + (i * width * 4);
                    if (x + i < height && y + j < width && pointer[calcOffset] != 0)
                    {
                        byte* ptr = &pointer[calcOffset];
                        isStrong = isStrong | FindConnectedPixels(ptr, x + i, y + j, height, width, BLOB);
                    }
                }
            }
            return isStrong;
        	
        }

			
	}
}
