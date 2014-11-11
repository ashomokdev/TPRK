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

namespace Test3DChart
{
    /// <summary>
    /// Interaction logic for Graph2D.xaml
    /// </summary>
    public partial class Graph2D : Window
    {
        public Graph2D()
        {
            InitializeComponent();
            AddingLines();
        }

        public void AddingLines()
        {
            List<Test3DChart.Point> sourse = new List<Test3DChart.Point>
            {
                new Test3DChart.Point {Action = "x1", Consequence = 1}
            };
            LineSeries firstSeries = Chart2D.Series[0] as LineSeries;
            firstSeries.ItemsSource = sourse;


            List<Test3DChart.Point> sourse2 = new List<Test3DChart.Point>
            {
                new Test3DChart.Point {Action = "x2", Consequence = 2}
            };
            LineSeries testSeries = new LineSeries();
            Chart2D.Series.Add(testSeries);
            LineSeries secondSeries = Chart2D.Series[1] as LineSeries;
            secondSeries.ItemsSource = sourse2;

            //Chart2D.Series.Add(testSeries);
            //DataCollection coll = new DataCollection();
            //testSeries.ItemsSource = coll;

            

        
        }
    }
}
