namespace DBListenTest
{
    #region Namespaces
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using DBListenTest.Helpers;
    #endregion // Namespaces
    public class Util
    {
    #region HTTP Access
    public static bool UseLocalServer = true;

    // HTTP access constants.

    const string _base_url_local = @"http://localhost:3001";
        //const string _base_url_global = "http://fireratingdb.herokuapp.com";
        //const string _api_version = "api/v1";

        /// <summary>
        /// Return REST API base URL.
        /// </summary>
        //public static string RestApiBaseUrl
        //{
        //  get
        //  {
        //    return UseLocalServer
        //      ? _base_url_local
        //      : _base_url_global;
        //  }
        //}

        /// <summary>
        /// Return REST API URI.
        /// </summary>
        //public static string RestApiUri
        //{
        //  get
        //  {
        //    return RestApiBaseUrl + "/" + _api_version;
        //  }
        //}


        /// <summary>
        /// PUT JSON document data into 
        /// the specified mongoDB collection.
        /// </summary>
        public static HttpStatusCode Put(
          out string content,
          out string errorMessage,
          string collection_name_and_id,
          DBData doorData)
        {
            var client = new RestClient(_base_url_local);

            var request = new RestRequest(_base_url_local + "/"
              + collection_name_and_id, Method.PUT);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(doorData); // uses JsonSerializer

            IRestResponse response = client.Execute(request);

            content = response.Content; // raw content as string
            errorMessage = response.ErrorMessage;

            return response.StatusCode;
        }

        /// <summary>
        /// GET JSON document data from 
        /// the specified mongoDB collection.
        /// </summary>
        static List<string> response_list;

        public static List<string> Get(string comand)
        {
            try
            {
                response_list = new List<string>();
                var client = new RestClient();
                client.BaseUrl = new System.Uri(_base_url_local);
                //List<string> response_list = new List<string>();

                var request = new RestRequest();
                request.Resource = comand;
                request.Method = Method.GET;

                IRestResponse<List<TestData>> response = client.Execute<List<TestData>>(request);

                foreach (TestData data in response.Data)
                {
                    response_list.Add(data.index);
                }

                //response_list.Add(response.ToString());

                return response_list;
            }
            catch (Exception exception)
            {
                UltimateLogCreator.log_string_list.Add(exception.ToString());
                return response_list;
            }
        }

        /// <summary>
        /// Delete all door data for this project
        /// from the specified mongoDB collection.
        /// </summary>
        //public static string Delete(
        //  string collection_name_and_id )
        //{
        //  var client = new RestClient( RestApiBaseUrl );

        //  var request = new RestRequest( _api_version + "/"
        //    + collection_name_and_id, Method.DELETE );

        //  IRestResponse response = client.Execute( request );

        //  return response.Content;
        //}

        /// <summary>
        /// Batch PUT JSON document data into 
        /// the specified mongoDB collection.
        /// </summary>
        //public static HttpStatusCode PostBatch(
        //  out string content,
        //  out string errorMessage,
        //  string collection_name,
        //  List<DBData> doorData )
        //{
        //  var client = new RestClient( RestApiBaseUrl );

        //        string request_string = _api_version + "/" + collection_name;
        //        LogToFile.Log(request_string);

        //  var request = new RestRequest(request_string, Method.POST );

        //  request.RequestFormat = DataFormat.Json;

        //  request.AddBody( doorData ); // uses JsonSerializer

        //  IRestResponse response = client.Execute( request );

        //  content = response.Content; // raw content as string
        //  errorMessage = response.ErrorMessage;

        //  return response.StatusCode;
        //}

        /// <summary>
        /// Convert a string to a byte array.
        /// </summary>
        public static byte[] GetBytes( string str )
    {
      byte[] bytes = new byte[str.Length
        * sizeof( char )];

      System.Buffer.BlockCopy( str.ToCharArray(),
        0, bytes, 0, bytes.Length );

      return bytes;
    }

    /// <summary>
    /// Convert a byte array to a string.
    /// </summary>
    static string GetString( byte[] bytes )
    {
      char[] chars = new char[bytes.Length / sizeof( char )];
      System.Buffer.BlockCopy( bytes, 0, chars, 0, bytes.Length );
      return new string( chars );
    }
    #endregion // HTTP Access

    #region Timestamp
    static readonly DateTime _1970_01_01
      = new DateTime( 1970, 1, 1 );

    /// <summary>
    /// Converts a given DateTime into a Unix 
    /// timestamp, i.e., number of seconds since 
    /// 1970-01-01.
    /// </summary>
    public static uint ToUnixTimestamp( DateTime a )
    {
      return (uint) Math.Truncate(
        a.ToUniversalTime()
          .Subtract( _1970_01_01 )
          .TotalSeconds );
    }

    /// <summary>
    /// Gets a Unix timestamp representing 
    /// the current moment, i.e., the number 
    /// of seconds since 1970-01-01.
    /// </summary>
    public static uint UnixTimestamp()
    {
      return (uint) Math.Truncate(
        DateTime.UtcNow.Subtract( _1970_01_01 )
          .TotalSeconds );
    }
        #endregion // Timestamp

