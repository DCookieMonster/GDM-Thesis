using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GDMVisualization
{
    public class EdgePoints
    {
        public double x1 { get; set; }
        public double y1 { get; set; }
        public double x2 { get; set; }
        public double y2 { get; set; }
        public string id { get; set; }

        public EdgePoints()
        {
            this.x1 = 0;
            this.y1 = 0;
            this.x2 = 0;
            this.y2 = 0;
            this.id = "";
        }

        public EdgePoints(double x1, double y1, double x2, double y2, string id)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.id = id;
        }

        public EdgePoints(EdgePoints c)
        {
            this.x1 = c.x1;
            this.y1 = c.y1;
            this.x2 = c.x2;
            this.y2 = c.y2;
            this.id = c.id;
        }

        public EdgePoints(Point c1, Point c2, string id)
        {
            this.x1 = (int)c1.X;
            this.y1 = (int)c1.Y;
            this.x2 = (int)c2.X;
            this.y2 = (int)c2.Y;
            this.id = id;
        }

        //public int GetX1()
        //{
        //    return this.x1;
        //}

        //public int GetX2()
        //{
        //    return this.x2;
        //}

        //public int GetY1()
        //{
        //    return this.y1;
        //}

        //public int GetY2()
        //{
        //    return this.y2;
        //}

        //public void SetX1(int x)
        //{
        //    this.x1 = x;
        //}

        //public void SetX2(int x)
        //{
        //    this.x2 = x;
        //}

        //public void SetY1(int y)
        //{
        //    this.y1 = y;
        //}

        //public void SetY2(int y)
        //{
        //    this.y2 = y;
        //}
    }
}

