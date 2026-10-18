using System;

namespace Po_Griby.MapSystem
{
    public readonly struct Vector2D
    {
        public int x { get; }
        public int y { get; }

        public Vector2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
            => new(a.x + b.x, a.y + b.y);

        public static Vector2D operator -(Vector2D a, Vector2D b)
            => new(a.x - b.x, a.y - b.y);

        public int Length => (int)Math.Sqrt(x * x + y * y);
    }
}
