using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace Test3DChart
{
    static class AddingLinesController
    {
        public static int LinesCount = GetLinesCount();

        public static int GetLinesCount()
        {
            string consequences = Informator.Consequences;
            string[] split = consequences.Split(',');
            List<string> list = new List<string>();
            foreach (string item in split)
            {
               list.Add(item.Replace(" ", string.Empty));
            }
            List<string> onlyUniqueValues = new List<string>(list.Distinct());
            return onlyUniqueValues.Count;       
        }
    }
}
