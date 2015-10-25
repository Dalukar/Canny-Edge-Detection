using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CannyEdgeDetection
{
	/// <summary>
	/// Класс для обнаружения границ на изображении.
	/// </summary>
	public static class CannyEdgeDetector
	{
		static public int gaussKernelSize = 5;              // Размер ядра для гаусса.
        static public double gaussKernelDeviation = 1.4;    // Отклонение гауссиана.
		static public byte strongThreshold = 40;            // Граница яркости для сильных граней.
        static public byte weakThreshold = 5;               // Граница яркости для слабых граней.
        static public Bitmap originalimg;                   // Картинки с промежуточными этапами.
        static public Bitmap afterGaussImg;                 //
        static public Bitmap afterCannyImg;                 //
        static public Bitmap afterSupressionImg;            //
        static public Bitmap afterThresholdImg;             //
        static public Bitmap afterBLOBsDetectImg;           //
        static double [,] gaussKernel;                      // Матрица для сглаживания.
        static double [,] GradientX;                        // Матрицы градиентов по осям.
        static double [,] GradientY;                        //
        static double [,] gradientDirections;               // Матрица направлений градиентов.
        static double [,] CannyKernelX = new double[3, 3]{{-1, 0, 1},   // Матрицы для вычисления градиентов.
                                                          {-2, 0, 2},   //
                                                          {-1, 0, 1}};  //
        static double[,] CannyKernelY = new double[3, 3] {{ 1, 2, 1},   //
                                                          { 0, 0, 0},   //
                                                          {-1,-2,-1}};  //
        /// <summary>
        /// Вычисление границ на изображении, также сохраняет промежуточные результаты в соответствующие переменные класса.
        /// </summary>
        public static Bitmap CalculateEdges(Bitmap sourceBitmap)
        {
            // Поочередне выполнение всех этапов.
			CalculateGaussKernel(gaussKernelSize, gaussKernelDeviation);
			originalimg = ToGrayscale(sourceBitmap);
			afterGaussImg = ApplyFilter(originalimg, gaussKernel);
            afterCannyImg = GenerateGradients(afterGaussImg);
            afterSupressionImg = NonMaximumSuppression(afterCannyImg);
            afterThresholdImg = DoubleTrheshold(afterSupressionImg, strongThreshold, weakThreshold);
            afterBLOBsDetectImg = FindBLOBs(afterThresholdImg);
            return afterThresholdImg;
        }

        /// <summary>
        /// Расчет матрицы для сглаживания.
        /// </summary>
		static void CalculateGaussKernel(int length, double weight) 
		{
		    gaussKernel = new double [length, length];
		    double sumTotal = 0; 		
		    int kernelRadius = length / 2; 
		    double distance = 0; 
		    
            // Расчет постоянной части формулы для всех ячеек.
		    double calculatedEuler = 1.0 / (2.0 * Math.PI * Math.Pow(weight, 2));

            // Перебор всех ячеек матрицы и расчет их значений.
		    for (int filterY = -kernelRadius; filterY <= kernelRadius; filterY++) 
		    {
		        for (int filterX = -kernelRadius; filterX <= kernelRadius; filterX++) 
		        {
		            distance = ((filterX * filterX) + (filterY * filterY)) / (2 * (weight * weight));
                    gaussKernel[filterY + kernelRadius, filterX + kernelRadius] = calculatedEuler * Math.Exp(-distance);
                    // расчет суммарного значения.
		            sumTotal += gaussKernel[filterY + kernelRadius, filterX + kernelRadius]; 
		        } 
		    } 

            // Приведение матрицы к состоянию, при котором сумма ячеек равна 1.
		    for (int y = 0; y < length; y++) 
		    { 
		        for (int x = 0; x < length; x++) 
		        { 
		            gaussKernel[y, x] = gaussKernel[y, x] *  (1.0 / sumTotal); 
		        } 
		    } 
		}

        /// <summary>
        /// Конвертация изображения в оттенки серого.
        /// </summary>
		static Bitmap ToGrayscale(Bitmap image)
		{
            // Создание изображения для воврата и получение доступа к пикселям.
            Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int gray = 0;
    		unsafe {

                // получение указателей на пиксели.
        		byte* imagePointer1 = (byte*)bitmapData1.Scan0;
        		byte* imagePointer2 = (byte*)bitmapData2.Scan0;

                //Перебор всех пикселей на изображении.
        		for(int i = 0; i < bitmapData1.Height; i++) {
            		for(int j = 0; j < bitmapData1.Width; j++) {

                        // усреднение цвета и запись его на все каналы выходного пикселя, альфа канал всегда остается = 255.
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
        /// Применение фильтра с указанной матрицей.
        /// </summary>
		static Bitmap ApplyFilter(this Bitmap sourceBitmap, double[,] kernel)
		{
            // Создание массива изображения для воврата и получение доступа к пикселям.
    		BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
			                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length); 
    		sourceBitmap.UnlockBits(sourceData); 
    		
    		double blue = 0.0; 
    		double green = 0.0; 
    		double red = 0.0;

            int filterWidth = kernel.GetLength(1);  // Размер фильтра.
            int filterHeight = kernel.GetLength(0); 

    		int filterOffset = (filterWidth-1) / 2; // Отступ за счет размера фильтра.
            int calcOffset = 0;                     // Отступ указателя до соседнего пикселя.

    		int byteOffset = 0;                     // Отступ указателя до текущего пикселя.

            // Перебор всех пикселей изображения, кроме крайних.
    		for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
    		{
        		for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++) 
        		{
            		blue = 0; 
            		green = 0; 
            		red = 0; 

            		byteOffset = offsetY * sourceData.Stride + offsetX * 4; 

                    // Перебор соседних пикселей и расчет значения пикселя после сглаживания.
            		for (int filterY = -filterOffset; filterY <= filterOffset; filterY++) 
            		{ 
                		for (int filterX = -filterOffset; filterX <= filterOffset; filterX++) 
                		{ 
                    		calcOffset = byteOffset +  (filterX * 4) + (filterY * sourceData.Stride);

                            // Значение соседнего пикселя умножается на значение в матрице и прибавляется к текущему.
                            blue += (double)(pixelBuffer[calcOffset]) * kernel[filterY + filterOffset, filterX + filterOffset];
                            green += (double)(pixelBuffer[calcOffset + 1]) * kernel[filterY + filterOffset, filterX + filterOffset]; 
                    		red += (double)(pixelBuffer[calcOffset + 2]) * kernel[filterY + filterOffset, filterX + filterOffset]; 
                		} 
            		} 

                    // Исключение возможности переполнения размера в 1 байт.
            		blue = blue > 255 ? 255 : blue; 
            		green = green > 255 ? 255 : green; 
            		red = red > 255 ? 255 : red;

                    // Сохранение значения пикселя в масиве для возврата.
            		resultBuffer[byteOffset] = (byte)(blue); 
            		resultBuffer[byteOffset + 1] = (byte)(green); 
            		resultBuffer[byteOffset + 2] = (byte)(red); 
            		resultBuffer[byteOffset + 3] = 255; 
        		}
    		}

            // Создание изображения для возврата из массива.
    		Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height); 
    		BitmapData resultData = resultBitmap.LockBits(new Rectangle (0, 0, resultBitmap.Width, resultBitmap.Height), 
    		                                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    		Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length); 
    		resultBitmap.UnlockBits(resultData); 

    		return resultBitmap; 
		}
        
        /// <summary>
        /// Создание матриц градиентов и их направлений, возвращает изображение с суммой градиентов по осям.
        /// </summary>
		static Bitmap GenerateGradients(Bitmap sourceBitmap)  
		{
            // Создание массива изображения для воврата и получение доступа к пикселям.
    		BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
			                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);        	
    		byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];  
    		byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height]; 
    		Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length); 
    		sourceBitmap.UnlockBits(sourceData);  

            // создание массивов для хранения градиентов.
    		GradientX = new double[sourceBitmap.Height, sourceBitmap.Width];
            GradientY = new double[sourceBitmap.Height, sourceBitmap.Width];
            gradientDirections = new double[sourceBitmap.Height, sourceBitmap.Width];

            int filterWidth = CannyKernelX.GetLength(1);    // Размер фильтра.
            int filterHeight = CannyKernelX.GetLength(0);   // 
            int filterOffset = (filterWidth - 1) / 2;       // Отступ за счет размера фильтра.
            int calcOffset = 0;                             // Отступ указателя до соседнего пикселя.
    		double gradX;                                   // Градиенты по осям.
    		double gradY;                                   //
            int byteOffset = 0;                             // Отступ указателя до текущего пикселя.


            // Перебор всех пикселей изображения, кроме крайних.
    		for (int offsetX = filterOffset; offsetX < sourceBitmap.Height - filterOffset; offsetX++)
    		{
        		for (int offsetY = filterOffset; offsetY < sourceBitmap.Width - filterOffset; offsetY++) 
        		{
            		gradX = 0; 
            		gradY = 0; 

            		byteOffset = offsetX * sourceData.Stride + offsetY * 4;
                    // Перебор соседних пикселей и расчет значений градиентов.
            		for (int filterY = -filterOffset; filterY <= filterOffset; filterY++) 
            		{ 
                		for (int filterX = -filterOffset; filterX <= filterOffset; filterX++) 
                		{ 
                    		calcOffset = byteOffset +  (filterY * 4) + (filterX * sourceData.Stride);

                            // Значение соседнего пикселя умножается на значение в матрице и прибавляется к градиенту в точке.
                            gradX += (double)(pixelBuffer[calcOffset]) * CannyKernelX[filterX + filterOffset, filterY + filterOffset];
                            gradY += (double)(pixelBuffer[calcOffset]) * CannyKernelY[filterX + filterOffset, filterY + filterOffset];
                		} 
            		} 

            		GradientX[offsetX, offsetY] = gradX;
            		GradientY[offsetX, offsetY] = gradY;

                    // Расчет вектора градиента, исключаем деление на 0.
            		gradientDirections[offsetX, offsetY] = Math.Atan(gradY / (gradX == 0 ? 0.01 : gradX)); 

                    // расчет значения пикселя для отображения.
            		double pixelValue = (Math.Abs(gradX) + Math.Abs(gradY))/2;

                    // Исключение возможности переполнения размера в 1 байт.
                    pixelValue = pixelValue > 255 ? 0 : pixelValue;

                    // Запись значения в массив.
            		resultBuffer[byteOffset] = (byte)(pixelValue); 
            		resultBuffer[byteOffset + 1] = (byte)(pixelValue); 
            		resultBuffer[byteOffset + 2] = (byte)(pixelValue); 
            		resultBuffer[byteOffset + 3] = 255; 
        		}
    		}

            // Создание изображения для возврата из массива.
    		Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height); 
    		BitmapData resultData = resultBitmap.LockBits(new Rectangle (0, 0, resultBitmap.Width, resultBitmap.Height), 
    		                                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    		Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length); 
    		resultBitmap.UnlockBits(resultData); 

    		return resultBitmap; 
		}
        
        /// <summary>
        /// Удаление точек, находящихся вне локальных максимумов.
        /// </summary>
        static Bitmap NonMaximumSuppression (Bitmap image)
        {
            // Создание изображения для воврата и получение доступа к пикселям.
            Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                // исключение границ изображения.
                byte* imagePointer1 = (byte*)bitmapData1.Scan0 + bitmapData1.Stride + 4;
                byte* imagePointer2 = (byte*)bitmapData2.Scan0 + bitmapData2.Stride + 4;

                // Перебор всех точек изображения.
                for (int i = 1; i < bitmapData1.Height-1; i++)
                {
                    for (int j = 1; j < bitmapData1.Width-1; j++)
                    {
                        // Проверка условий локального максимума.
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
                            // если не максимум, то 0.
                            imagePointer2[0] = 0;
                            imagePointer2[1] = 0;
                            imagePointer2[2] = 0;
                        }
                        else
                        {
                            // Иначе сохраняем пиксель.
                            imagePointer2[0] = imagePointer1[0];
                            imagePointer2[1] = imagePointer1[1];
                            imagePointer2[2] = imagePointer1[2];
                        }
                        imagePointer2[3] = 255;
                        //4 байта на пиксель.
                        imagePointer1 += 4;
                        imagePointer2 += 4;
                    }

                    // исключение границ изображения.
                    imagePointer1 += 8;
                    imagePointer2 += 8;
                }
            }
            returnMap.UnlockBits(bitmapData2);
            image.UnlockBits(bitmapData1);
            return returnMap;
        }

        /// <summary>
        /// Удаление нечетких границ и разбиение оставшихся на сильные и слабые.
        /// </summary>
        static Bitmap DoubleTrheshold(Bitmap image, byte strongThreshold, byte weakThreshhol)
		{
            // Создание изображения для воврата и получение доступа к пикселям.
            Bitmap returnMap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		BitmapData bitmapData2 = returnMap.LockBits(new Rectangle(0, 0, returnMap.Width, returnMap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    		unsafe {

                // Перебор всех точек изображения.
        		byte* imagePointer1 = (byte*)bitmapData1.Scan0;
        		byte* imagePointer2 = (byte*)bitmapData2.Scan0;
        		byte pixelValue;
        		for(int i = 0; i < bitmapData1.Height; i++) {
            		for(int j = 0; j < bitmapData1.Width; j++) {

                        // Проверка яркости точки.
        				if(imagePointer1[0] > strongThreshold)
        				{
                            // Сильная грань.
        					pixelValue = 255;
        				}
        				else if(imagePointer1[0] > weakThreshhol)
        				{
                            // Слабая грань.
        					pixelValue = 50;
        				}
        				else
        				{
                            // Шум.
        					pixelValue = 0;
        				}

                        // Запись результаа в выходное изображение.
                		imagePointer2[0] = pixelValue;
                        imagePointer2[1] = pixelValue;
                        imagePointer2[2] = pixelValue;
                        imagePointer2[3] = 255;

                		//4 байта на пиксель.
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
        /// Поиск и отображение BLOB'ов на изображении.
        /// </summary>
        static Bitmap FindBLOBs(Bitmap image)
		{
            // Создание изображения для воврата и получение доступа к пикселям.
            Bitmap imageCopy = new Bitmap(image);
    		BitmapData bitmapData = imageCopy.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
    	                                        	ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            List<List<int[]>> BLOBs = new List<List<int[]>>(); // Список BLOB'ов.
    		unsafe {

                // Перебор всех пикселей.
        		byte* imagePointer = (byte*)bitmapData.Scan0;
        		for(int i = 0; i < bitmapData.Height; i++) {
            		for(int j = 0; j < bitmapData.Width; j++) {
                        if (imagePointer[0] != 0)
                        {
                            // Если пиксель !=0, создаем новый BLOB и начинаем поиск смежных пикселей.
                            List<int[]> BLOB = new List<int[]>();
                            if(FindConnectedPixels(imagePointer, i, j, bitmapData.Height, bitmapData.Width, BLOB))
                            {
                                // Если грань сильная, заносим ее в список.
                                BLOBs.Add(BLOB);
                            }
                        }
                		// 4 байта на пиксель.
                		imagePointer += 4;
            		}
        		}

                // Конечная отрисовка всех найденных граней.
                imagePointer = (byte*)bitmapData.Scan0;
                int pointOffset;
                // Каждый BLOB из списка отрисовывается попиксельно.
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
            // Проверка на сильную грань.
            bool isStrong = pointer[0] == 255;

            //добавление текущей точки в массив.
            BLOB.Add(new int[] { x, y });

            // Затирание текущей точки для исключения повторного добавления.
            pointer[0] = 0;
            pointer[1] = 0;
            pointer[2] = 0;

            int calcOffset = 0;  // отступ указателя до соседней точки.

            // Перебор соседних точек.
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    calcOffset = (j * 4) + (i * width * 4);

                    // Если соседняя точка не пустая, вызываем функцию еще раз.
                    if (x + i < height && y + j < width && pointer[calcOffset] != 0)
                    {
                        byte* ptr = &pointer[calcOffset]; // указатель на соседнюю точку

                        // Проверка наличия сильных граней в остальных пикселях.
                        isStrong = isStrong | FindConnectedPixels(ptr, x + i, y + j, height, width, BLOB); 
                    }
                }
            }
            return isStrong;    	
        }	
	}
}
