using FractalAlgorithms;
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
        public static int maxIteration = 500;
        public static double xRange;
        public static double xMin;
        public static double yRange;
        public static double yMin;
        public static WriteableBitmap writeableBitmap;
        public string[] fractalSets = { "Mandlebrot", "Julia" };
        public bool boxEmptyAndInputCancelled = false;//Used to fix a bug that occurs when the input box is empty and the input gets cancelled and the next input doesn't get cancelled

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
            DrawFractal(writeableBitmap);
        }

        private void DrawFractal(WriteableBitmap fractalBitmap)
        {
            using (Stream bufferStream = fractalBitmap.PixelBuffer.AsStream())
            {
                //bufferStream.Position = 0;
                foreach (byte[] pixel in Mandlebrot())
                {
                    bufferStream.Write(pixel, 0, 4);
                }
            }
            myImage.Source = fractalBitmap;
            fractalBitmap.Invalidate();
        }

        /*private System.Collections.Generic.IEnumerable<byte[]> Julia()
        {

        }*/

        private System.Collections.Generic.IEnumerable<byte[]> Mandlebrot()
        {
            int[] iterationFrequencies = new int[maxIteration];
            double y0, x0, a, b, a2, b2;
            int iteration;
            byte[] pixel = { 255, 255, 255, 255 };
            int[,] iterations = new int[width, height];

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
                        pixel = Color.CalculateColor(hue, total);
                    }
                    yield return pixel;
                }
            }
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

            DrawFractal(writeableBitmap);
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

            DrawFractal(writeableBitmap);
        }

        private void iterationsBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            foreach (char c in args.NewText)
            {
                if (c < '0' || c > '9')
                {
                    args.Cancel = true;
                    if (iterationsBox.Text.Length == 0)
                    {
                        boxEmptyAndInputCancelled = true;
                    }
                    return;
                }
            }
        }

        private void iterationsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (boxEmptyAndInputCancelled)
            {
                iterationsBox.SelectionStart = 1;
                boxEmptyAndInputCancelled = false;
            }
            if (int.TryParse(iterationsBox.Text, out int number))
            {
                maxIteration = number;
            }
        }

        private void iterationsBox_LostFocus(object sender, RoutedEventArgs e)
        {
            iterationsBox.Text = maxIteration.ToString();
            DrawFractal(writeableBitmap);
        }

        private void fractalBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reset_Zoom();
        }

        private void fractalBox_Loaded(object sender, RoutedEventArgs e)
        {
            fractalBox.ItemsSource = fractalSets;
            fractalBox.SelectedIndex = 0;
        }
    }
}
