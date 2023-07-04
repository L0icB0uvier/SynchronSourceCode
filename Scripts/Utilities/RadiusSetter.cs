namespace Utilities
{
    public static class RadiusSetter
    {
        private const float Ratio = 0.707f;

        public static void SetRadius(float radius, out float radiusX, out float radiusY)
        {
            radiusX = radius;
            radiusY = radius * Ratio;
        }
    }
}