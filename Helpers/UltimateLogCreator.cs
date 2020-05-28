
namespace DBListenTest.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;

    public class UltimateLogCreator
    {
        public static List<string> cats_log_info = new List<string>();
        public static List<string> log_string_list = new List<string>();

        private double solid_volume_sum(List<Solid> all_solids)
        {
            var volume_sum = new List<double>();
            all_solids.ForEach(solid => volume_sum.Add(solid.Volume));
            return volume_sum.Sum();
        }

        static public void log_report()
        {
            if (log_string_list.Count > 0)
            {
                var _logModel = new LoggerView("log_report", "List counter: " + log_string_list.Count.ToString() + Environment.NewLine + String.Join(Environment.NewLine, log_string_list.ToArray()));
                _logModel.Show();
            }
        }

        public static void log_report_by_string_list(string log_title, List<string> string_list)
        {
            if (string_list.Count > 0)
            {
                var _logModel = new LoggerView(log_title, "List counter: " + string_list.Count.ToString() + Environment.NewLine + String.Join(Environment.NewLine, string_list.ToArray()));
                _logModel.Show();
            }
        }
    }
}
