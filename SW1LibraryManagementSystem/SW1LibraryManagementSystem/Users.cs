using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

[Serializable]
class Users
{
    private string id;
    private string firstName = "";
    private string lastName = "";
    private string dob = "";
    private string address = "";
    private string phoneNumber = "";
    private string email = "";

    public static TextInfo textFormatter = new CultureInfo("en-US", false).TextInfo;
    public static BinaryFormatter binaryFormatter = new BinaryFormatter();
    public static FileStream file;
    
    public static string[] fields = { "ID" , "FirstName" , "LastName", "DOB", "Address", "PhoneNumber", "Email"};
    public Users(string id, string firstName, string lastName, string dob, string address, string phoneNumber, string email)
    {
        setId(textFormatter.ToTitleCase(id));
        setName(textFormatter.ToTitleCase(firstName), textFormatter.ToTitleCase(lastName));
        setDob(dob);
        setPhoneNumber(phoneNumber);
        setEmail(email);
        this.address = address;
    }
    public void setId(string id)
    {
        if (Regex.Match(id, @"^[A-Z]{1,}[0-9]{7,7}$").Success)
        {
            this.id = id;
        }
        else
        {
            Layout.customiseWriteLine("Invalid student id");
        }
    }
    public void setName(string firstName, string lastName)
    {
        //http://www.java2s.com/Code/CSharp/Development-Class/Validatefirstnameandlastname.htm
        if (Regex.Match(firstName, "^[A-Z][a-zA-Z]*$").Success)
        {
            this.firstName = firstName;
        }
        else
        {
            Layout.customiseWriteLine("Invalid first name");
        }
        if (Regex.Match(lastName, "^[A-Z][a-zA-Z]*$").Success)
        {
            this.lastName = lastName;
        }
        else
        {
            Layout.customiseWriteLine("Invalid last name");
        }
    }
    public void setDob(string dob)
    {
        //https://www.aspsnippets.com/Articles/Validate-date-string-in-ddMMyyyy-format-in-ASPNet.aspx
        if (Regex.Match(dob, "(((0|1)[0-9]|2[0-9]|3[0-1])\\/(0[1-9]|1[0-2])\\/((19|20)\\d\\d))$").Success)
        {
            this.dob = dob;
        }
        else
        {
            Layout.customiseWriteLine("Invalid date of birth");
        }
    }
    public void setPhoneNumber(string phoneNumber)
    {
        //https://www.csharp-console-examples.com/csharp-console/simple-regular-expression-phone-number-validation-in-c/
        if (Regex.Match(phoneNumber, @"^([\+][1-9]{2,3}[-]?|[0])?[1-9][0-9]{8,8}$").Success)
        {
            this.phoneNumber = phoneNumber;
        }
        else
        {
            Layout.customiseWriteLine("Invalid phone number");
        }
    }
    public void setEmail(string email)
    {
        if (Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
        {
            this.email = email;
        }
        else
        {
            Layout.customiseWriteLine("Invalid email address");
        }
    }
    public string getId()
    {
        return this.id;
    }
    public string getFirstName()
    {
        return this.firstName;
    }
    public string getLastName()
    {
        return this.lastName;
    }
    public string getDob()
    {
        return this.dob;
    }
    public string getAddress()
    {
        return this.address;
    }
    public string getPhoneNumber()
    {
        return this.phoneNumber;
    }
    public string getEmail()
    {
        return this.email;
    }
    public static void searchDetails(string FileName)
    {
        if (File.Exists(FileName))
        {
            file = File.Open(FileName, FileMode.Open, FileAccess.Read);

            DataTables.clearDataTable();
            DataTables.dataTableStructure(fields);
            Layout.headerTitle("Search the User Details");
            DataTables.dataTableHeader(fields);


            while (file.Position != file.Length)
            {
                try
                {
                    Users user = (Users)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(user.id, user.firstName, user.lastName, user.dob, user.address, user.phoneNumber, user.email);
                }
                catch (Exception e)
                {
                    Layout.customiseWriteLine(e.Message);
                    Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                    break;
                }
               

            }
            string searchField = "ID";
            string searchValue = "";
            int count = 0;
            do
            {
                if (count > 0)
                {
                    Layout.customiseWriteLine("Sorry type the field in correct format!");
                }

                Layout.customiseWrite("Which field do you want to search? ");
                searchField = Console.ReadLine();

                count++;

            } while (!DataTables.dataTable.Columns.Contains(searchField));

            Layout.customiseWrite("Enter the value to search the file: ");
            searchValue = Console.ReadLine();
            Console.WriteLine();
            if (DataTables.dataTable.Select( searchField + " = '" + searchValue + "'").Length == 0)
            {
                Layout.customiseWriteLine("No Records Found!");
            }
            DataTables.dataTableBody(searchField + " = '" + searchValue + "'","",fields);
           
            file.Close();
        }
        else
        {
            Layout.customiseWriteLine("File is Missing!");
        }
    }
    //Check the student ID is available or not
    public static bool idChecker(string FileName, string id)
    {
        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields);
        Console.WriteLine();

        file = File.Open(FileName, FileMode.Open, FileAccess.Read);


        while (file.Position != file.Length)
        {
            try
            {
                Users user = (Users)binaryFormatter.Deserialize(file);
                DataTables.dataTable.Rows.Add(user.id, user.firstName, user.lastName, user.dob, user.address, user.phoneNumber, user.email);
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message);
                Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                break;
            }
           
        }

