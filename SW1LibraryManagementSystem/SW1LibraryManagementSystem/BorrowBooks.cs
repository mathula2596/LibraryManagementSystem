using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

[Serializable]
class BorrowBooks
{
    private int id;
    private string studentId = "";
    private int bookId = 0;
    private string borrowedDate;
    private string returnDate;
    private string status = "Lended";
    private string actualReturnedDate;
    private Decimal fine;

    private static int allowedPeriod = 1;
    private static decimal fineAmount = 1.0m;

    public static BinaryFormatter binaryFormatter = new BinaryFormatter();
    public static FileStream file;
    public static string[] fields = { "ID", "StudentId", "BookId", "LendDate", "ReturnDate","Status","ActualReturnedDate", "Fine(£)" };

    public static StreamWriter streamWriter;

    public static TextInfo textFormatter = new CultureInfo("en-US", false).TextInfo;
    private const string FileName = "borrow.bin";
    private const string BorrowedBooksListFileName = "borrowBooksId.txt";
    private static string TempFile = "TempBorrow.bin";

    private static int allowedBooks = 2;
    public static List<string> numberOfAvailableStudentId = new List<string>();
    public static StreamReader streamReader;
    public static string getFileName()
    {
        return FileName;
    }
    public static string getBorrowedBooksListFileName()
    {
        return BorrowedBooksListFileName;
    }
    public static int getAllowedPeriod()
    {
        Config.configDictionary = Config.loadConfigFile(Config.ConfigFilePath);
        allowedPeriod = Config.configDictionary.ContainsKey("AllowedPeriod") ? int.Parse(Config.configDictionary["AllowedPeriod"]) : allowedPeriod;

        return allowedPeriod;
    }
    public static int getNumberOfBooksAllowed()
    {
        Config.configDictionary = Config.loadConfigFile(Config.ConfigFilePath);
        allowedBooks = Config.configDictionary.ContainsKey("NumberOfBooksAllowed") ? int.Parse(Config.configDictionary["NumberOfBooksAllowed"]) : allowedBooks;

        return allowedBooks;
    }
    public static decimal getFineAmount()
    {
        Config.configDictionary = Config.loadConfigFile(Config.ConfigFilePath);
        fineAmount = Config.configDictionary.ContainsKey("FineAmount") ? decimal.Parse(Config.configDictionary["FineAmount"]) : fineAmount;

        return fineAmount;
    }
    public BorrowBooks(string studentId, string bookId)
    {
        setId();
        setStudentId(textFormatter.ToTitleCase(studentId));
        setBookId(bookId);
        setBorrowedDate();
        setReturnDate();
    }
    public void setStatus(string status)
    {
        this.status = status;
    }
    public string getStatus()
    {
        return this.status;
    }
    public decimal getFine()
    {
        return this.fine;
    }
    public string getActualReturnedDate()
    {
        return this.actualReturnedDate;
    }
    public void setActualReturnedDate()
    {
        this.actualReturnedDate = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
    }
    public string getStudentId()
    {
        return this.studentId;
    }
    public int getBookId()
    {
        return this.bookId;
    }
    public int getId()
    {
        return this.id;
    }
    public void setId()
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
    public void setStudentId(string studentId)
    {
        Students students = new Students();
        if(Users.idChecker(students.getFileName(), studentId))
        {
            this.studentId = studentId;
        }
        else
        {
            Layout.customiseWriteLine("Invalid Student Id");
        }

    }
    public void setBookId(string bookId)
    {
        if (Regex.Match(bookId, @"^[0-9]{1,}$").Success)
        {
            Books book = new Books();
            if (Books.idChecker(book.getFileName(), int.Parse(bookId)))
            {
                this.bookId = int.Parse(bookId);
            }
            else
            {
                Layout.customiseWriteLine("Invalid Book Id");
            }
        }
        else
        {
            Layout.customiseWriteLine("Invalid Book Id");
        }
    }
    public void setReturnDate()
    {
        allowedPeriod = getAllowedPeriod();
        string borrowedDate = getBorrowedDate();
        DateTime borrowdate = DateTime.Parse(borrowedDate);
        string returnDate = borrowdate.AddDays(allowedPeriod).ToString();
        this.returnDate = DateTime.Parse(returnDate).ToString("dd/MM/yyyy");
    }
    public void setBorrowedDate()
    {
        this.borrowedDate = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
    }
    public string getBorrowedDate()
    {
        return this.borrowedDate;
    }
    public string getReturnDate()
    {
        return this.returnDate;
    }
    //auto increase id
    public static int selectLastId()
    {
        file = File.Open(FileName, FileMode.Open, FileAccess.Read);
        DataTables.clearDataTable();
        DataTables.dataTableStructure(fields, "int");
        while (file.Position != file.Length)
        {
            try
            {
                BorrowBooks borrowBooks = (BorrowBooks)binaryFormatter.Deserialize(file);
                DataTables.dataTable.Rows.Add(borrowBooks.id, borrowBooks.studentId, borrowBooks.bookId, borrowBooks.borrowedDate, borrowBooks.returnDate, borrowBooks.status, borrowBooks.actualReturnedDate, borrowBooks.fine);
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
                    lastSavedId = int.Parse((string)row[field].ToString());
                    break;
                }
                break;

            }
        }

