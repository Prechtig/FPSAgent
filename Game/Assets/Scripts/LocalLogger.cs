using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class LocalLogger
    {
        private static string _path;

        public static void Initialize(string path)
        {
            _path = path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            _path += "log.txt";
        }

        public static void Write(string s)
        {
            if (s.Length != 0)
            {
                using (FileStream fileStream = new FileStream(_path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        sw.WriteLine(s);
                    }
                }
            }
            else
            {
                throw new Exception("Logger not Initialized. File path not set");
            }
        }
    }
}