        #region Messages
        /// <summary>
        /// Return an English plural suffix 's' or
        /// nothing for the given number of items.
        /// </summary>
        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        /// <summary>
        /// Display a short big message.
        /// </summary>
        public static void InfoMsg(string msg)
        {
            Util.Log(msg);
            TaskDialog.Show(App.Caption, msg);
        }

        /// <summary>
        /// Display a longer message in smaller font.
        /// </summary>
        public static void InfoMsg2(
          string instruction,
          string msg,
          bool prompt = true)
        {
            Util.Log(string.Format("{0}: {1}",
              instruction, msg));

            if (prompt)
            {
                TaskDialog dlg = new TaskDialog(App.Caption);
                dlg.MainInstruction = instruction;
                dlg.MainContent = msg;
                dlg.Show();
            }
        }

        /// <summary>
        /// Display an error message.
        /// </summary>
        public static void ErrorMsg(string msg)
        {
            Util.Log(msg);
            TaskDialog dlg = new TaskDialog(App.Caption);
            dlg.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            dlg.MainInstruction = msg;
            dlg.Show();
        }

        /// <summary>
        /// Print a debug log message with a time stamp
        /// to the Visual Studio debug output window.
        /// </summary>
        /// 
        // Creates the text file that the trace listener will write to.

        public static void Log(string msg)
        {
            string timestamp = DateTime.Now.ToString(
              "HH:mm:ss.fff");

            //Debug.Print(timestamp + " " + msg);
            LogToFile.Log(timestamp + " " + msg);
        }
        #endregion // Messages

        #region Project Identifier
        /// <summary>
        /// Define a project identifier for the 
        /// given Revit document.
        /// </summary>
        public static string GetProjectIdentifier(
          Document doc)
        {
            SHA256 hasher = SHA256Managed.Create();

            string key = System.Environment.MachineName
              + ":" + doc.PathName;

            byte[] hashValue = hasher.ComputeHash(GetBytes(
              key));

            string hashb64 = Convert.ToBase64String(
              hashValue);

            return hashb64.Replace('/', '_');
        }
        #endregion // Project Identifier

        #region Shared Parameter Definition
        // Shared parameter definition constants.

        public const string SharedParameterGroupName = "API Parameters";
        public const string SharedParameterName = "API FireRating";
        public const string SharedParameterFilePath = "C:/tmp/SharedParams.txt";

        /// <summary>
        /// Get shared parameters file.
        /// </summary>
        public static DefinitionFile GetSharedParamsFile(
          Application app)
        {
            string sharedParamsFileName
              = app.SharedParametersFilename;

            if (0 == sharedParamsFileName.Length
              || null == app.OpenSharedParameterFile())
            {
                StreamWriter stream;
                stream = new StreamWriter(SharedParameterFilePath);
                stream.Close();

                app.SharedParametersFilename = SharedParameterFilePath;
                sharedParamsFileName = app.SharedParametersFilename;
            }

            // Get the current file object and return it.

            DefinitionFile sharedParametersFile
              = app.OpenSharedParameterFile();

            return sharedParametersFile;
        }

        /// <summary>
        /// Return all element instances for a given 
        /// category, identified either by a built-in 
        /// category or by a category name.
        /// </summary>
        public static FilteredElementCollector GetTargetInstances(
          Document doc,
          object targetCategory)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc);

            bool isName = targetCategory.GetType().Equals(
              typeof(string));

            if (isName)
            {
                Category cat = doc.Settings.Categories
                  .get_Item(targetCategory as string);

                collector.OfCategoryId(cat.Id);
            }
            else
            {
                collector.WhereElementIsNotElementType();

                collector.OfCategory((BuiltInCategory)targetCategory);

                //var model_elements
                //  = from e in collector
                //    where ( null != e.Category && e.Category.HasMaterialQuantities )
                //    select e;

                //elements = model_elements.ToList<Element>();
            }
            return collector;
        }

        /// <summary>
        /// Return GUID for a given shared parameter group and name.
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <param name="defGroup">Definition group name</param>
        /// <param name="defName">Definition name</param>
        /// <returns>GUID</returns>
        public static Guid SharedParamGuid(
          Application app,
          string defGroup,
          string defName)
        {
            DefinitionFile file = app.OpenSharedParameterFile();
            DefinitionGroup group = (null == file)
              ? null : file.Groups.get_Item(defGroup);
            Definition definition = (null == group)
              ? null : group.Definitions.get_Item(defName);
            ExternalDefinition externalDefinition
              = (null == definition)
                ? null : definition as ExternalDefinition;
            return (null == externalDefinition)
              ? Guid.Empty
              : externalDefinition.GUID;
        }

        public static bool GetSharedParamGuid(
          Application app,
          out Guid paramGuid)
        {
            paramGuid = Util.SharedParamGuid(app,
              Util.SharedParameterGroupName,
              Util.SharedParameterName);

            return !paramGuid.Equals(Guid.Empty);
        }
        #endregion // Shared Parameter Definition
    }
    [Serializable]
    public class TestData
    {
        public string title { get; set; }
        public string index { get; set; }
    }
}