        file.Close();
        return lastSavedId;

    }
    public static void getDetails(string status)
    {
        Layout.customiseWrite("Enter the Student ID: ");
        string studentId = Console.ReadLine();

        Layout.customiseWrite("Enter the Book ID: ");
        string bookId = Console.ReadLine();

        if(status == "Lended")
        {
            addBorrowBooks(studentId, bookId);
        }
        else if(status == "Return")
        {
            searchRecord(studentId, bookId);
        }
    }
    public static void registerBorrowBook()
    {
        clearConsole();

        Layout.headerTitle("Lend the Books to Students");
        getDetails("Lended");
    }
    public static void registerReturnBooks()
    {
        clearConsole();

        Layout.headerTitle("Returns Book from Students"); 
        getDetails("Return");

    }
    public static void searchRecord(string studentId, string bookId)
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
                    BorrowBooks borrowBooksData = (BorrowBooks)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(borrowBooksData.getId(), borrowBooksData.getStudentId(), borrowBooksData.getBookId(), borrowBooksData.getBorrowedDate(), borrowBooksData.getReturnDate(), borrowBooksData.getStatus(), borrowBooksData.getActualReturnedDate(), borrowBooksData.getFine());
                }
                catch (Exception e)
                {
                    Layout.customiseWriteLine(e.Message);
                    Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                    break;
                }
                

            }
            if (DataTables.dataTable.Select("StudentId = '" + studentId + "' AND BookId = '" + bookId + "' AND Status = 'Lended'").Length > 0)
            {
                DataTables.dataTableHeader(fields);
            }

            DataTables.dataTableBody("StudentId = '" + studentId + "' AND BookId = '" + bookId + "' AND Status = 'Lended'", "", fields);

            int recordId = 0;

            if (DataTables.dataTable.Select("StudentId = '" + studentId + "' AND BookId = '" + bookId + "' AND Status = 'Lended'").Length > 0)
            {
                foreach (DataRow row in DataTables.dataTable.Select("StudentId = '" + studentId + "' AND BookId = '" + bookId + "' AND Status = 'Lended'"))
                {
                    recordId = int.Parse((string)row["ID"]);
                }
                updateDetails(FileName, recordId);
            }
            else
            {
                Layout.customiseWriteLine("No Record Found for Returning the Book");
            }

            file.Close();
        }
    }
    public static void updateDetails(string fileName, int id)
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
            BorrowBooks borrowBooks = (BorrowBooks)binaryFormatter.Deserialize(file);
            if (borrowBooks.id == id)
            {
                borrowBooks.status = "Returned";
                borrowBooks.setActualReturnedDate();
                borrowBooks.actualReturnedDate = borrowBooks.getActualReturnedDate();

                //calculate fine amount when return
                DateTime convertedReturnedDate = DateTime.Parse(borrowBooks.returnDate);
                DateTime convertedActualReturnedDate = DateTime.Parse(borrowBooks.actualReturnedDate);

                double numberOfDaysDifferent = (convertedActualReturnedDate - convertedReturnedDate).TotalDays;
                getFineAmount();
                if (numberOfDaysDifferent > 0)
                {
                    borrowBooks.fine = (decimal)numberOfDaysDifferent * fineAmount;
                }

                List<string> borrowedbookslist = File.ReadAllLines(BorrowedBooksListFileName).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
                borrowedbookslist.Remove(borrowBooks.studentId + "," + borrowBooks.bookId);

                File.WriteAllLines(BorrowedBooksListFileName, borrowedbookslist);

                binaryFormatter.Serialize(files, borrowBooks);
                Console.WriteLine();
                Layout.customiseWriteLine("Books Returned Successfully");

                continue;

            }
            binaryFormatter.Serialize(files, borrowBooks);
        }
        file.Close();
        files.Close();
        File.Delete(FileName);
        File.Move(TempFile, FileName);
    }
    public static void addBorrowBooks(string studentId, string bookId)
    {
        BorrowBooks borrowBooks = new BorrowBooks(studentId, bookId);
        storeDetails(FileName, borrowBooks);
    }

    //Verify the number of books allowed for the student to borrow at the same time
    public static bool verifyAllowedBooks(string studentId)
    {
        getNumberOfBooksAllowed();

        string BorrowedBooksListFileName = BooksAvailablility.getFileName();

        if (File.Exists(BorrowedBooksListFileName))
        {
            streamReader = new StreamReader(BorrowedBooksListFileName);

            while (!streamReader.EndOfStream)
            {
                string[] records = streamReader.ReadLine().Split(',');

                numberOfAvailableStudentId.Add(records[0]);
            }
            streamReader.Close();
        }

        var result = Enumerable.Range(0, numberOfAvailableStudentId.Count)
             .Where(i => numberOfAvailableStudentId[i] == studentId).ToArray();
       
        if (result.Length >= allowedBooks)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static void storeDetails(string fileName, BorrowBooks borrowBooks)
    {
        string returnDate = borrowBooks.getReturnDate();
        int id = borrowBooks.getId();
        bool availability = BooksAvailablility.availability(borrowBooks.bookId);
        bool bookLimit = verifyAllowedBooks(borrowBooks.studentId);
        borrowBooks.status = "Lended";
        borrowBooks.actualReturnedDate = null;
        borrowBooks.fine = decimal.Parse("0.00");

        if (availability)
        {
            if(bookLimit)
            {
                if (borrowBooks.id > 0 && borrowBooks.studentId != "" && borrowBooks.bookId != 0 && borrowBooks.returnDate != ""&& borrowBooks.borrowedDate != "")
                {
                    Layout.customiseWrite("Are you sure do you want to save (Y/N)? ");
                    string confirmStore = Console.ReadLine().ToLower();
                    if (confirmStore == "y")
                    {

                        if (File.Exists(FileName))
                        {
                            DataTables.clearDataTable();
                            DataTables.dataTableStructure(fields);
                            //Layout.headerTitle("");

                            file = File.Open(FileName, FileMode.Open, FileAccess.Read);

                            while (file.Position != file.Length)
                            {
                                try
                                {
                                    BorrowBooks borrowBooksData = (BorrowBooks)binaryFormatter.Deserialize(file);
                                    DataTables.dataTable.Rows.Add(borrowBooksData.id, borrowBooksData.studentId, borrowBooksData.bookId, borrowBooksData.borrowedDate, borrowBooksData.returnDate, borrowBooks.status, borrowBooks.actualReturnedDate, borrowBooks.fine);
                                }
                                catch (Exception e)
                                {
                                    Layout.customiseWriteLine(e.Message);
                                    Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                                    break;
                                }
                                

                            }
                            if(DataTables.dataTable.Select("ID = '" + id + "'").Length > 0)
                            {
                                DataTables.dataTableHeader(fields);
                            }
                            DataTables.dataTableBody("ID = '" + id + "'", "", fields);
                            file.Close();
                        }

                        if (File.Exists(FileName))
                        {
                            file = File.Open(FileName, FileMode.Append);
                            if (DataTables.dataTable.Select("ID = '" + id + "'").Length == 0)
                            {
                                binaryFormatter.Serialize(file, borrowBooks);
                                file.Close();

                                streamWriter = new StreamWriter(BorrowedBooksListFileName,append:true);
                                streamWriter.WriteLine(borrowBooks.studentId + ","+borrowBooks.bookId);
                                streamWriter.Close();
                                Console.WriteLine();
                                Layout.customiseWriteLine("Books Lend Successfully");
                                // Console.Clear();
                                //Program.Main();
                            }
                            else
                            {
                                Console.WriteLine();
                                Layout.customiseWriteLine("Sorry book already exist");
                                file.Close();
                                Program.Main();
                            }
                        }
                        else
                        {
                            file = File.Create(FileName);
                            binaryFormatter.Serialize(file, borrowBooks);
                            file.Close();
                            streamWriter = new StreamWriter(BorrowedBooksListFileName, append: true);
                            streamWriter.WriteLine(borrowBooks.studentId + "," + borrowBooks.bookId);
                            streamWriter.Close();
                            Console.WriteLine();
                            Layout.customiseWriteLine("Books Lend Successfully");

                        }

                    }

                }
                else
                {
                    Layout.customiseWriteLine("Please check your inputs!");
                    //file.Close();
                    //getBooksDetails();
                }
            }
            else
            {
                Layout.customiseWriteLine("Return the previous lended books!");
            }
        }
        else
        {
            Layout.customiseWriteLine("Selected Book Not Availabile to Lend!");
        }
    }
    public static void viewDetails(string FileName, string status, string title)
    {
        if (File.Exists(FileName))
        {
            file = File.Open(FileName, FileMode.Open, FileAccess.Read);
            DataTables.clearDataTable();
            DataTables.dataTableStructure(fields,"int");
            if(title != "")
            {
                Layout.headerTitle("List All the "+ title + " History Details");
            }
            else
            {
                Layout.headerTitle("List All the Lending and Return History Details");
            }
            DataTables.dataTableHeader(fields);

            while (file.Position != file.Length)
            {
                try
                {
                    BorrowBooks borrowBooks = (BorrowBooks)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(borrowBooks.id, borrowBooks.studentId, borrowBooks.bookId, borrowBooks.borrowedDate, borrowBooks.returnDate, borrowBooks.status, borrowBooks.actualReturnedDate, borrowBooks.fine);
                }
                catch (Exception e)
                {
                    Layout.customiseWriteLine(e.Message);
                    Layout.customiseWriteLine("Your file was corrupted. Please check it. Otherwise your changes wouldn't be saved!");
                    break;
                }
               

            }
            if (status != "")
            {
                if (DataTables.dataTable.Select("Status = '" + status + "'", "").Length == 0)
                {
                    Layout.customiseWriteLine("No Records Found!");
                }
            }
            else
            {
                if (DataTables.dataTable.Select("", "").Length == 0)
                {
                    Layout.customiseWriteLine("No Records Found!");
                }
            }
           
            Layout.customiseWrite("Are you want to sort and view the above table? (Y/N) ");
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
                if (status != "")
                {
                    DataTables.dataTableBody("Status = '" + status + "'", sortField + " " + sortOrder, fields);
                }
                else
                {
                    DataTables.dataTableBody("", sortField + " " + sortOrder, fields);
                }
            }
            else
            {
                if (status != "")
                {
                    DataTables.dataTableBody("Status = '" + status + "'", "", fields);
                }
                else
                {
                    DataTables.dataTableBody("", "", fields);
                }
            }

            file.Close();
        }
        else
        {
            Layout.customiseWriteLine("File is Missing!");
        }

    }
    public static void viewReturnDetails()
    {
        viewDetails(FileName, "Returned", "Return");
    }
    public static void viewBorrowReturnDetails()
    {
        clearConsole();
        viewDetails(FileName, "", "");
    }
    public static void viewLendingDetails()
    {
        viewDetails(FileName, "Lended", "Lending");
    }
    public static void clearConsole()
    {
        Console.Clear();
        Program.customise();
    }
    public static void viewBorrowBooksHistory()
    {
        clearConsole();
        viewLendingDetails();
    }
    public static void viewReturnBooksHistory()
    {
        clearConsole();
        viewReturnDetails();
    }
    public static void searchBorrowHistory()
    {
        clearConsole();
        searchBorrowDetails();
    }
    public static void searchReturnHistory()
    {
        clearConsole();
        searchReturnDetails();
    }
    public static void searchDetails(string FileName, string status, string title)
    {
        if (File.Exists(FileName))
        {
            file = File.Open(FileName, FileMode.Open, FileAccess.Read);

            DataTables.clearDataTable();
            DataTables.dataTableStructure(fields,"int");

            if (title != "")
            {
                Layout.headerTitle("Search the " + title + " History Details");
            }
            else
            {
                Layout.headerTitle("Search All the Lending and Return History Details");
            }

            DataTables.dataTableHeader(fields);


            while (file.Position != file.Length)
            {
                try
                {
                    BorrowBooks borrowBooks = (BorrowBooks)binaryFormatter.Deserialize(file);
                    DataTables.dataTable.Rows.Add(borrowBooks.id, borrowBooks.studentId, borrowBooks.bookId, borrowBooks.borrowedDate, borrowBooks.returnDate, borrowBooks.status, borrowBooks.actualReturnedDate, borrowBooks.fine);
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
            if (status != "")
            {
                if (DataTables.dataTable.Select(searchField + " = '" + searchValue + "' AND Status = '" + status + "'").Length == 0)
                {
                    Layout.customiseWriteLine("No Records Found!");
                }
                DataTables.dataTableBody(searchField + " = '" + searchValue + "' AND Status = '" + status + "'", "", fields);
            }
            else
            {
                if (DataTables.dataTable.Select(searchField + " = '" + searchValue + "'").Length == 0)
                {
                    Layout.customiseWriteLine("No Records Found!");
                }
                DataTables.dataTableBody(searchField + " = '" + searchValue + "'", "", fields);
            }
           

            file.Close();

        }
        else
        {
            Layout.customiseWriteLine("File is Missing!");
        }
    }
    public static void searchBorrowDetails()
    {
        searchDetails(FileName, "Lended", "Lemding");
    }
    public static void searchReturnDetails()
    {
        searchDetails(FileName,"Returned","Return");
    }
    public static void searchBorrowReturnDetails()
    {
        clearConsole();
        searchDetails(FileName, "", "");
    }
}

