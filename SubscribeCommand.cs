namespace DBListenTest
{
    using System;
    //using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    //using Autodesk.Revit.UI.Selection;

    using Helpers;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SubscribeCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, 
            ref string message,
            ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                UIApplication uiapp = commandData.Application;

                string project_id = "test-id";

                if (!App.Subscribed /*&& 0 == DbAccessor.Timestamp*/ )
                {
                    DBListener.Init(project_id);
                }

                DBListener.ToggleSubscription(uiapp);

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
