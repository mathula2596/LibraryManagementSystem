using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Students
{
    private const string FileName = "students.bin";
    private const string TempFile = "tempFileStudents.bin";

    public string getFileName()
    {
        return FileName;
    }
    public static void clearConsole()
    {
        Console.Clear();
        Program.customise();
    }
    public static void getStudentDetails(string idDefault = "0")
    {
        string id = idDefault;

        // Add new students
        if (idDefault == "0")
        {
            clearConsole();
            Layout.headerTitle("Add New Students Details");

            Layout.customiseWrite("Enter the Student ID: ");
            id = Console.ReadLine();
        }
        Layout.customiseWrite("Enter the Student First Name: ");
        string firstName = Console.ReadLine();

        Layout.customiseWrite("Enter the Student Last Name: ");
        string lastName = Console.ReadLine();

        Layout.customiseWrite("Enter the Student DOB (dd/mm/yyyy): ");
        string dob = Console.ReadLine();

        Layout.customiseWrite("Enter the Student Address: ");
        string address = Console.ReadLine();

        Layout.customiseWrite("Enter the Student Phone Number (+00000000000): ");
        string phoneNumber = Console.ReadLine();

        Layout.customiseWrite("Enter the Student Email: ");
        string email = Console.ReadLine();

        // Add new students
        if (idDefault == "0")
        {
            addStudent(id, firstName, lastName, dob, address, phoneNumber, email);
        }
        else
        {
            //Update the students
            Users student = new Users(id, firstName, lastName, dob, address, phoneNumber, email);
            Users.updateDetails(FileName, TempFile, id, student);
        }

    }
    public static void addStudent(string id, string firstName, string lastName, string dob, string address, string phoneNumber, string email)
    {
        Users student = new Users(id,firstName,lastName,dob,address,phoneNumber,email);

        Users.storeDetails(FileName,student);
    }
    public static void viewStudent()
    {
        clearConsole();
        Users.viewDetails(FileName);
    }
    public static void searchStudent()
    {
        clearConsole();
        Users.searchDetails(FileName);
    }
    public static void editStudent()
    {
        clearConsole();
        Layout.headerTitle("Edit the Students Details");
        bool idVerification = false;
        string id = "";
        int count = 0; 
        do
        {
            if(count > 0)
            {
                Layout.customiseWriteLine("Please check your ID!");
            }
            Layout.customiseWrite("Enter the Student ID: ");
            id = Console.ReadLine().ToUpper();
            idVerification = Users.idChecker(FileName, id);
            count++;
        } while (!idVerification);

        getStudentDetails(id);
    }
    public static void deleteStudent()
    {
        clearConsole();
        Layout.headerTitle("Delete the Students Details");
        bool idVerification = false;
        string id = "";
        int count = 0;
        string confirmation = "";
        do
        {
            if (count > 0)
            {
                Layout.customiseWriteLine("Please check your ID!");
            }
            Layout.customiseWrite("Enter the Student ID: ");
            id = Console.ReadLine().ToUpper();
            idVerification = Users.idChecker(FileName, id);
            count++;
        } while (!idVerification);

        Layout.customiseWrite("Are you sure do you want to delete (Y/N)? ");
        confirmation = Console.ReadLine().ToLower();

        if(confirmation == "y")
        {
            Users.deleteDetails(id,FileName,TempFile);
        }
        else
        {
            Menu.mainMenu();
        }
    }
}
