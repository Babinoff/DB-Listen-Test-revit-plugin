
namespace DBListenTest
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Windows;
    using System.IO;
    using Helpers;
    using TaskDialog = Autodesk.Revit.UI.TaskDialog;

    class DBListener
    {
        static string path = @"c:\test\db-test-log.txt";
        public static bool bootun_togle = false;

        static string _project_id = null;

        /// <summary>
        /// Return the current Revit project id.
        /// </summary>
        public static string ProjectId
        {
            get
            {
                return _project_id;
            }
        }

        /// <summary>
        /// For subscription to automatic BIM updates,
        /// retrieve database records modified after 
        /// this timestamp.
        /// </summary>
        static public uint Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Initialise project id and set the timestamp 
        /// to start polling for database updates.
        /// </summary>
        static public uint Init(string project_id)
        {
            //_project_id = project_id;

            Timestamp = Util.UnixTimestamp();

            Util.InfoMsg(string.Format(
              "Timestamp set to {0}."
              + "\nChanges from now on will be retrieved.",
              Timestamp));

            return Timestamp;
        }

        /// <summary>
        /// Store the modified door records 
        /// retrieved from the database.
        /// </summary>
        static List<string>
          _modified_door_records = null;

        static double last_chek = 999;
        static int db_count = 0;

        /// <summary>
        /// Return the current modified door records 
        /// retrieved from the cloud database.
        /// </summary>
        public static List<string> ModifiedDoors
        {
            get
            {
                return _modified_door_records;
            }
        }

        static int _nLoopCount = 0;

        static int _nCheckCount = 0;

        static int _nUpdatesRequested = 0;

        static int _timeout = 1500;

        #region Windows API DLL Imports
        // DLL imports from user32.dll to set focus to
        // Revit to force it to forward the external event
        // Raise to actually call the external event 
        // Execute.

        /// <summary>
        /// The GetForegroundWindow function returns a 
        /// handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// Move the window associated with the passed 
        /// handle to the front.
        /// </summary>
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(
          IntPtr hWnd);
        #endregion // Windows API DLL Imports

        static string query = "get-index";

        static void CheckForPendingDatabaseChanges()
        {
            //Util.Log("query: " + query); ///НЕЛЬЗЯ ЗАПУСКАТЬ ЛОГ ВНЕ ВАЙЛА ПОЧЕМУТО СОВСЕМ СОВСЕМ НЕЛЬЗЯ // unhandledExceptionFilter is executed in a non-main thread
            //try
            //{
            while (null != App.Event)
                {
                    ++_nLoopCount;

                    if (App.Event.IsPending)
                    {
                        Util.Log(string.Format(
                          "CheckForPendingDatabaseChanges loop {0} - "
                          + "database update event is pending",
                          _nLoopCount));
                    }
                    else
                    {
                        //using (JtTimer pt = new JtTimer(
                        //  "CheckForPendingDatabaseChanges"))
                        //{
                        ++_nCheckCount;

                        Util.Log(string.Format(
                            "CheckForPendingDatabaseChanges loop {0} - "
                            + "check for changes {1}",
                            _nLoopCount, _nCheckCount));

                        _modified_door_records = Util.Get(query);

                        Util.Log("Translate: " + _modified_door_records[0]);

                        db_count = Convert.ToInt32(_modified_door_records[0]);

                        if (null != _modified_door_records && 0 < _modified_door_records.Count && last_chek != db_count)
                        {
                            last_chek = db_count;
                            App.Event.Raise(); //запуск БИМ УПДАТЕРА
                                                //UltimateLogCreator.log_string_list.Add(_modified_door_records.Count.ToString());
                                                //UltimateLogCreator.log_report();
                            ++_nUpdatesRequested;

                            //TaskDialog.Show("BD test", "index change: " + db_count);

                            Util.Log(string.Format(
                                "database update pending event raised {0} times",
                                _nUpdatesRequested));

                            // Set focus to Revit for a moment.
                            // Otherwise, it may take a while before 
                            // Revit reacts to the raised event and
                            // actually calls the event handler Execute 
                            // method.

                            IntPtr hBefore = GetForegroundWindow();

                            SetForegroundWindow(
                                ComponentManager.ApplicationWindow);

                            SetForegroundWindow(hBefore);
                        }
                    }
                }

                Thread.Sleep(_timeout);
                }
            //}
            //catch (Exception exception)
            //{
            //    var _logModel = new LoggerView("UpdateParam Exception", exception.ToString());
            //    _logModel.Show();
            //}
        }


        /// <summary>
        /// Separate thread running the loop
        /// polling for pending database changes.
        /// </summary>
        static Thread _thread = null;

        /// <summary>
        /// Toggle subscription to automatic database 
        /// updates. Forward the call to the external 
        /// application that creates the external event,
        /// store it and launch a separate thread checking 
        /// for database updates. When changes are pending,
        /// invoke the external event Raise method.
        /// </summary>
        public static void ToggleSubscription(
          UIApplication uiapp)
        {
            // Todo: stop thread first!

            if (App.ToggleSubscription2(new BimUpdater()))
            {
                // Start a new thread to regularly check the
                // database status and raise the external event
                // when updates are pending.

                _thread = new Thread(
                  CheckForPendingDatabaseChanges);

                _thread.Start();
            }
            else
            {
                _thread.Abort();
                _thread = null;
            }
        }
    }
}