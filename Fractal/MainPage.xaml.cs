using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Fractal
{
    public sealed partial class MainPage : Page
    {
        public static int width = 1000;
        public static int height = 224 * width / 247;
        public static int maxIteration = 100;
        public static double xRange;
        public static double xMin;
        public static double yRange;
        public static double yMin;
        public static WriteableBitmap writeableBitmap;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void myImage_Loaded(object sender, RoutedEventArgs e)
        {
            writeableBitmap = new WriteableBitmap(width, height);
            myImage.Width = width;
            myImage.Height = height;
            myImage.Source = writeableBitmap;

            Reset_Zoom();
            Mandlebrot(writeableBitmap);
        }

        private void Mandlebrot(WriteableBitmap fractalBitmap)
        {
            int[] iterationFrequencies = new int[maxIteration];
            using (Stream bufferStream = fractalBitmap.PixelBuffer.AsStream())
            {
                double y0, x0, a, b, a2, b2;
                int iteration;
                byte[] pixel = { 255, 255, 255, 255 };
                int[,] iterations = new int[width, height];
                bufferStream.Position = 0;

                for (int y = 0; y < height; y++)
                {
                    y0 = y * yRange / height + yMin;
                    for (int x = 0; x < width; x++)
                    {
                        x0 = x * xRange / width + xMin;
                        a = b = a2 = b2 = iteration = 0;

                        while (a2 + b2 <= 4 && iteration < maxIteration)
                        {
                            b = 2 * a * b + y0;
                            a = a2 - b2 + x0;
                            a2 = a * a;
                            b2 = b * b;
                            iteration++;
                        }



                        /*if (iteration < maxIteration)
                        {
                            double log_zn = Math.Log(a2 + b2) / 2;
                            double nu = Math.Log(log_zn / Math.Log(2)) / Math.Log(2);
                            iteration += 1 - nu;
                        }*/

                        iterations[x, y] = iteration - 1;


                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        iterationFrequencies[iterations[x, y]]++;

                    }
                }

                int total = 0;
                for (int i = 0; i < maxIteration; i++)
                {
                    total += iterationFrequencies[i];
                }

                int hue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        hue = 0;
                        for (int i = 0; i < iterations[x, y]; i++)
                        {
                            hue += iterationFrequencies[i];
                        }

                        if (iterations[x, y] == maxIteration - 1)
                        {
                            pixel[0] = pixel[1] = pixel[2] = 0;
                        }
                        else
                        {
                            pixel[0] = pixel[1] = pixel[2] = (byte)(255 * hue / total);
                        }
                        if (hue > 0)
                        {
                            ;
                        }
                        bufferStream.Write(pixel, 0, 4);
                    }
                }
            }
            myImage.Source = fractalBitmap;
            fractalBitmap.Invalidate();
        }

        private void Reset_Zoom_Button(object sender, RoutedEventArgs e)
        {
            Reset_Zoom();
        }
        private void Reset_Zoom()
        {
            xRange = 2.47;
            xMin = -2;
            yRange = 2.24;
            yMin = -1.12;

            Mandlebrot(writeableBitmap);
        }

        private void myImage_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(myImage);
            if (pointer.Properties.IsLeftButtonPressed)
            {
                xRange /= 2;
                yRange /= 2;
                xMin += xRange * pointer.Position.X / width;
                yMin += yRange * pointer.Position.Y / height;
            }
            else if (pointer.Properties.IsRightButtonPressed)
            {
                xMin -= xRange * pointer.Position.X / width;
                yMin -= yRange * pointer.Position.Y / height;
                xRange *= 2;
                yRange *= 2;
            }

            Mandlebrot(writeableBitmap);
        }
    }
}