        if (DataTables.dataTable.Select("ID = '" + id + "'").Length > 0)
        {
            DataTables.dataTableHeader(fields);
        }

        DataTables.dataTableBody("ID = '" + id + "'", "",fields);
        file.Close();

        if (DataTables.dataTable.Select("ID = '" + id + "'").Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static void deleteDetails(string id,string FileName, string TempFile)
    {
        file.Close();

        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields);
        DataTables.dataTableBody("ID = '" + id + "'", "",fields);

        FileStream files;

        file = File.Open(FileName, FileMode.Open, FileAccess.Read);

        if (File.Exists(TempFile))
        {
            files = File.Open(TempFile, FileMode.Truncate, FileAccess.Write);
            files.Close();
            files = File.Open(TempFile, FileMode.Append, FileAccess.Write);
        }
        else
        {
            files = File.Create(TempFile);
        }
        while (file.Position != file.Length)
        {
            try
            {
                Users user = (Users)binaryFormatter.Deserialize(file);

                if (user.id == id)
                {
                    continue;
                }
                binaryFormatter.Serialize(files, user);
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message);
                Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                break;
            }

           
        }
        Console.WriteLine();
        Layout.customiseWriteLine("Student Record Deleted Successfully");

        file.Close();
        files.Close();
        File.Delete(FileName);
        File.Move(TempFile, FileName);
    }
    public static void updateDetails(string FileName, string TempFile, string id, Users userDetails)
    {
        file.Close();

        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields);
        DataTables.dataTableBody("ID = '" + id + "'", "",fields);
        
        FileStream tempFiles;
        file = File.Open(FileName, FileMode.Open, FileAccess.Read);

        if (File.Exists(TempFile))
        {
            tempFiles = File.Open(TempFile, FileMode.Truncate, FileAccess.Write);
            tempFiles.Close();
            tempFiles = File.Open(TempFile, FileMode.Append, FileAccess.Write);
        }
        else
        {
            tempFiles = File.Create(TempFile);
        }
        while (file.Position != file.Length)
        {
            try
            {
                Users user = (Users)binaryFormatter.Deserialize(file);

                if (user.id == id)
                {
                    if (user.id != "" && userDetails.firstName != "" && userDetails.lastName != "" && userDetails.dob != "" && userDetails.email != "" && userDetails.phoneNumber != "" && userDetails.address != "")
                    {
                        user.setName(userDetails.firstName, user.lastName);
                        user.firstName = user.getFirstName();

                        user.setName(user.firstName, userDetails.lastName);
                        user.lastName = user.getLastName();

                        user.setDob(userDetails.dob);
                        user.dob = user.getDob();

                        user.address = userDetails.address;

                        user.setPhoneNumber(userDetails.phoneNumber);
                        user.phoneNumber = user.getPhoneNumber();

                        user.setEmail(userDetails.email);
                        user.email = user.getEmail();

                        binaryFormatter.Serialize(tempFiles, user);

                        Console.WriteLine();
                        Layout.customiseWriteLine("Student Record Updated Successfully");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine();
                        Layout.customiseWriteLine("Please check your inputs");
                    }

                }
                binaryFormatter.Serialize(tempFiles, user);
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message);
                Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                break;
            }
           
        }
       
        file.Close();
        tempFiles.Close();
        File.Delete(FileName);
        File.Move(TempFile, FileName);
    }
    public static void viewDetails(string FileName)
    {
        if (File.Exists(FileName))
        {
            file = File.Open(FileName,FileMode.Open,FileAccess.Read);
            DataTables.clearDataTable();
            DataTables.dataTableStructure(fields);
            Layout.headerTitle("List All the User Details");
            DataTables.dataTableHeader(fields);

            while (file.Position != file.Length)
            {
                try
                {
                    Users user = (Users)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(user.id, user.firstName, user.lastName, user.dob, user.address, user.phoneNumber, user.email);
                }
                catch (Exception e)
                {
                    Layout.customiseWriteLine(e.Message);
                    Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                    break;
                }

            }
            if (DataTables.dataTable.Select("", "").Length == 0)
            {
                Layout.customiseWriteLine("No Records Found!");
            }

            Layout.customiseWrite("Are you want to sort and view the above table (Y/N)? ");
            string sortSelection = Console.ReadLine().ToLower();
            Console.WriteLine();
            string sortField = "";
            string sortOrder = "";

            int count = 0;
            if (sortSelection == "y")
            {
                do
                {
                    if (count > 0)
                    {
                        Layout.customiseWriteLine("Sorry type the fields are not in correct format!");
                    }

                    Layout.customiseWrite("Which field do you want to sort? ");
                    sortField = Console.ReadLine();

                    Layout.customiseWrite("Which order do you want sort (ASC/DESC) \n");
                    Layout.customiseWrite("Type \"ASC\" for Ascending Order and \"DESC\" for Descending Order? ");
                    sortOrder = Console.ReadLine();
                    Console.WriteLine();
                    count++;
                   
                } while (!DataTables.dataTable.Columns.Contains(sortField) || (sortOrder != "ASC" && sortOrder != "DESC"));
               
                DataTables.dataTableBody("", sortField + " " + sortOrder, fields);
            }

            else
            {
                DataTables.dataTableBody("","",fields);
            }
            
            file.Close();
        }
        else
        {
            Layout.customiseWriteLine("File is Missing!");
        }

    }

    //https://www.c-sharpcorner.com/UploadFile/d3e4b1/serializing-and-deserializing-the-object-as-binary-data-usin/
    public static void storeDetails(string FileName, Users user)
    {
       
        if (user.id != "" && user.firstName != "" && user.lastName != "" && user.dob != "" && user.email != "" && user.phoneNumber != "")
        {
            Layout.customiseWrite("Are you sure do you want to save? (Y/N)");
            string confirmStore = Console.ReadLine().ToLower();
            if(confirmStore == "y")
            {

                if (File.Exists(FileName))
                {
                    DataTables.clearDataTable();
                    DataTables.dataTableStructure(fields);

                    file = File.Open(FileName, FileMode.Open, FileAccess.Read);

                    while (file.Position != file.Length)
                    {
                        try
                        {
                            Users userData = (Users)binaryFormatter.Deserialize(file);
                            DataTables.dataTable.Rows.Add(userData.id, userData.firstName, userData.lastName, userData.dob, userData.address, userData.phoneNumber, userData.email);
                        }
                        catch (Exception e)
                        {
                            Layout.customiseWriteLine(e.Message);
                            Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                            break;
                        }
                        
                    }
                    if (DataTables.dataTable.Select("ID = '" + user.id + "'").Length > 0)
                    {
                        DataTables.dataTableHeader(fields);
                        Console.WriteLine();
                    }
                    DataTables.dataTableBody("ID = '" + user.id + "'", "",fields);
                    file.Close();
                }

                if (File.Exists(FileName))
                {
                    file = File.Open(FileName, FileMode.Append);
                    if (DataTables.dataTable.Select("ID = '" + user.id + "'").Length == 0)
                    {
                        binaryFormatter.Serialize(file, user);
                        file.Close();
                        Console.WriteLine();
                        Layout.customiseWriteLine("Student Record Created Successfully");
                    }
                    else
                    {
                        Layout.customiseWriteLine("Sorry user already exist");
                        file.Close();
                    }
                }
                else
                {
                    file = File.Create(FileName);
                   
                    binaryFormatter.Serialize(file, user);
                    file.Close();
                    Console.WriteLine();
                    Layout.customiseWriteLine("Student Record Created Successfully");
                }
            }
           
        }
        else
        {
            Layout.customiseWriteLine("Please check your inputs");
        }
    }
}
