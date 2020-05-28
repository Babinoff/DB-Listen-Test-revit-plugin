namespace DBListenTest
{
    using System;
    //using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System.Collections.Generic;
    using Helpers;
    //using Autodesk.Revit.UI.Selection;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class TestDbCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                //UIApplication uiapp = commandData.Application;

                string query = "get-index";
                List<string> result = Util.Get(query);

                //TaskDialog.Show("test db counter:", result.Count.ToString());

                string error_message = null;
                bool rc = BimUpdater.UpdateBim(doc, result, ref error_message);


                //UltimateLogCreator.log_report_by_string_list("response" , result);

                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                UltimateLogCreator.log_string_list.Add(exception.ToString());
                return Result.Failed;
            }
        }

    }
}
