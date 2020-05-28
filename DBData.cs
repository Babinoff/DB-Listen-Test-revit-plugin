namespace DBListenTest
{
    using Autodesk.Revit.DB;
    using System;
    public class DBData
    {
        public string _id { get; set; }
        public string project_id { get; set; }
        public string level { get; set; }
        public string tag { get; set; }
        public double firerating { get; set; }
        public uint modified { get; set; }

        public const BuiltInParameter BipMark = BuiltInParameter.ALL_MODEL_MARK;


        /// <summary>
        /// Constructor to populate instance by 
        /// deserialising the REST GET response.
        /// </summary>
        public DBData(
              Element door,
              string project_id_arg,
              Guid paramGuid,
              uint timestamp)
        {
            Document doc = door.Document;

            _id = door.UniqueId;

            project_id = project_id_arg;

            level = doc.GetElement(door.LevelId).Name;

            tag = door.get_Parameter(BipMark).AsString();

            firerating = door.get_Parameter(paramGuid)
              .AsDouble();

            modified = timestamp;
        }
    }
}
