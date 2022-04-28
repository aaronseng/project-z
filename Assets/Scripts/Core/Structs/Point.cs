using System;

namespace ProjectZ.Core
{
    //Point struct
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        ///   <para>X component of the point.</para>
        /// </summary>
        public int x;

        /// <summary>
        ///   <para>Y component of the point.</para>
        /// </summary>
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object other) => other is Point other1 && this.Equals(other1);

        public bool Equals(Point other) => this.x == other.x && this.y == other.y;

        public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode() << 2;

        public static bool operator ==(Point lhs, Point rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            return num1 + num2 == 0;
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return !(lhs == rhs);
        }

        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);

        public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);

        public static Point operator *(Point a, int d) => new Point(a.x * d, a.y * d);

        public override string ToString()
        {
            return $"Point [x: {x} , y: {y}]";
        }
    }
}