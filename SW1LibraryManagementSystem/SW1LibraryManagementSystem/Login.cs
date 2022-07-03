using System;

class Login
{
    public static string userName = "";
    public static string password = "";
    public static int passwordFailureAttempt = 3;
    public static string enteredUserName = "";
    public static string enteredPassword = "";

    public static void getLoginDetails()
    {
        Config.configDictionary = Config.loadConfigFile(Config.ConfigFilePath);

        userName = Config.configDictionary.ContainsKey("UserName") ? Config.configDictionary["UserName"] : "";
        password = Config.configDictionary.ContainsKey("Password") ? Config.configDictionary["Password"] : "";
        passwordFailureAttempt = Config.configDictionary.ContainsKey("PasswordFailureAttempt") ? int.Parse(Config.configDictionary["PasswordFailureAttempt"]) : 3;

    }

    public static bool userLogin()
    {
        getLoginDetails();
        Layout.headerTitle("Login Screen");
        int attempt = 0;
        do
        {
            if (attempt != 0 && attempt < passwordFailureAttempt)
            {
                Layout.customiseWrite("Please check your username and password!\n");
            }
            if (attempt > passwordFailureAttempt-1)
            {
                Console.Clear();
                Layout.customiseWriteLine("\nAttempt Exceeded!\n");
                break;
                
            }
            Layout.customiseWrite("Enter your username: ");
            enteredUserName = Console.ReadLine();
            Layout.customiseWrite("Enter your password: ");
            enteredPassword = CheckPassword();
            
            attempt++;

        } while (userName != enteredUserName || password != enteredPassword);


        if (userName == enteredUserName && password == enteredPassword)
        {
            return true;
        }
        else
        {
            return false;

        }
    }

    //https://www.c-sharpcorner.com/article/show-asterisks-instead-of-characters-for-password-input-in-console-application/
    //Hide password characters
    public static string CheckPassword()
    {
        enteredPassword = "";
        do
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                enteredPassword += key.KeyChar;
                Console.Write("*");
            }
            else
            {
                if (key.Key == ConsoleKey.Backspace && enteredPassword.Length > 0)
                {
                    enteredPassword = enteredPassword.Substring(0, (enteredPassword.Length - 1));
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    
                        Console.WriteLine("");
                        break;
                }
            }
        } while (true);
       

        return enteredPassword;
    }
}
