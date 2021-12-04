namespace Fractal
{
    class Mandlebrot : IFractal
    {
        public double xRange => 2.47;

        public double yRange => 2.24;

        public double xMin => -2;

        public double yMin => -1.12;

        public int calculatePixel(double x0, double y0, int maxIteration)
        {
            double a, b, a2, b2;
            int iteration = 0;
            a = b = a2 = b2 = iteration = 0;

            while (a2 + b2 <= 4 && iteration < maxIteration + 1)
            {
                b = 2 * a * b + y0;
                a = a2 - b2 + x0;
                a2 = a * a;
                b2 = b * b;
                iteration++;
            }

            return iteration - 1;
        }
    }
}
