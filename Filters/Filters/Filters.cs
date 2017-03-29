﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Filters
{
    abstract class Filters
    {
        public int Clamp (int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
            }
            return resultImage;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            double resultR = sourceColor.R * 0.36;
            double resultG = sourceColor.G * 0.53;
            double resultB = sourceColor.B * 0.11;
            double intensity = resultR + resultG + resultB;

            return Color.FromArgb(Clamp((int)intensity, 0, 255),
                                  Clamp((int)intensity, 0, 255),
                                   Clamp((int)intensity, 0, 255));
        }
    }

    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            double resultR = sourceColor.R * 0.36;
            double resultG = sourceColor.G * 0.53;
            double resultB = sourceColor.B * 0.11;
            double intensity = resultR + resultG + resultB;
            int k = 100;

            return Color.FromArgb(Clamp((int)(intensity + 2 * k), 0, 255),
                                  Clamp((int)(intensity + 0.5 * k), 0, 255),
                                  Clamp((int)(intensity - 1 * k), 0, 255));
        }
    }

    class BrightnessFilterPlus : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int k = 50;
            Color sourceColor = sourseImage.GetPixel(x, y);
            double resultR = sourceColor.R + k;
            double resultG = sourceColor.G + k;
            double resultB = sourceColor.B + k;

            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                 Clamp((int)resultG, 0, 255),
                                 Clamp((int)resultB, 0, 255));
        }
    }

    class BrightnessFilterMinus : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int k = 50;
            Color sourceColor = sourseImage.GetPixel(x, y);
            double resultR = sourceColor.R - k;
            double resultG = sourceColor.G - k;
            double resultB = sourceColor.B - k;

            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                 Clamp((int)resultG, 0, 255),
                                 Clamp((int)resultB, 0, 255));
        }
    }

    class RemoveFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int k = 100;
            double resultR;
            double resultG;
            double resultB;
            if (x + k < sourseImage.Width)
            {
                Color sourceColor = sourseImage.GetPixel(x + k, y);

                resultR = sourceColor.R;
                resultG = sourceColor.G;
                resultB = sourceColor.B;
            }
            else
            {
                resultR = 0;
                resultG = 0;
                resultB = 0;

            }
            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                  Clamp((int)resultG, 0, 255),
                                   Clamp((int)resultB, 0, 255));
        }
    }

    class SpinFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {

            int x0 = sourseImage.Width / 2,
                y0 = sourseImage.Height / 2;
            double spin = Math.PI / 6;

            double resultR;
            double resultG;
            double resultB;

            int newX = (int)((x - x0) * Math.Cos(spin) - (y - y0) * Math.Sin(spin) + x0);
            int newY = (int)((x - x0) * Math.Sin(spin) + (y - y0) * Math.Cos(spin) + y0);

            if (newX < 0 || newX >= sourseImage.Width || newY < 0 || newY >= sourseImage.Height)
            {
                resultR = 0;
                resultG = 0;
                resultB = 0;
            }
            else
            {
                Color sourceColor = sourseImage.GetPixel(newX, newY);


                resultR = sourceColor.R;
                resultG = sourceColor.G;
                resultB = sourceColor.B;
            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                         Clamp((int)resultG, 0, 255),
                          Clamp((int)resultB, 0, 255));
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }
    
    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }

    }

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKernel (int rad, float sigma)
        {
            // определяем размер ядра
            int size = 2 * rad + 1;
            // создаем ядро фильтра
            kernel = new float[size, size];
            // коэффициент нормировки ядра
            float norm = 0;
            // рассчитываем ядро линейного фильтра
            for (int i = -rad; i <= rad; i++)
                for (int j = -rad; j <= rad; j++)
                {
                    kernel[i + rad, j + rad] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + rad, j + rad];
                }
            // нормируем ядро
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }

        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    }

    class Sharpness : MatrixFilter
    {
        public Sharpness()
        {
            kernel = new float[3, 3];
            kernel[0, 0] = kernel[0, 1] = kernel[0, 2] = kernel[1, 0] = kernel[1, 2] = kernel[2, 0] = kernel[2, 1] = kernel[2, 2] = -1;
            kernel[1, 1] = 9;
        }
    }

    class Stamping : MatrixFilter
    {
        public Stamping()
        {
            kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };


            float norm = 2;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    kernel[i, j] /= norm;


        }

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int coef = 50;

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idx = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idy = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighborColor = sourseImage.GetPixel(idx, idy);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)(resultR + coef), 0, 255),
                Clamp((int)(resultG + coef), 0, 255),
                Clamp((int)(resultB + coef), 0, 255)
            );
        }
    }
}