
using System.Numerics;

namespace BoalsProject
{
    internal class MyTime
    {
        internal static double DeltaTime = 1;

        private DateTime _previousUpdateTime = DateTime.Now;

        public MyTime()
        {
            GlobalActions.UpdateAction += () =>
            {
                DeltaTime = (DateTime.Now - _previousUpdateTime).TotalSeconds;
                _previousUpdateTime = DateTime.Now;
            };
        }
    }

    public static class MyTimeUtils
    {
        public static Vector2 Multiply(this Vector2 v, double m)
        {
            return new Vector2((float)(v.X * m), (float)(v.Y * m));
        }
    }
}
