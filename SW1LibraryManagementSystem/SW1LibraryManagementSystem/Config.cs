using System;
using System.Collections.Generic;
using System.IO;

class Config
{
    public const string ConfigFilePath = "../../config/config.txt";

    public static Dictionary<string, string> configDictionary = new Dictionary<string, string>();

    //Get the data from config file
    public static string[] configValueFromConfigLine(string configuration)
    {
        string[] splitedValue = configuration.Split(':');
        return splitedValue;
    }

    //Convert the collected data from the config file into C# usable format
    public static Dictionary<string, string> loadConfigFile(string fileName)
    {
        string[] allLines = File.ReadAllLines(fileName);

        for (int i = 0; i < allLines.Length; i++)
        {
            if (allLines[i].Length > 0)
            {
                configDictionary[configValueFromConfigLine(allLines[i])[0]] = configValueFromConfigLine(allLines[i])[1];
            }
        }
        return configDictionary;
    }

    /*public static void escapeToMainMenu()
    {
        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.Escape)
        {
            Menu.mainMenu();
        }
    }*/
}
