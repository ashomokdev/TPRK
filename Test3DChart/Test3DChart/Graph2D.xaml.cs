using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace Test3DChart
{
    /// <summary>
    /// Interaction logic for Graph2D.xaml
    /// </summary>
    public partial class Graph2D : Window
    {
        private class ArrayIndex
        {
            internal Point[] Points;
            internal int Index;
            public ArrayIndex(Point[] points, int index)
            {
                Points = points;
                Index = index;
            }
        }

        public Graph2D()
        {
            try
            {
                InitializeComponent();
                AddingLines();
                Graph3D graph = new Graph3D();
                graph.Show();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + e.Source + e.InnerException);
                throw;
            }
        }

        public void AddingLines()
        {
            int parametersCount = Informator.GetListParameters().Count;
            List<string> actions = Informator.GetListActions();
            Point[][] array2D = new Point[actions.Count][];
            int seriesCount = 1;
            for (int i = 0; i < actions.Count; i++)
            {
                //fill 2D array
                array2D[i] = Informator.GetConsequences(actions[i]).ToArray();
                Array.Sort(array2D[i], delegate(Point a, Point b)
                {
                    return a.Consequence.CompareTo(b.Consequence);
                });
                seriesCount = seriesCount * array2D[i].Length;
            }

            List<ArrayIndex> list = new List<ArrayIndex>();
            for (int i = 0; i < array2D.Length; i++)
            {
                ArrayIndex arrayIndex = new ArrayIndex(array2D[i], 0);
                list.Add(arrayIndex);
            }

            while (seriesCount > 0)
            {
                List<Test3DChart.Point> points = new List<Point>();
                for (int i = 0; i < actions.Count; i++)
                {
                    Point point = list[i].Points[list[i].Index];
                    points.Add(point);
                }

                bool changed = false;
                int tupleNumb = list.Count - 1;
                while (!changed)
                {
                    int index = list[tupleNumb].Index;
                    if (index + 1 < list[tupleNumb].Points.Length)
                    {
                        list[tupleNumb].Index = index + 1;
                        changed = true;
                    }
                    else
                    {
                        list[tupleNumb].Index = 0;
                        tupleNumb = tupleNumb - 1;
                        if (tupleNumb < 0)
                        {
                            changed = true;
                        }
                    }
                }
                LineSeries lineSeries1 = new LineSeries();
                lineSeries1.Title = GetTitle(points);
                lineSeries1.DependentValuePath = "Consequence";
                lineSeries1.IndependentValuePath = "Action";
                lineSeries1.ItemsSource = points;
                Chart2D.Series.Add(lineSeries1);
                seriesCount--;
            }
        }

        private string GetTitle(List<Point> points)
        {
            string result = string.Empty;
            for (int i = 0; i < points.Count; i++)
            {
                result += "(X" + (i + 1) + ", " + "C" + (points[i].Consequence + 1) + ")";
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Chart2D.Series.Clear();
            Informator.Update();
            AddingLines();
        }
    }
}
