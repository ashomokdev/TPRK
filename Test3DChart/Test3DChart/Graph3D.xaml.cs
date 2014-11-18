using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf3DChartTutorial;

namespace Test3DChart
{
    /// <summary>
    /// Interaction logic for Graph3D.xaml
    /// </summary>
    public partial class Graph3D : Window
    {
        private List<List<Point>> _seriesPoints;
        public Graph3D(List<List<Point>> seriesPoints)
        {
            InitializeComponent();
            _seriesPoints = seriesPoints;
            InitializeGraph(_seriesPoints);
        }

        private void InitializeGraph(List<List<Point>> seriesPoints)
        {
            int ΘCount = seriesPoints.Count;
            string actions = GetActions(seriesPoints[0]);
            string consequences = GetConsequences(seriesPoints);

            wPF3DChart2.XValuesInput = actions;
            wPF3DChart2.YValuesInput = consequences;
            wPF3DChart2.ZValuesInput = GetZValuesInput(ΘCount);
            wPF3DChart2.ChartTitle = "Розтягнена параметрична схема ситуації";
            wPF3DChart2.ZValuesColor = GetZValuesColor(ΘCount);

            textBoxActions.Text = wPF3DChart2.XValuesInput;
            textBoxConsequences.Text = wPF3DChart2.YValuesInput;
            textBoxParameters.Text = wPF3DChart2.ZValuesInput;

            textBox4.Text = wPF3DChart2.ChartTitle;
            textBox5.Text = wPF3DChart2.ZValuesColor;

            slider1.Minimum = 1.0;
            slider1.Maximum = 20.0;
            slider1.Value = wPF3DChart2.MouseSens;

            Binding MouseValueBinding = new Binding("Value");
            MouseValueBinding.Source = slider1;
            MouseValueBinding.Mode = BindingMode.TwoWay;
            wPF3DChart2.SetBinding(WPF3DChart.MouseSensProperty, MouseValueBinding);


            Binding XValueBinding = new Binding("Text");
            XValueBinding.Source = textBoxActions;
            XValueBinding.Mode = BindingMode.TwoWay;
            wPF3DChart2.SetBinding(WPF3DChart.XValuesInputProperty, XValueBinding);

            Binding YValueBinding = new Binding("Text");
            YValueBinding.Source = textBoxConsequences;
            YValueBinding.Mode = BindingMode.TwoWay;
            wPF3DChart2.SetBinding(WPF3DChart.YValuesInputProperty, YValueBinding);

            Binding ZValueBinding = new Binding("Text");
            ZValueBinding.Source = textBoxParameters;
            ZValueBinding.Mode = BindingMode.TwoWay;
            wPF3DChart2.SetBinding(WPF3DChart.ZValuesInputProperty, ZValueBinding);

            Binding ChartTitleBinding = new Binding("Text");
            ChartTitleBinding.Source = textBox4;
            ChartTitleBinding.Mode = BindingMode.TwoWay;
            wPF3DChart2.SetBinding(WPF3DChart.ChartTitleProperty, ChartTitleBinding);

            Binding ZValueColorBinding = new Binding("Text");
            ZValueColorBinding.Source = textBox5;
            ZValueColorBinding.Mode = BindingMode.TwoWay;
            wPF3DChart2.SetBinding(WPF3DChart.ZValuesColorProperty, ZValueColorBinding);
        }

        private string GetZValuesColor(int ΘCount)
        {
            Random randomGen = new Random();
            var props = typeof(Colors).GetProperties();
            List<string> knownColors = new List<string>();
            foreach (var prop in props)
            {
                knownColors.Add(prop.Name);
            }

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ΘCount; i++)
            {
                int randomIndex = randomGen.Next(knownColors.Count);
                StringBuilder stringBuilder = new StringBuilder(knownColors[randomIndex]);
                if (i != ΘCount - 1)
                {
                    stringBuilder.Append(", ");
                }
                result.Append(stringBuilder);
            }
            return result.ToString();
        }

        private string GetConsequences(List<List<Point>> seriesPoints)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < seriesPoints.Count;  i++)
            {
                for (int j = 0; j < seriesPoints[i].Count; j++)
                {
                    StringBuilder stringBuilder = new StringBuilder(seriesPoints[i][j].Consequence.ToString());
                    if (i != seriesPoints.Count - 1 || j != seriesPoints[i].Count -1)
                    {
                        stringBuilder.Append(", ");
                    }
                    result.Append(stringBuilder);
                }
            }
            return result.ToString();  
        }

        private string GetActions(List<Point> points)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < points.Count; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(points[i].Action);
                if (i != points.Count - 1)
                {
                    stringBuilder.Append(", ");
                }
                result.Append(stringBuilder);
            }
            return result.ToString();
        }

        private string GetZValuesInput(int ΘCount)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ΘCount; i++)
            {
                StringBuilder stringBuilder = new StringBuilder("Θ").Append(i + 1);
                if (i != ΘCount - 1)
                {
                    stringBuilder.Append(", ");
                }
                result.Append(stringBuilder);
            }
            return result.ToString();
        }
    }
}
