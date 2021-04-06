using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetScan.TetScan.Utils
{
    class FileIO
    {
        public static string read(string path)
        {
            string ret = "";
            try
            {
                StreamReader sr = new StreamReader(path);
                
                string line = sr.ReadLine();
                ret += line;

                while (line != null)
                {
                    line = sr.ReadLine();
                    ret += line;
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return ret;
        }
        
        public static void write(string content, string path)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(content);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
