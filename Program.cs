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

        static readonly string[] commands = { "del", "cat", "repeat", "end" };

        static Dictionary<int, string> subjectToDeletion;
        static void Main()
        {
            Console.WriteLine("Project folder: " + ProjectFolder);
            Console.WriteLine("\tProjects: ");


            var dirs = Directory.GetDirectories(ProjectFolder).AsEnumerable<string>();
            foreach (string s in dirs)
            {
                Console.WriteLine("\t\t" + s);
            }

            Console.WriteLine();
            Console.WriteLine("\tSubject to deletion: ");

            int num = 0;
            subjectToDeletion = (from dir in dirs
                                     where dir.StartsWith(Temp) || dir.StartsWith(temp)
                                     select dir).ToDictionary(dir => ++num);

            foreach ((int key, string s) in subjectToDeletion)
                Console.WriteLine(key + "\t\t" + s);
            Console.WriteLine();

            Repeat(() => IssueCommand(ReadCommand()));
            
        }

        private static void Repeat(Action repeated)
        {
        START:
            repeated();

#if BLABBER
        ASKAGAIN:
            Console.WriteLine("Continue? [y/n]");
            string answer = Console.ReadLine();
            if (answer.StartsWith("y"))
                goto START;
            else if (answer.StartsWith("n"))
                return;
            else
                goto ASKAGAIN;
#endif
#if !BLABBER
        goto START;
#endif

        }
        private static void IssueCommand(string command)
        {
            if (command.Equals("end"))
                return;
            else if (command.StartsWith("del"))
            {
                try
                {
                    int key = Convert.ToInt32(command.Substring(3));
                    DeleteDirectory(subjectToDeletion[key], key);
                }
                catch
                {
                    throw new Exception("Failed to delete your folder");
                }
            }
            else if (command.StartsWith("cat"))
            {
                string path = subjectToDeletion[Convert.ToInt32(command.Substring(3))];
                string file = Find(path);

                using FileStream fs = new FileStream(path + "\\" + file, FileMode.Open);
                using StreamReader sr = new StreamReader(fs);
                {
                    Console.WriteLine(sr.ReadToEnd());
                }
            }
            else if (command.StartsWith("repeat"))
            {
                foreach ((int key, string s) in subjectToDeletion)
                    Console.WriteLine(key + "\t\t" + s);
            }
            else if (command.StartsWith("help"))
            {
                foreach (string cmd in commands)
                {
                    Console.WriteLine("\t" + cmd);
                }
            }
            
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

            string? s = "";

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
        private static bool DeleteDirectory(string path, int dictKey, bool deleteRecursively = true, bool logDeletion = true)
        {
            try
            {
                FileSystem.DeleteDirectory(path, recycle: RecycleOption.SendToRecycleBin, showUI: UIOption.OnlyErrorDialogs);
                //Directory.Delete(path, deleteRecursively);
                subjectToDeletion.Remove(dictKey);
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