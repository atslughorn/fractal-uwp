namespace Fractal
{
    class BurningShip : IFractal
    {
        public double xRange => 3.2;

        public double yRange => 2.2;

        public double xMin => -2;

        public double yMin => -1.75;

        public int calculatePixel(double x0, double y0, int maxIteration)
        {
            double xtemp, zx, zy;
            int iteration = 0;
            zx = x0;
            zy = y0;

            while (zx * zx + zy * zy < 4 && iteration < maxIteration)
            {
                xtemp = zx * zx - zy * zy + x0;
                zy = System.Math.Abs(2 * zx * zy) + y0;
                zx = xtemp;
                iteration++;
            }

            return iteration;
        }
    }
}
