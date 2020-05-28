namespace DBListenTest.Helpers
{
    using System.IO;
    class LogToFile
    {
        static string path = @"c:\test\db-test-log.txt";
        public static void Log(string text)
        {
            if (File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                }
            }
        }
        public static void Assert(bool test, string text)
        {
            if (File.Exists(path) && test)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                }
            }
        }
    }
}
