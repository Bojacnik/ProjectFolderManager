#define BLABBER
#undef BLABBER

using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ProjectFolderManager
{
    internal class Program
    {
        static readonly string ProjectFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Source\\Repos";
        static readonly string Temp = ProjectFolder + "\\" + "Temp";
        static readonly string temp = ProjectFolder + "\\" + "temp";

        static readonly string[] commands = { "end", "del", "cat", "repeat", "help"};
        static Dictionary<int, string> projectFolders;

        static void Main()
        {
            Console.WriteLine("Project folder: " + ProjectFolder);
            Console.WriteLine("\tProjects: ");

            int i = 0;
            projectFolders = Directory.GetDirectories(ProjectFolder).ToDictionary(x => i++);

            ConsoleColor defaultClsColor = Console.BackgroundColor;
            foreach ((int o, string s) in projectFolders)
            {
                Console.BackgroundColor = defaultClsColor;
                if (s.Contains(Temp) || s.Contains(temp))
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(o + "\t\t" + s);
            }
            Console.BackgroundColor = defaultClsColor;
            Console.WriteLine();

            bool repeat = true;
            do
            {
                IssueCommand(ReadCommand(), out repeat);
            } while (repeat);
        }

        private static void IssueCommand(string command, out bool repeat)
        {
            if (command.Equals("end"))
            { 
                repeat = false;
                return;
            }
            else if (command.StartsWith("del"))
            {
                try
                {
                    int key = Convert.ToInt32(command.Substring(3));
                    DeleteDirectory(key, projectFolders[key]);
                }
                catch
                {
                    throw new Exception("Failed to delete your folder");
                }
            }
            else if (command.StartsWith("cat"))
            {
                string path = projectFolders[Convert.ToInt32(command.Substring(3))];
                string file = Find(path);

                using FileStream fs = new FileStream(path + "\\" + file, FileMode.Open);
                using StreamReader sr = new StreamReader(fs);
                {
                    Console.WriteLine(sr.ReadToEnd());
                }
            }
            else if (command.StartsWith("repeat"))
            {
                ConsoleColor defaultClsColor = Console.BackgroundColor;
                foreach ((int o, string s) in projectFolders)
                {
                    Console.BackgroundColor = defaultClsColor;
                    if (s.Contains(Temp) || s.Contains(temp))
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    Console.WriteLine(o + "\t\t" + s);
                }
                Console.BackgroundColor = defaultClsColor;
                Console.WriteLine();
            }
            else if (command.StartsWith("help"))
            {
                foreach (string cmd in commands)
                {
                    Console.WriteLine("\t" + cmd);
                }
            }

            repeat = true;
            return;
        }
        private static string ReadCommand()
        {
            Console.Write("Command: ");
            string? ret = Console.ReadLine();

            if (ret != null)
                return ret;
            else
                return "end";
        }
        private static string Find(string path, string extension = ".cs")
        {
            var query = (from _path in GetOnlyFileNames(Directory.GetFiles(path))
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
        private static string[] GetOnlyFileNames(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = Path.GetFileName(paths[i]);
            }
            return paths;
        }
        private static bool DeleteDirectory(int key, string path, bool deleteRecursively = true, bool logDeletion = true)
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

    }
}