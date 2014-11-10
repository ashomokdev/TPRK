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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf3DChartTutorial;

namespace Test3DChart
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {           
             InitializeComponent();
             textBox1.Text = wPF3DChart1.XValuesInput;
             textBox2.Text = wPF3DChart1.YValuesInput;
             textBox3.Text = wPF3DChart1.ZValuesInput;
             textBox4.Text = wPF3DChart1.ChartTitle;
             textBox5.Text = wPF3DChart1.ZValuesColor;
             //textBox6.Text = wPF3DChart1.XAxisColor;
             //textBox7.Text = wPF3DChart1.YAxisColor;
             slider1.Minimum = 1.0;
             slider1.Maximum = 20.0;
             slider1.Value = wPF3DChart1.MouseSens;

             Binding MouseValueBinding = new Binding("Value");
             MouseValueBinding.Source = slider1;
             MouseValueBinding.Mode = BindingMode.TwoWay;
             wPF3DChart1.SetBinding(WPF3DChart.MouseSensProperty, MouseValueBinding);


             Binding XValueBinding = new Binding("Text");
             XValueBinding.Source = textBox1;
             XValueBinding.Mode = BindingMode.TwoWay;
             wPF3DChart1.SetBinding(WPF3DChart.XValuesInputProperty, XValueBinding);

             Binding YValueBinding = new Binding("Text");
             YValueBinding.Source = textBox2;
             YValueBinding.Mode = BindingMode.TwoWay;
             wPF3DChart1.SetBinding(WPF3DChart.YValuesInputProperty, YValueBinding);

             Binding ZValueBinding = new Binding("Text");
             ZValueBinding.Source = textBox3;
             ZValueBinding.Mode = BindingMode.TwoWay;
             wPF3DChart1.SetBinding(WPF3DChart.ZValuesInputProperty, ZValueBinding);

             Binding ChartTitleBinding = new Binding("Text");
             ChartTitleBinding.Source = textBox4;
             ChartTitleBinding.Mode = BindingMode.TwoWay;
             wPF3DChart1.SetBinding(WPF3DChart.ChartTitleProperty, ChartTitleBinding);

             Binding ZValueColorBinding = new Binding("Text");
             ZValueColorBinding.Source = textBox5;
             ZValueColorBinding.Mode = BindingMode.TwoWay;
             wPF3DChart1.SetBinding(WPF3DChart.ZValuesColorProperty, ZValueColorBinding);
            

            //Binding XAxisColorbinding = new Binding("Text");
            //XAxisColorbinding.Source = textBox6;
            //XAxisColorbinding.Mode = BindingMode.TwoWay;
            //wPF3DChart1.SetBinding(WPF3DChart.XAxisColorProperty, XAxisColorbinding);

            //Binding YAxisColorbinding = new Binding("Text");
            //YAxisColorbinding.Source = textBox7;
            //YAxisColorbinding.Mode = BindingMode.TwoWay;
            //wPF3DChart1.SetBinding(WPF3DChart.YAxisColorProperty, YAxisColorbinding);

            //radioButton2.IsChecked = true;

            //Binding HideChartBinding = new Binding("IsChecked");
            //HideChartBinding.Source = radioButton1;
            //HideChartBinding.Mode = BindingMode.TwoWay;
            //wPF3DChart1.SetBinding(WPF3DChart.HideChartProperty, HideChartBinding);


            Graph2D graph = new Graph2D();
            graph.Show();
        }

        public void Update()
        {
            wPF3DChart1.XValuesInput = textBox1.Text;
            wPF3DChart1.YValuesInput = textBox2.Text;
            wPF3DChart1.ZValuesInput = textBox3.Text;
            wPF3DChart1.ChartTitle = textBox4.Text;
            wPF3DChart1.Update();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Update();
            }
            base.OnKeyUp(e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }
    }
}
