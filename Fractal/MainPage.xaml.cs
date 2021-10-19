using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fractal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void myImage_Loaded(object sender, RoutedEventArgs e)
        {
            int width = 1000;
            int height = 224 * width / 247;
            WriteableBitmap source = new WriteableBitmap(width, height);

            using (Stream bufferStream = source.PixelBuffer.AsStream())
            {
                double y0, x0, a, b, a2, b2;
                double iteration;
                int maxIteration = 1000;
                byte[] pixel = { 255, 255, 255, 255 };

                for (int y = 0; y < height; y++)
                {
                    y0 = y * 2.24 / height - 1.12;
                    for (int x = 0; x < width; x++)
                    {
                        x0 = x * 2.47 / width - 2;
                        a = b = a2 = b2 = iteration = 0;

                        while (a2 + b2 <= 4 && iteration < maxIteration)
                        {
                            b = 2 * a * b + y0;
                            a = a2 - b2 + x0;
                            a2 = a * a;
                            b2 = b * b;
                            iteration++;
                        }



                        if (iteration < 50)
                        {
                            double log_zn = Math.Log(a2 + b2) / 2;
                            double nu = Math.Log(log_zn / Math.Log(2)) / Math.Log(2);
                            iteration += 1 - nu;

                            iteration *= maxIteration / 50;
                        }
                        else
                        {
                            iteration = maxIteration;
                        }

                        pixel[0] = pixel[1] = pixel[2] = (byte)(255 * (maxIteration - iteration) / maxIteration);
                        bufferStream.Write(pixel, 0, 4);
                    }
                }
            }

            myImage.Width = width;
            myImage.Height = height;
            myImage.Source = source;
        }

        private void myImage2_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
