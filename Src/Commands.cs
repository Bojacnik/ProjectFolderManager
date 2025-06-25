using Microsoft.VisualBasic.FileIO;

namespace ProjectFolderManager
{
    public enum CommandsType
    {
        End,
        Delete,
        Cat,
        Repeat,
        Help
    }

    public static class Commands
    {
        public static void Delete(string command, Dictionary<int, string> projectFolders)
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
            var fs = new FileStream(path + "\\" + file, FileMode.Open);
            var sr = new StreamReader(fs);
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
            foreach (string cmd in Enum.GetValuesAsUnderlyingType<CommandsType>())
            {
                Console.WriteLine("\t" + cmd);
            }
        }


        private static bool _deleteDirectory(Dictionary<int, string> projectFolders, int key, string path,
            bool deleteRecursively = true, bool logDeletion = true)
        {
            try
            {
                FileSystem.DeleteDirectory(path, recycle: RecycleOption.SendToRecycleBin,
                    showUI: UIOption.OnlyErrorDialogs);
                //Directory.Delete(path, deleteRecursively);
                projectFolders.Remove(key);
                if (logDeletion)
                    Console.WriteLine("Successfully removed " + path);
                return true;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("IO Exception " + e.Message);
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.Error.WriteLine("Unauthorized access " + e.Message);
                return false;
            }
        }

        private static string _find(string path, string extension = ".cs")
        {
            HashSet<string> query = (from _path in _getOnlyFileNames(Directory.GetFiles(path))
                where _path.EndsWith(extension)
                select _path).ToHashSet();

            string s = "";

            if (query.Contains("Form1.cs"))
                query.TryGetValue("Form1.cs", out s);
            else if (query.Contains("Program.cs"))
                query.TryGetValue("Program.cs", out s);
            else if (query.Count > 0)
                s = query.AsEnumerable().First();

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
            return compare.Any(isPresent.Contains);
        }
    }
}