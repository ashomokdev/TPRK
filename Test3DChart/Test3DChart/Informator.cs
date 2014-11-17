using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Test3DChart
{
    static class Informator
    {
        private static Window1 startWindow = GetStartWindow();
        private static string actions = GetActions();
        private static string parameters = GetParameters();
        private static string consequences = GetConsequences();


        private static Window1 GetStartWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(Window1))
                {
                    return window as Window1;
                }
                else
                {
                    throw new Exception("Instance of Window1 not found.");
                }
            }
            return null;
        }

        internal static string GetActions()
        {
            return startWindow.textBoxActions.Text;
        }

        internal static string GetConsequences()
        {
            return startWindow.textBoxConsequences.Text;
        }

        private static string GetParameters()
        {
            return startWindow.textBoxParameters.Text;
        }

        public static List<Point> GetConsequences(string action)
        {
            List<Point> result = new List<Point>();
            List<int> allConsequences = GetListConsequences();
            List<string> allParameters = GetListParameters();
            List<string> allActions = GetListActions();
            List<int> consequences = new List<int>();
            int index = allActions.FindIndex(a => a.Equals(action));

            for (int i = index; i < allConsequences.Count; i = i + allActions.Count)
            {
                consequences.Add(allConsequences[i]);
            }
            List<int> onlyUniqueValues = new List<int>(consequences.Distinct());

            foreach (int consequence in onlyUniqueValues)
            {
                Point point = new Point(action, consequence);
                result.Add(point);
            }
            return result;
        }

        private static List<int> GetListConsequences()
        {
            string consequences = Consequences;
            string[] split = consequences.Split(',');
            List<string> list = new List<string>();
            foreach (string item in split)
            {
                list.Add(item.Replace(" ", string.Empty));
            }
            List<int> result = new List<int>();
            foreach (string item in list)
            {
                result.Add(Int32.Parse(item));
            }
            try
            {
                if ((GetListParameters().Count * GetListActions().Count) > result.Count)
                {
                    throw new Exception("Не вистачає значень наслідків C, наслідки були згенеровані автоматично значенням за замовчуванням (0).");
                }
            }
            catch (Exception)
            {
                while ((GetListParameters().Count * GetListActions().Count) != result.Count)
                {
                    result.Add(0);
                }
            }
            return result;
        }

        public static List<string> GetListActions()
        {
            string actions = Actions;
            string[] split = actions.Split(',');
            List<string> result = new List<string>();
            foreach (string item in split)
            {
                result.Add(item.Replace(" ", string.Empty));
            }
            return result;
        }

        public static List<string> GetListParameters()
        {
            string parameters = Parameters;
            string[] split = parameters.Split(',');
            List<string> result = new List<string>();
            foreach (string item in split)
            {
                result.Add(item.Replace(" ", string.Empty));
            }
            return result;
        }

        public static string Actions
        {
            get { return actions; }
        }

        public static string Consequences
        {
            get { return consequences; }
        }

        public static string Parameters
        {
            get { return parameters; }
        }

        internal static void Update()
        {
            actions = GetActions();
            parameters = GetParameters();
            consequences = GetConsequences();

        }
    }
}
