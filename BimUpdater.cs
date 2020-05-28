namespace DBListenTest
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.ApplicationServices;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Helpers;
    class BimUpdater : IExternalEventHandler
    {
        /// <summary>
        /// Update the BIM with the given database records.
        /// </summary>
        /// 

        public static bool UpdateBim(Document doc, List<string> doors, ref string error_message)
        //List<DBData> doors,
        {
            try
            {
                FilteredElementCollector f_collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
                IList<Element> elements = f_collector.OfCategory(BuiltInCategory.OST_Furniture).ToElements();

                View activ_view = doc.ActiveView;
                Application app = doc.Application;

                var patternCollector = new FilteredElementCollector(doc.ActiveView.Document);

                patternCollector.OfClass(typeof(Autodesk.Revit.DB.FillPatternElement));
                Autodesk.Revit.DB.FillPatternElement solidFill =
                    patternCollector.ToElements()
                    .Cast<Autodesk.Revit.DB.FillPatternElement>()
                    .First(x => x.GetFillPattern().IsSolidFill);

                UIDocument uidoc = new UIDocument(doc);
                Color color = new Color((byte)150, (byte)200, (byte)200);

                OverrideGraphicSettings grafics = new OverrideGraphicSettings();
                OverrideGraphicSettings grafics_clear = new OverrideGraphicSettings();

                grafics.SetSurfaceForegroundPatternColor(color);
                grafics.SetSurfaceForegroundPatternId(solidFill.Id);

                int db_count = Convert.ToInt32(doors[0]);
                int elem_count = 0;

                //TaskDialog.Show("target", db_count.ToString());

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("db-test");

                    foreach (Element elem in elements)
                    {
                        elem_count += 1;
                        activ_view.SetElementOverrides(elem.Id, grafics_clear);
                        if (elem_count == db_count)
                        {
                            activ_view.SetElementOverrides(elem.Id, grafics);
                        }
                    }

                    uidoc.RefreshActiveView();
                    t.Commit();
                }
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

        public void Execute(UIApplication uiapp)
        {
            try
            {
                uint timestamp_before_bim_update
                  = Util.UnixTimestamp();

                Document doc = uiapp.ActiveUIDocument.Document;

                string error_message = null;

                bool rc = UpdateBim(doc, DBListener.ModifiedDoors, ref error_message);
            }
            catch (Exception exception)
            {
                var _logModel = new LoggerView("UpdateParam Exception", exception.ToString());
                _logModel.Show();
            }
        }

        public string GetName()
        {
            return "bimUpdater";
        }
    }
}
