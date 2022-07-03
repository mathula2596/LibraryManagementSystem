using System;
using System.IO;

class Program
{
    public static void customise()
    {
        Layout.customiseConsoleSize();
        Layout.customiseConsoleColor();
        Layout.libraryName();
    }
    public static void Main()
    {
        // check the config file availability
        if(File.Exists(Config.ConfigFilePath))
        {
            customise();
            if (Login.userLogin())
            {
                Menu.mainMenu();
            }
            else
            {
                Layout.customiseWriteLine("Please check the login details!");
            }
        }
        else
        {
            Layout.customiseWriteLine("Please check the configuration file. It is missing!");
        }
    }
}

