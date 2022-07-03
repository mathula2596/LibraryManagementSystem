using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

[Serializable]
class Books
{
    private int id;
    private string title = "";
    private string author = "";
    private string publication = "";
    private string language = "";
    private string category = "";
    private string edition = "";
    private string shelf = "";

    private const string FileName = "books.bin";
    public const string TempFile = "tempFileBooks.bin";

    public static BinaryFormatter binaryFormatter = new BinaryFormatter();
    public static FileStream file;
    public static TextInfo textFormatter = new CultureInfo("en-US", false).TextInfo;


    public static string[] fields = {"ID", "Title", "Author", "Publication","Language","Category","Edition","Shelf"};

    public Books(int id, string title, string author, string publication, string language, string category, string edition,string shelf)
    {
        setId(id);
        this.title = textFormatter.ToTitleCase(title);
        this.author = textFormatter.ToTitleCase(author);
        this.publication = textFormatter.ToTitleCase(publication);
        setLanguage(language);
        this.category = textFormatter.ToTitleCase(category);
        setEdition(edition);
        this.shelf = shelf;
    }

    public Books()
    {

    }
    public int getId()
    {
        return this.id;
    }
    public void setId(int id)
    {
        if (File.Exists(FileName))
        {
            this.id = selectLastId() + 1;
        }
        else
        {
            this.id = 1;
        }
    }
    public void setEdition(string edition)
    {
        //https://www.csharp-console-examples.com/csharp-console/simple-regular-expression-phone-number-validation-in-c/
        if (Regex.Match(edition, @"^[0-9]{1,3}$").Success)
        {
            this.edition = edition;
        }
        else
        {
            Layout.customiseWriteLine("Invalid Edition");
        }
    }
    public string getEdition()
    {
        return this.edition;
    }
    public void setLanguage(string language)
    {
        if (Regex.Match(language, "^[a-zA-Z]*$").Success)
        {
            this.language = textFormatter.ToTitleCase(language);
        }
        else
        {
            Layout.customiseWriteLine("Invalid language");
        }
    }
    public string getLanguage()
    {
        return this.language;
    }
    public string getFileName()
    {
        return FileName;
    }

    public static void clearConsole()
    {
        Console.Clear();
        Program.customise();
    }
    public  static void getBooksDetails(int idDefault = 0)
    {
        int id = 1;
        if(idDefault == 0)
        {
            clearConsole();
            Layout.headerTitle("Add New Book Details");
        }
        Layout.customiseWrite("Enter the Title: ");
        string title = Console.ReadLine();

        Layout.customiseWrite("Enter the Author Name: ");
        string author = Console.ReadLine();

        Layout.customiseWrite("Enter the Publication: ");
        string publication = Console.ReadLine();

        Layout.customiseWrite("Enter the Language: ");
        string language = Console.ReadLine();

        Layout.customiseWrite("Enter the Category: ");
        string category = Console.ReadLine();

        Layout.customiseWrite("Enter the Edition: ");
        string edition = Console.ReadLine();

        Layout.customiseWrite("Enter the Shelf Location: ");
        string shelf = Console.ReadLine();

        bool status = true;

        if(idDefault == 0)
        {
            addBooks(id, title, author, publication, language, category, edition, status, shelf);
        }
        else
        {
            Books book = new Books(id, title, author, publication, language, category, edition, shelf);
            updateDetails(FileName, TempFile, idDefault, book);
        }

    }
    public static void updateDetails(string FileName, string TempFile, int id, Books bookDetails)
    {
        file.Close();

        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields);
        DataTables.dataTableBody("ID = '" + id + "'", "", fields);

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
                Books book = (Books)binaryFormatter.Deserialize(file);

