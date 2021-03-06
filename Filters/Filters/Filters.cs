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
        //protected abstract Color calculateNewPixelColor(Bitmap sourceImage, Bitmap _secondImage, int x, int y);
        virtual public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
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

    class MyColor : IComparable
    {
        public int R;
        public int G;
        public int B;
        public double I;
        public MyColor(int R, int G, int B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.I = 0.36 * R + 0.53 * G + 0.11 * B;
        }
        public int CompareTo(object obj)
        {
            MyColor ob = (MyColor)obj;
            if (I > ob.I)
                return 1;
            if (I == ob.I)
                return 0;
            return -1;
        }
    }


    class MedianFilter : Filters
    {
        private Color medianColor;
        private int size;

        public MedianFilter(int size = 3)
        {
            this.size = size;
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            return medianColor;
        }

        override public Bitmap processImage(Bitmap sourseImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);

            var arrayColor = new List<MyColor>();
            int R, G, B;
            MyColor color;

            for (int y = size / 2; y < sourseImage.Height - size / 2; y++)
            {
                worker.ReportProgress((int)((float)(y - size / 2) / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int x = size / 2; x < sourseImage.Width - size / 2; x++)
                {
                    arrayColor.Clear();

                    for (int j = -size / 2; j <= size / 2; j++)
                    {
                        for (int i = -size / 2; i <= size / 2; i++)
                        {
                            R = sourseImage.GetPixel(x + i, y + j).R;
                            G = sourseImage.GetPixel(x + i, y + j).G;
                            B = sourseImage.GetPixel(x + i, y + j).B;
                            color = new MyColor(R, G, B);
                            arrayColor.Add(color);
                        }
                    }

                    arrayColor.Sort();

                    R = arrayColor.ElementAt(size * size / 2).R;
                    G = arrayColor.ElementAt(size * size / 2).G;
                    B = arrayColor.ElementAt(size * size / 2).B;
                    medianColor = Color.FromArgb(R, G, B);

                    resultImage.SetPixel(x - size / 2, y - size / 2, calculateNewPixelColor(sourseImage, x - size / 2, y - size / 2));
                }
            }
            return resultImage;

        }
    }

    class LinearStrain : Filters
    {
        int MaxR;
        int MaxG;
        int MaxB;
        int MinR;
        int MinG;
        int MinB;

        private void setMaxRGB(Bitmap sourceImage)
        {


            MyColor color;

            int R;
            int G;
            int B;


            var list = new List<MyColor>();

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    R = sourceImage.GetPixel(i, j).R;
                    G = sourceImage.GetPixel(i, j).G;
                    B = sourceImage.GetPixel(i, j).B;
                    color = new MyColor(R, G, B);
                    list.Add(color);
                }
            }

            list.Sort();

            MaxR = list.ElementAt((sourceImage.Width - 1) * (sourceImage.Height - 1)).R;
            MaxG = list.ElementAt((sourceImage.Width - 1) * (sourceImage.Height - 1)).G;
            MaxB = list.ElementAt((sourceImage.Width - 1) * (sourceImage.Height - 1)).B;

            MinR = list.ElementAt(0).R;
            MinG = list.ElementAt(0).G;
            MinB = list.ElementAt(0).B;
        }


        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int R = (sourseImage.GetPixel(x, y).R - MinR) * (255 / (MaxR - MinR));
            int G = (sourseImage.GetPixel(x, y).G - MinG) * (255 / (MaxG - MinG));
            int B = (sourseImage.GetPixel(x, y).B - MinB) * 255 / (MaxB - MinB);
            return Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(B, 0, 255));
        }
        public override Bitmap processImage(Bitmap sourseImage, BackgroundWorker worker)
        {
            setMaxRGB(sourseImage);
            return base.processImage(sourseImage, worker);
        }
    }

    class GrayWorldFilter : Filters
    {
        float coef;
        float coefR;
        float coefG;
        float coefB;
        public GrayWorldFilter(ref Bitmap sourceImage)
        {
            coef = 0;
            coefR = 0;
            coefG = 0;
            coefB = 0;
            float n = sourceImage.Width * sourceImage.Height;
            for (int x = 0; x < sourceImage.Width; x++)
                for (int y = 0; y < sourceImage.Height; y++)
                {
                    coefR += sourceImage.GetPixel(x, y).R / n;
                    coefG += sourceImage.GetPixel(x, y).G / n;
                    coefB += sourceImage.GetPixel(x, y).B / n;
                }
            coef = (coefR + coefG + coefB) / 3;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            return Color.FromArgb
                 (Clamp((int)(coef / coefR * sourceColor.R), 0, 255),
                 Clamp((int)(coef / coefG * sourceColor.G), 0, 255),
                 Clamp((int)(coef / coefB * sourceColor.B), 0, 255));
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

    static class MathMorfology
    {
        static public Bitmap Erosion(Bitmap sourceImage, bool[,] matr)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            int MH = matr.GetLength(0);
            int MW = matr.GetLength(1);

            int minR;
            int minG;
            int minB;

            Color sourceColor;

            for (int y = MH / 2; y < sourceImage.Height - MH / 2; y++)
            {
                for (int x = MW / 2; x < sourceImage.Width - MW / 2; x++)
                {
                    sourceColor = sourceImage.GetPixel(x - MH / 2, y - MW / 2);
                    minR = sourceColor.R;
                    minG = sourceColor.G;
                    minB = sourceColor.B;

                    for (int j = -MH / 2; j <= MH / 2; j++)
                    {
                        for (int i = -MW / 2; i <= MW / 2; i++)
                        {
                            if (matr[i + MH / 2, j + MW / 2])
                            {
                                if (sourceImage.GetPixel(x + i, y + j).R < minR)
                                {
                                    minR = sourceImage.GetPixel(x + i, y + j).R;
                                }
                                if (sourceImage.GetPixel(x + i, y + j).G < minG)
                                {
                                    minG = sourceImage.GetPixel(x + i, y + j).G;
                                }
                                if (sourceImage.GetPixel(x + i, y + j).B < minB)
                                {

                                    minB = sourceImage.GetPixel(x + i, y + j).B;
                                }
                            }
                        }
                    }
                    resultImage.SetPixel(x - MH / 2, y - MW / 2, Color.FromArgb(minR, minG, minB));

                }
            }
            return resultImage;

        }

        static public Bitmap Dilation(Bitmap sourceImage, bool[,] matr)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            int MH = matr.GetLength(0);
            int MW = matr.GetLength(1);

            int maxR;
            int maxG;
            int maxB;

            Color sourceColor;

            for (int y = MH / 2; y < sourceImage.Height - MH / 2; y++)
            {
                for (int x = MW / 2; x < sourceImage.Width - MW / 2; x++)
                {
                    sourceColor = sourceImage.GetPixel(x - MH / 2, y - MW / 2);
                    maxR = sourceColor.R;
                    maxG = sourceColor.G;
                    maxB = sourceColor.B;

                    for (int j = -MH / 2; j <= MH / 2; j++)
                    {
                        for (int i = -MW / 2; i <= MW / 2; i++)
                        {
                            if (matr[i + MH / 2, j + MW / 2])
                            {
                                if (sourceImage.GetPixel(x + i, y + j).R > maxR)
                                {
                                    maxR = sourceImage.GetPixel(x + i, y + j).R;
                                }
                                if (sourceImage.GetPixel(x + i, y + j).G > maxG)
                                {
                                    maxG = sourceImage.GetPixel(x + i, y + j).G;
                                }
                                if (sourceImage.GetPixel(x + i, y + j).B > maxB)
                                {

                                    maxB = sourceImage.GetPixel(x + i, y + j).B;
                                }
                            }

                        }
                    }
                    resultImage.SetPixel(x - MH / 2, y - MW / 2, Color.FromArgb(maxR, maxG, maxB));

                }
            }
            return resultImage;

        }

        static public Bitmap Gradient(Bitmap sourceImage, bool[,] matr)
        {

            Bitmap image1 = Dilation(sourceImage, matr);
            Bitmap image2 = Erosion(sourceImage, matr);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            int R;
            int G;
            int B;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    R = image1.GetPixel(i, j).R - image2.GetPixel(i, j).R;
                    G = image1.GetPixel(i, j).G - image2.GetPixel(i, j).G;
                    B = image1.GetPixel(i, j).B - image2.GetPixel(i, j).B;

                    resultImage.SetPixel(i, j, Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(G, 0, 255)));

                }
            }
            return resultImage;


        }
        static private int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }

}