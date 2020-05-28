namespace DBListenTest
{
    #region Namespaces
    using System.Diagnostics;
    using Autodesk.Revit.UI;
    using System.IO;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.ApplicationServices;
    using Helpers;
    #endregion
    class App : IExternalApplication
    {
        const string _subscribe = "Subscribe";
        const string _unsubscribe = "Unsubscribe";
        public const string Caption = "OLOLO";

        static ExternalEvent _event = null;

        static string _namespace = typeof(App).Namespace;

        static string _path = typeof(App).Assembly.Location;

        public static App ThisApp = null;
        public static UltimateLogCreator log_worker;

        public static UIControlledApplication _cached_Ui_Ctr_App;

        public static PushButton button1;
        public static PushButton button2;

        internal class Resources
        {
        }

        public Result OnStartup(UIControlledApplication ui_control_app)
        {

            try
            {
                log_worker = new UltimateLogCreator();
                List<string> list_of_log_string = new List<string>();
                _cached_Ui_Ctr_App = ui_control_app;
                ControlledApplication contrl_app = _cached_Ui_Ctr_App.ControlledApplication;

                // Method to add Tab and Panel 
                RibbonPanel panel = RibbonPanel(_cached_Ui_Ctr_App);
                string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
                button1 = panel.AddItem( new PushButtonData("Connect to db", "Connect to db", thisAssemblyPath,
                    "DBListenTest.SubscribeCommand")) as PushButton;
                button1.ToolTip = "Connect to db"; 

                button2 = panel.AddItem(new PushButtonData("Test db", "Test db", thisAssemblyPath,
                    "DBListenTest.TestDbCommand")) as PushButton;
                button2.ToolTip = "Test db";

                //button1.Enabled = true;
                //panel.Enabled = true;
                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                var _logModel = new LoggerView("UpdateParam Exception", exception.ToString());
                _logModel.Show();
                //TaskDialog.Show("Construction model's tools APP run faild", exception.ToString());
                return Result.Failed;
            }
        }


        public static bool Subscribed
        {
            get
            {
                bool rc = button1.ItemText.Equals(_unsubscribe);
                return rc;
            }
        }

        public static bool ToggleSubscription2(IExternalEventHandler handler)
        {
            if (Subscribed)
            {
                LogToFile.Log("Unsubscribing...");

                _event.Dispose();
                _event = null;

                button1.ItemText = _subscribe;

                LogToFile.Log("Unsubscribed.");
            }
            else
            {
                LogToFile.Log("Subscribing...");

                _event = ExternalEvent.Create(handler);

                button1.ItemText = _unsubscribe;

                LogToFile.Log("Subscribed.");
            }
            return null != _event;
        }

        /// <summary>
        /// Provide public read-only access to external event.
        /// </summary>
        public static ExternalEvent Event
        {
            get { return _event; }
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            if (Subscribed)
            {
                _event.Dispose();
                _event = null;
            }
            return Result.Succeeded;
        }


        #region Ribbon Panel

        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "DB Listen"; // Tab name
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch
            {
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "DB Listen");
            }
            catch
            {
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == "DB Listen")
                {
                    ribbonPanel = p;
                }
            }

            //return panel 
            return ribbonPanel;
        }
        #endregion
    }
}