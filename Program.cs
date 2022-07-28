
namespace ProjectFolderManager
{
    internal class Program
    {
        static readonly string ProjectFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Source\\Repos";
        static readonly string Temp = ProjectFolder + "\\" + "Temp";
        static readonly string temp = ProjectFolder + "\\" + "temp";

        static Dictionary<int, string> projectFolders;

        static void Main()
        {
            Console.WriteLine("Project folder: " + ProjectFolder);
            Console.WriteLine("\tProjects: ");

            int i = 0;
            projectFolders = Directory.GetDirectories(ProjectFolder).ToDictionary(x => i++);

            Commands.Repeat(projectFolders, Temp, temp);

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
                Commands.Delete(command, projectFolders);
            }
            else if (command.StartsWith("cat"))
            {
                Commands.Cat(command, projectFolders);
            }
            else if (command.StartsWith("repeat"))
            {
                Commands.Repeat(projectFolders, Temp, temp);
            }
            else if (command.StartsWith("help"))
            {
                Commands.Help();
            }

            repeat = true;
            return;
        }
        private static string ReadCommand()
        {
            Console.Write("Command: ");
            string ret = Console.ReadLine();

            if (ret != null)
                return ret;
            else
                return "end";
        }
        

    }
}