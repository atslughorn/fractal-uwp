using FractalAlgorithms;
using System.Collections.Generic;
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
        public IEnumerable<byte[]> pixels;
        public IFractal currentFractal;
        public Dictionary<string, IFractal> fractalSets = new Dictionary<string, IFractal>();
        public bool boxEmptyAndInputCancelled = false;//Used to fix a bug that occurs when the input box is empty and the input gets cancelled and the next input doesn't get cancelled

        public MainPage()
        {
            this.InitializeComponent();
            fractalSets.Add("Mandlebrot", new Mandlebrot());
            fractalSets.Add("Burning Ship", new BurningShip());
        }

        private void myImage_Loaded(object sender, RoutedEventArgs e)
        {
            writeableBitmap = new WriteableBitmap(width, height);
            myImage.Width = width;
            myImage.Height = height;
            myImage.Source = writeableBitmap;

            //Reset_Zoom();
        }

        private void DrawFractal(WriteableBitmap fractalBitmap)
        {
            using (Stream bufferStream = fractalBitmap.PixelBuffer.AsStream())
            {
                pixels = SelectedFractal();
                //bufferStream.Position = 0;

                foreach (byte[] pixel in pixels)
                {
                    bufferStream.Write(pixel, 0, 4);
                }
            }
            myImage.Source = fractalBitmap;
            fractalBitmap.Invalidate();
        }

        private IEnumerable<byte[]> SelectedFractal()
        {
            switch (fractalBox.SelectedItem)
            {
                default:
                    return GenerateFractal();
            }
        }

        private int BurningShip(int x0, int y0)
        {
            int xtemp;
            int iteration = 0;
            int zx = x0;
            int zy = y0;

            while (zx * zx + zy * zy < 4 && iteration < maxIteration)
            {
                xtemp = zx * zx - zy * zy + x0;
                zy = System.Math.Abs(2 * zx * zy) + y0;
                zx = xtemp;
                iteration++;
            }

            return iteration;
        }

        private IEnumerable<byte[]> GenerateFractal()
        {
            int[] iterationFrequencies = new int[maxIteration + 1];
            double y0, x0;
            int iteration;
            byte[] pixel = { 255, 255, 255, 255 };
            int[,] iterations = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                y0 = y * yRange / height + yMin;
                for (int x = 0; x < width; x++)
                {
                    x0 = x * xRange / width + xMin;

                    iteration = currentFractal.calculatePixel(x0, y0, maxIteration);

                    iterations[x, y] = iteration;


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

                    if (iterations[x, y] == maxIteration)
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
            xRange = currentFractal.xRange;
            xMin = currentFractal.xMin;
            yRange = currentFractal.yRange;
            yMin = currentFractal.yMin;
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
            fractalSets.TryGetValue((string) e.AddedItems[0], out currentFractal);
            Reset_Zoom();
        }

        private void fractalBox_Loaded(object sender, RoutedEventArgs e)
        {
            fractalBox.ItemsSource = fractalSets.Keys;
            currentFractal = fractalSets.Values.GetEnumerator().Current;
            fractalBox.SelectedIndex = 0;
        }
    }
}
