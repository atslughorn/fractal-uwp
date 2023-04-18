namespace FractalAlgorithms
{
    public class Color
    {
        public static int[,] colors = { { 0, 0, 255 }, { 0, 128, 255 }, { 0, 255, 255 }, { 0, 255, 128 }, { 0, 255, 0 }, { 128, 255, 0 }, { 255, 255, 0 }, { 255, 128, 0 }, { 255, 0, 0 }, { 128, 0, 0} };

        public static byte[] CalculateColor(int hue, int total)
        {
            byte[] pixel = { 255, 255, 255, 255 };
            int color = (colors.GetLength(0) - 1) * hue / total;
            float colorInterpolation = (colors.GetLength(0) - 1) * ((float)hue) / total % 1;

            for (int j = 0; j < 3; j++)
            {
                pixel[j] = (byte)(colors[color, j] + colorInterpolation * (colors[color + 1, j] - colors[color, j]));
            }

            return pixel;
        }
    }
}
