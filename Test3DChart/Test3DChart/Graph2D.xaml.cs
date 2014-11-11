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
           // AddingLines();
        }

        public void AddingLines()
        {
         
            LineSeries testSeries = new LineSeries();
            DataCollection coll = new DataCollection();


            testSeries.DataContext = coll;
            Chart2D.Series.Add(testSeries);
        }
    }
}
