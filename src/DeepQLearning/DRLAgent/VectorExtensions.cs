using System;

namespace DeepQLearning.DRLAgent
{
    public static class VectorExtensions
    {
        public static double DistanceFrom(this Vector @this, Vector other) =>
            Math.Sqrt(Math.Pow(@this.X - other.X, 2) + Math.Pow(@this.Y - other.Y, 2));

        public static double Length(this Vector @this) => Math.Sqrt(Math.Pow(@this.X, 2) + Math.Pow(@this.Y, 2));

        public static Vector Add(this Vector @this, Vector other) => new Vector(@this.X + other.X, @this.Y + other.Y);

        public static Vector Subtract(this Vector @this, Vector other) =>
            new Vector(@this.X - other.X, @this.Y - other.Y);

        public static Vector Rotate(this Vector @this, double angle) => new Vector(
            @this.X * Math.Cos(angle) + @this.Y * Math.Sin(angle),
            -@this.X * Math.Sin(angle) + @this.Y * Math.Cos(angle));

        // in place operations
        public static Vector Scale(this Vector @this, double scalar) => new Vector(@this.X * scalar, @this.Y * scalar);

        public static Vector Normalize(this Vector @this)
        {
            var d = @this.Length();
            return @this.Scale(1.0 / d);
        }
    }
}