using System.Drawing;

namespace _2labMisoi
{
    public struct Point
    {
        public int X;
        public int Y;
        public SolidBrush brush;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
            brush = new SolidBrush(Color.White);
        }

        public override bool Equals(object point)
        {
            if (X == ((Point)point).X && Y == ((Point)point).Y)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static bool operator !=(Point x, Point y)
        {
            if (x.X != y.X || x.Y != y.Y)
                return true;
            else
                return false;
        }

        public static bool operator ==(Point x, Point y)
        {
            if (x.X == y.X && x.Y == y.Y)
                return true;
            else
                return false;            
        }

        public override string ToString()
        {
            return "X: " + X.ToString() + ", Y: " + Y.ToString();
        }
    }
}
