namespace ProjectFolderManager;

public class App
{
    private static readonly string ProjectFolder =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Source\Repos";

    private static readonly string Temp = ProjectFolder + "\\" + "Temp";
    private static readonly string SmallTempPath = ProjectFolder + "\\" + "temp";

    private static Dictionary<int, string> _projectFolders;

    public static void Run()
    {
        Console.WriteLine("Project folder: " + ProjectFolder);
        Console.WriteLine("\tProjects: ");

        int i = 0;
        _projectFolders = Directory.GetDirectories(ProjectFolder).ToDictionary(x => i++);

        Commands.Repeat(_projectFolders, Temp, SmallTempPath);

        bool repeat;
        do
        {
            repeat = IssueCommand(ReadCommand());
        } while (repeat);
    }

    private static bool IssueCommand(string command)
    {
        bool parseSuccess = Enum.TryParse(command, out CommandsType cmd);

        if (!parseSuccess)
            Commands.Help();

        switch (cmd)
        {
            case CommandsType.Help:
                Commands.Help();
                break;
            case CommandsType.Repeat:
                Commands.Repeat(_projectFolders);
                break;
            case CommandsType.Cat:
                Commands.Cat(command, _projectFolders);
                break;
            case CommandsType.End:
                Console.WriteLine("Goodbye");
                return false;
            case CommandsType.Delete:
                Commands.Delete(command, _projectFolders);
                break;
            default:
                var exception = new ArgumentOutOfRangeException
                {
                    HelpLink = null,
                    HResult = 0,
                    Source = null
                };
                throw exception;
        }

        return true;
    }

    private static string ReadCommand()
    {
        Console.Write("Command: ");
        return Console.ReadLine() ?? "end";
    }
}