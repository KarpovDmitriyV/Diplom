using System.Collections.Generic;
using System.Drawing;

namespace _2labMisoi
{
    public class Region
    {
        public int Area;
        public List<Point> pixels = new List<Point>();
        public int Perimeter;
        public double Сompactness;
        public Color _color;
        public int index;
        public List<Point> perimeterCoordinates = new List<Point>();


        public Region(Color color)
        {
            _color = color;
        }
    }
}
