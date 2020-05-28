namespace DBListenTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Helpers;
    class BimUpdater : IExternalEventHandler
    {
        /// <summary>
        /// Update the BIM with the given database records.
        /// </summary>
        public static bool UpdateBim(Document doc, List<string> doors, ref string error_message)
        //List<DBData> doors,
        {
            try 
            {

                TaskDialog.Show("BD test", "hello world: " + doors.Count);
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();

                //if (doors != null && doors.Count > 0)
                //{
                //    TaskDialog.Show("BD test", "hello world: " + doors.Count);
                //}

                //stopwatch.Stop();
            }
            catch (Exception exception)
            {
                var _logModel = new LoggerView("UpdateParam Exception", exception.ToString());
                _logModel.Show();
                //TaskDialog.Show("Construction model's tools APP run faild", exception.ToString());
                return false;
            }

            return true;
        }

        public void Execute(UIApplication a)
        {
            uint timestamp_before_bim_update
              = Util.UnixTimestamp();

            Document doc = a.ActiveUIDocument.Document;

            string error_message = null;

            bool rc = UpdateBim(doc, DBListener.ModifiedDoors, ref error_message);
        }

        public string GetName()
        {
            return App.Caption + " " + GetType().Name;
        }
    }
}
