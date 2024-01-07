namespace Codes.Utils
{
    public static class MathUtils
    {
        public static int Mod(int x, int y)
        {
            return x - y * (int)System.Math.Floor((double) x / y);
        }
    }
}