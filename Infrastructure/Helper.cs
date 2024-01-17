using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mini.Infrastructure
{
    public class Helper
    {
        public class Helpers
        {
            public static void RenameFile(string pathFile)
            {
                try
                {
                    string path = pathFile;
                    string sFileExtension = Path.GetExtension(path).ToLower();
                    string dateNow = DateTime.Now.ToString("yyyy-MM-dd-hhmmss");
                    string pathName = Path.ChangeExtension(path, null);
                    string newPath = pathName + "_" + dateNow + sFileExtension;
                    System.IO.File.Move(path, newPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
