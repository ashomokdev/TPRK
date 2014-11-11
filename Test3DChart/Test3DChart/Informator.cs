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

        private static string GetActions()
        {
            return startWindow.textBoxActions.Text;
        }

        private static string GetConsequences()
        {
            return startWindow.textBoxConsequences.Text;
        }

        public static string Actions
        {
            get { return actions; }
        }

        public static string Consequences
        {
            get { return consequences; }
        }

    }
}
