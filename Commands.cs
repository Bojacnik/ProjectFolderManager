using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ProjectFolderManager
{
    public static class Commands
    {
        private static readonly string[] commands = { "end", "del", "cat", "repeat", "help" };

        public static void Delete(string command, Dictionary<int, string>  projectFolders)
        {
            try
            {
                int key = Convert.ToInt32(command.Substring(3));
                _deleteDirectory(projectFolders, key, projectFolders[key]);
            }
            catch
            {
                throw new Exception("Failed to delete your folder");
            }
        }
        public static void Cat(string command, Dictionary<int, string> projectFolders)
        {
            string path = projectFolders[Convert.ToInt32(command.Substring(3))];
            string file = _find(path);
            FileStream fs = new FileStream(path + "\\" + file, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }
        public static void Repeat(Dictionary<int, string> projectFolders, params string[] tempPaths)
        {
            ConsoleColor defaultClsColor = Console.BackgroundColor;
            foreach ((int o, string s) in projectFolders)
            {
                Console.BackgroundColor = defaultClsColor;
                if (_IsSubstring(tempPaths, s))
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                Console.Write(o + "\t\t" + s);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            Console.BackgroundColor = defaultClsColor;
            Console.WriteLine();
        }
        public static void Help()
        {
            foreach (string cmd in commands)
            {
                Console.WriteLine("\t" + cmd);
            }
        }


        private static bool _deleteDirectory(Dictionary<int, string> projectFolders, int key, string path, bool deleteRecursively = true, bool logDeletion = true)
        {
            try
            {
                FileSystem.DeleteDirectory(path, recycle: RecycleOption.SendToRecycleBin, showUI: UIOption.OnlyErrorDialogs);
                //Directory.Delete(path, deleteRecursively);
                projectFolders.Remove(key);
                if (logDeletion)
                    Console.WriteLine("Successfully removed " + path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static string _find(string path, string extension = ".cs")
        {
            var query = (from _path in _getOnlyFileNames(Directory.GetFiles(path))
                         where _path.EndsWith(extension)
                         select _path).ToHashSet();

            string s = "";

            if (query.Contains("Form1.cs"))
                query.TryGetValue("Form1.cs", out s);
            else if (query.Contains("Program.cs"))
                query.TryGetValue("Program.cs", out s);
            else if (query.Count > 0)
                s = query.AsEnumerable().First().ToString();

            if (string.IsNullOrEmpty(s))
                throw new Exception();
            else
                return s;
        }
        private static string[] _getOnlyFileNames(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = Path.GetFileName(paths[i]);
            }
            return paths;
        }
        private static bool _IsSubstring(string[] compare, string isPresent)
        {
            for (var i = 0; i < compare.Length; i++)
            {
                if (isPresent.Contains(compare[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
