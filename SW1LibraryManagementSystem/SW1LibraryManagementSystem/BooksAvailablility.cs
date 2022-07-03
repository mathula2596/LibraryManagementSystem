using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class BooksAvailablility
{
    private const string BorrowedBooksListFileName = "borrowBooksId.txt";

    public static StreamReader streamReader;

    public static List<int> booksId = new List<int>();

    // Check book availability for lending
    public static bool availability(int bookId)
    {
        if (File.Exists(BorrowedBooksListFileName))
        {
            streamReader = new StreamReader(BorrowedBooksListFileName);

            while (!streamReader.EndOfStream)
            {
                string [] records = streamReader.ReadLine().Split(',');

                booksId.Add(int.Parse(records[1]));
            }
            streamReader.Close();
        }

        if(booksId.Contains(bookId))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static string getFileName()
    {
        return BorrowedBooksListFileName;
    }

    public static int setBookId(string bookId)
    {
        if (Regex.Match(bookId, @"^[0-9]{1,}$").Success)
        {
            Books book = new Books();
            if (Books.idChecker(book.getFileName(), int.Parse(bookId)))
            {
               return int.Parse(bookId);
            }
            else
            {
                Layout.customiseWriteLine("Invalid Book Id");
            }
        }
        else
        {
            Console.WriteLine();
            Layout.customiseWriteLine("Invalid Book Id");
        }
        return 0;
    }

    // Check the book availability by the ID
    public static void bookAvailabilityCheck()
    {
        Layout.headerTitle("Check the Availability of the Books");
        Books book = new Books();
        Layout.customiseWrite("Enter the Book Id to check for the availability: ");
        string bookId = Console.ReadLine();
        int bookIdConverted = setBookId(bookId);
        bool bookAvailability = availability(bookIdConverted);
        if(bookIdConverted !=0 )
        {
            Console.WriteLine();
            if (bookAvailability)
            {
                Layout.customiseWriteLine("The requested book is available to lend");
            }
            else
            {
                Layout.customiseWriteLine("The requested book is unavailable to lend");
            }
        }

    }
}
