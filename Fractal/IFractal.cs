namespace Fractal
{
    public interface IFractal
    {
        double xRange
        { get; }
        double yRange
        { get; }
        double xMin
        { get; }
        double yMin
        { get; }

        int calculatePixel(double x0, double y0, int maxIteration);
    }
}
