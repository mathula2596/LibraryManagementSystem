using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

class Layout
{
    public static string backgroundColor = "";
    public static string textColor = "";

    public static TextInfo textFormatter = new CultureInfo("en-US", false).TextInfo;

    //Entier customiseConsoleSize procedure was copied from the below link to open the command prompt in full screen 
    /*https://social.msdn.microsoft.com/Forums/vstudio/en-US/62f5f6a2-127f-4fa5-a6a9-69efb167baa2/consolesetwindowsize-and-position-help?forum=csharpgeneral*/

    [DllImport("kernel32.dll", ExactSpelling = true)]

    public static extern IntPtr GetConsoleWindow();

    public static IntPtr ThisConsole = GetConsoleWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public const int Maximize = 3;
    
    //Customising the command prompt size
    public static void customiseConsoleSize()
    {
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        Layout.ShowWindow(Layout.ThisConsole, Layout.Maximize);
    }

    //Add tab space before print a line
    public static void customiseWrite(string message)
    {
        Console.WriteLine();
        Console.Write("\t" + message);
    }
    //Add tab space before print a line
    public static void customiseWriteLine(string message)
    {
        Console.WriteLine("\t" + message);
    }

    //Center Alignment
    public static void headerTitle(string title = "")
    {
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
        Console.Write(title);
        Console.WriteLine("\n\n\n");
    }

    //Get the company or library name
    public static void libraryName()
    {
        string libraryName = "Your Library Name Here";
        string title = "Library System";
        //Check whether the configuration file is available or not
        try
        {
            Config.configDictionary = Config.loadConfigFile(Config.ConfigFilePath);
            libraryName = Config.configDictionary.ContainsKey("LibraryName") ? Config.configDictionary["LibraryName"].ToUpper() : libraryName;

            title = Config.configDictionary.ContainsKey("LibraryName") ? Config.configDictionary["ConsoleTitle"] : title;
        }
        catch (FileNotFoundException e)
        {
            Layout.customiseWriteLine(e.Message + " Configuration file is missing please include that file and rerun the system!");
        }

        Console.Title = title;
        string underLine = "-------------------------";
        Console.SetCursorPosition((Console.WindowWidth - underLine.Length) / 2, Console.CursorTop);
        Console.WriteLine(underLine);
        Console.SetCursorPosition((Console.WindowWidth - libraryName.Length) / 2, Console.CursorTop);
        Console.WriteLine(libraryName);
        Console.SetCursorPosition((Console.WindowWidth - underLine.Length) / 2, Console.CursorTop);
        Console.WriteLine(underLine);
        Console.WriteLine();


    }

    //Customising the command prompt color
    public static void customiseConsoleColor()
    {
        //Check whether the configuration file is available or not
        try
        {
            Config.configDictionary = Config.loadConfigFile(Config.ConfigFilePath);

            backgroundColor = textFormatter.ToTitleCase(Config.configDictionary.ContainsKey("BackgroundColor") ? Config.configDictionary["BackgroundColor"] : "Black");
            textColor = textFormatter.ToTitleCase(Config.configDictionary.ContainsKey("TextColor") ? Config.configDictionary["TextColor"] : "Black");

            // Check the spelling of the colors in the configuration file
            try
            {
                //https://www.codeproject.com/Questions/121424/Convert-String-to-System-ConsoleColor
                Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), backgroundColor);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), textColor);
                Console.Clear();
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message + " Please check the spelling at the configuration file");
            }
        }
        catch (FileNotFoundException e)
        {
            Layout.customiseWriteLine(e.Message + " Configuration file is missing please include that file and rerun the system!");
        }

    }
}