                if (book.id == id)
                {
                    if (book.id > 0 && bookDetails.title != "" && bookDetails.author != "" && bookDetails.publication != "" && bookDetails.language != "" && bookDetails.category != "" && bookDetails.edition != "" && bookDetails.shelf != "")
                    {
                        book.title = bookDetails.title;
                        book.author = bookDetails.author;
                        book.publication = bookDetails.publication;
                        book.setLanguage(bookDetails.language);
                        book.language = book.getLanguage();
                        book.category = bookDetails.category;
                        book.edition = bookDetails.getEdition();
                        book.shelf = bookDetails.shelf;

                        binaryFormatter.Serialize(tempFiles, book);
                        Console.WriteLine();
                        Layout.customiseWriteLine("Books Record Updated Successfully");

                        continue;
                    }
                    else
                    {
                        Console.WriteLine();
                        Layout.customiseWriteLine("Please check your inputs");
                    }

                }
                binaryFormatter.Serialize(tempFiles, book);
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
    public  static void addBooks(int id, string title, string author, string publication, string language, string category, string edition, bool status, string shelf)
    {
        Books book = new Books(id, title, author, publication, language, category, edition, shelf);
        storeDetails(FileName, book);
    }
    public static void storeDetails(string FileName, Books book)
    {
        //Not null validation
        if (book.id > 0 && book.title != "" && book.author != "" && book.publication != "" && book.language != "" && book.category != "" && book.edition != "" && book.shelf != "")
        {
            Layout.customiseWrite("Are you sure do you want to save (Y/N)? ");
            string confirmStore = Console.ReadLine().ToLower();
            if (confirmStore == "y")
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
                            Books bookData = (Books)binaryFormatter.Deserialize(file);
                            DataTables.dataTable.Rows.Add(bookData.id, bookData.title, bookData.author, bookData.publication, bookData.language, bookData.category, bookData.edition, bookData.shelf);
                        }
                        catch (Exception e)
                        {
                            Layout.customiseWriteLine(e.Message);
                            Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                            break;
                        }
                        
                    }
                    if (DataTables.dataTable.Select("ID = '" + book.id + "'").Length > 0)
                    {
                        DataTables.dataTableHeader(fields);
                    }
                    DataTables.dataTableBody("ID = '" + book.id + "'", "", fields);
                    file.Close();
                }

                if (File.Exists(FileName))
                {
                    file = File.Open(FileName, FileMode.Append);
                    if (DataTables.dataTable.Select("ID = '" + book.id + "'").Length == 0)
                    {
                        binaryFormatter.Serialize(file, book);
                        file.Close();
                        Console.WriteLine();
                        Layout.customiseWriteLine("Books Record Created Successfully");
                    }
                    else
                    {
                        Console.WriteLine();
                        Layout.customiseWriteLine("Sorry Book Already Exist");
                        file.Close();
                    }
                }
                else
                {
                    file = File.Create(FileName);
                    binaryFormatter.Serialize(file, book);
                    Console.WriteLine();
                    Layout.customiseWriteLine("Books Record Created Successfully");
                    file.Close();
                }
            }

        }
        else
        {
            Console.WriteLine();
            Layout.customiseWriteLine("Please check your inputs");
        }
    }
    //get the id of the last record to use as auto id
    public static int selectLastId()
    {
        file = File.Open(FileName, FileMode.Open, FileAccess.Read);
        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields,"int");
        while (file.Position != file.Length)
        {
            try
            {
                Books book = (Books)binaryFormatter.Deserialize(file);
                DataTables.dataTable.Rows.Add(book.id, book.title, book.author, book.publication, book.language, book.category, book.edition, book.shelf);
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message);
                Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                break;
            }
          

        }
        int lastSavedId = 0;
        if (DataTables.dataTable.Select("", "").Length > 0)
        {
            foreach (DataRow row in DataTables.dataTable.Select("", "ID DESC"))
            {
                foreach (var field in fields)
                {
                    lastSavedId = int.Parse(row[field].ToString());
                    break;
                }
                break;
            }
        }

        file.Close();
        return lastSavedId;

    }
    public static void viewDetails(string FileName)
    {
        if (File.Exists(FileName))
        {
            file = File.Open(FileName, FileMode.Open, FileAccess.Read);
            DataTables.clearDataTable();
            DataTables.dataTableStructure(fields,"int");
            Layout.headerTitle("List All the Books Details");
            DataTables.dataTableHeader(fields);

            while (file.Position != file.Length)
            {
                try
                {
                    Books book = (Books)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(book.id,book.title,book.author,book.publication,book.language,book.category,book.edition,book.shelf);
                }
                catch(Exception e)
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
                        Layout.customiseWriteLine("Sorry type the field not in correct format!");
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
                DataTables.dataTableBody("", "",fields);
            }

            file.Close();
        }
        else
        {
            Layout.customiseWriteLine("File is Missing!");
        }

    }
    public static void viewBooks()
    {
        clearConsole();
        Books.viewDetails(FileName);
    }
    public static void searchBooks()
    {
        clearConsole();
        Books.searchDetails(FileName);
    }
    public static void searchDetails(string FileName)
    {
        if (File.Exists(FileName))
        {
            file = File.Open(FileName, FileMode.Open, FileAccess.Read);

            DataTables.clearDataTable();
            DataTables.dataTableStructure(fields);
            Layout.headerTitle("Search the Books Details");
            DataTables.dataTableHeader(fields);

            while (file.Position != file.Length)
            {
                try
                {
                    Books book = (Books)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(book.id, book.title, book.author, book.publication, book.language, book.category, book.edition, book.shelf);
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
            if (DataTables.dataTable.Select(searchField + " = '" + searchValue + "'").Length == 0)
            {
                Layout.customiseWriteLine("No Records Found!");
            }
            DataTables.dataTableBody(searchField + " = '" + searchValue + "'", "", fields);

            file.Close();
        }
        else
        {
            Layout.customiseWriteLine("File is Missing!");
        }

    }
    // Check the book Id is available or not
    public static bool idChecker(string FileName, int id)
    {
        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields);
        file = File.Open(FileName, FileMode.Open, FileAccess.Read);

        while (file.Position != file.Length)
        {
            try
            {
                Books book = (Books)binaryFormatter.Deserialize(file);
                DataTables.dataTable.Rows.Add(book.id, book.title, book.author, book.publication, book.language, book.category, book.edition, book.shelf);
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message);
                Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                break;
            }

        }
        Console.WriteLine();
        if (DataTables.dataTable.Select("ID = '" + id + "'").Length > 0)
        {
            DataTables.dataTableHeader(fields);
            Console.WriteLine();
        }
        DataTables.dataTableBody("ID = '" + id + "'", "", fields);
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
    public static void deleteBook()
    {
        clearConsole();
        Layout.headerTitle("Delete the Books Details");
        bool idVerification = false;
        int id = 0;
        string confirmation = "";

        int count = 0;
        do
        {
            if (count > 0)
            {
                Layout.customiseWriteLine("Please check your ID!");
            }
            Layout.customiseWrite("Enter the Book ID: ");
            string enteredId = Console.ReadLine();

            if (int.TryParse(enteredId, out id))
            {
                id = int.Parse(enteredId);
            }
            idVerification = idChecker(FileName, id);
            count++;

        } while (!idVerification);

        Layout.customiseWrite("Are you sure do you want to delete (Y/N)? ");
        confirmation = Console.ReadLine().ToLower();

        if (confirmation == "y")
        {
            deleteDetails(id, FileName, TempFile);
        }
        else
        {
            Menu.mainMenu();
        }

    }
    public static void deleteDetails(int id, string FileName, string TempFile)
    {
        file.Close();

        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields);
        DataTables.dataTableBody("ID = '" + id + "'", "", fields);

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
                Books book = (Books)binaryFormatter.Deserialize(file);

                if (book.id == id)
                {
                    continue;
                }
                binaryFormatter.Serialize(files, book);
            }
            catch (Exception e)
            {
                Layout.customiseWriteLine(e.Message);
                Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                break;
            }
            
        }
        Console.WriteLine();
        Layout.customiseWriteLine("Books Record Deleted Successfully");

        file.Close();
        files.Close();
        File.Delete(FileName);
        File.Move(TempFile, FileName);

    }
    public static void editBook()
    {
        clearConsole();
        Layout.headerTitle("Edit the Books Details");

        bool idVerification = false;
        int id = -1;
        int count = 0;
        do
        {
            if (count > 0)
            {
                Layout.customiseWriteLine("Please check your ID!");
            }
            Layout.customiseWrite("Enter the Book ID: ");
            string enteredId = Console.ReadLine();
            if (int.TryParse(enteredId, out id))
            {
                id = int.Parse(enteredId);
            }
            idVerification = idChecker(FileName, id);
            count++;

        } while (!idVerification);

        getBooksDetails(id);

    }
}

