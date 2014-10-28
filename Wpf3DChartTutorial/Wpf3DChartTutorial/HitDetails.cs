using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Wpf3DChartTutorial
{
    internal class HitDetails
    {
        private Point3D p0, p1, p2, p3, pointToWrite;
        private Color barColor;
        Point3D pointToStart;
        double barHeight, barWidth;
        private string xItem, yItem, zItem;
        bool showValue;
        StringBuilder strToDisplay;

        public HitDetails(string XItemParam, string YItemParam, string ZItemParam, Point3D P0, Point3D P1, Point3D P2, Point3D P3, Point3D PointToWrite, Point3D PointToStartParam, double Width, double Height, Color BarColorParam)
        {
            xItem = XItemParam;
            yItem = YItemParam;
            zItem = ZItemParam;
            p0 = P0;
            p1 = P1;
            p2 = P2;
            p3 = P3;
            strToDisplay = new StringBuilder();
            pointToWrite = PointToWrite;
            barColor = BarColorParam;

            pointToStart = new Point3D(PointToStartParam.X, PointToStartParam.Y, PointToStartParam.Z);
            pointToStart.X -= 0.25;
            pointToStart.Z += 0.25;
            barHeight = Height + 0.5;
            barWidth = Width + 0.5;
            Update();
        }

        public void Update()
        {
            strToDisplay.Length = 0;
            strToDisplay.AppendFormat("X={0}; Y={1}; Z={2}", xItem, yItem, zItem);
        }

        public Point3D PointToWrite
        {
            get
            {
                return pointToWrite;
            }
        }

        public string StringToDisplay
        {
            get
            {
                return strToDisplay.ToString();
            }
        }

        public bool ShowValue
        {
            get
            {
                return showValue;
            }

            set
            {
                showValue = value;
            }
        }

        public string XItem
        {
            get
            {
                return xItem;

            }
        }

        public string YItem
        {
            get
            {
                return yItem;
            }
        }

        public string ZItem
        {
            get
            {
                return zItem;
            }
        }

        public Point3D P0
        {
            get
            {
                return p0;
            }
        }

        public Point3D P1
        {
            get
            {
                return p1;
            }
        }

        public Point3D P2
        {
            get
            {
                return p2;
            }
        }

        public Point3D P3
        {
            get
            {
                return p3;
            }
        }

        public Point3D PointToStart
        {
            get
            {
                return pointToStart;
            }
        }

        public double Height
        {
            get
            {
                return barHeight;
            }
        }

        public double Width
        {
            get
            {
                return barWidth;
            }
        }

        public Color BarColor
        {
            get
            {
                return barColor;
            }
        }

    
    }
}
