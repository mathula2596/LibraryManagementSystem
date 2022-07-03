using System;
using System.Linq;

class Menu
{
    public static void mainMenu()
    {
        Console.Clear();
        Program.customise();
        Layout.headerTitle("Main Menu");


        string[] menu = { "Students", "Books", "Lend Books", "Return Books", "Lending and Return History", "Availability","Logout" };
        string[] listSubMenu = { "Add", "List", "Edit", "Delete", "Search", "Back from" };
        int indexToRemove;

        for (int i = 0; i<menu.Length; i++)
        {
            Console.WriteLine("{0,10} - {1,-10}", i + 1  , menu[i]);
		}

        int selectedMenuOption = selectMenu(menu);

        switch (selectedMenuOption)
        {
            case 1:
               
                subMenu(menu, selectedMenuOption, listSubMenu);
                break;
            case 2:
                
                subMenu(menu, selectedMenuOption, listSubMenu);
                break;
            case 3:
                // Remove unwanted sub menu for the particular main menu
                indexToRemove = 2;
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                subMenu(menu, selectedMenuOption, listSubMenu);

                break;
            case 4:
                indexToRemove = 2;
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                subMenu(menu, selectedMenuOption, listSubMenu);
                break;
            case 5:
                indexToRemove = 0;
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                indexToRemove = 1;
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                listSubMenu = listSubMenu.Where((source, index) => index != indexToRemove).ToArray();
                subMenu(menu, selectedMenuOption, listSubMenu);
                break;
            case 6:
                Console.Clear();
                Program.customise();
                BooksAvailablility.bookAvailabilityCheck();
                Console.ReadKey();
                Console.Clear();
                mainMenu();
                break;
            case 7:
                Console.Clear();
                Program.customise();
                Program.Main();
                break;

            default:
                Layout.customiseWriteLine("Please choose the correct menu!");
                mainMenu();
                break;
        }
        
    }

    public static void subMenu(string [] mainMenuName, int selectedMainMenuOption, string [] listSubMenu )
    {
        //Clear the screen and display the library name
        Console.Clear();
        Program.customise();
        Layout.headerTitle("Sub Menu");


        string[] menu = listSubMenu;

        for (int i = 0; i < menu.Length; i++)
        {
            Console.WriteLine("{0,10} - {1,-10}", i + 1, menu[i]+ " " + mainMenuName[selectedMainMenuOption - 1]);
        }

        int selectedMenuOption = selectMenu(menu);

        switch (selectedMenuOption)
        {
            case 1:
                if(mainMenuName[selectedMainMenuOption - 1] == "Students")
                {
                    Students.getStudentDetails();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Books")
                {
                    Books.getBooksDetails();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Lend Books")
                {
                    BorrowBooks.registerBorrowBook();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Return Books")
                {
                    BorrowBooks.registerReturnBooks();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Lending and Return History")
                {
                    BorrowBooks.viewBorrowReturnDetails();
                }
                break;
            case 2:
                if (mainMenuName[selectedMainMenuOption - 1] == "Students")
                {
                    Students.viewStudent();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Books")
                {
                    Books.viewBooks();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Lend Books")
                {
                    BorrowBooks.viewBorrowBooksHistory();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Return Books")
                {
                    BorrowBooks.viewReturnBooksHistory();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Lending and Return History")
                {
                    BorrowBooks.searchBorrowReturnDetails();
                }
                break;
            case 3:
                if (mainMenuName[selectedMainMenuOption - 1] == "Students")
                {
                    Students.editStudent();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Books")
                {
                    Books.editBook();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Lend Books")
                {
                    BorrowBooks.searchBorrowHistory();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Return Books")
                {
                    BorrowBooks.searchReturnHistory();
                }
                break;
            case 4:
                if (mainMenuName[selectedMainMenuOption - 1] == "Students")
                {
                    Students.deleteStudent();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Books")
                {
                    Books.deleteBook();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Lend Books" || mainMenuName[selectedMainMenuOption - 1] == "Return Books")
                {
                    mainMenu();
                    break;
                }
                break;
            case 5:
                if (mainMenuName[selectedMainMenuOption - 1] == "Students")
                {
                    Students.searchStudent();
                }
                else if (mainMenuName[selectedMainMenuOption - 1] == "Books")
                {
                    Books.searchBooks();
                }
               
                break;
            case 6:
                mainMenu();
                break;
            default:
                Layout.customiseWriteLine("Please choose the correct menu!");
                mainMenu();
                break;
        }
        Console.ReadKey();
        mainMenu();

    }

    public static int selectMenu(string [] menu)
    {
        int defaultSelection = 0;
        int selectedOption = 0;

        bool validate = false;
        int validityCount = 0;
        do
        {
            if(validityCount > 0)
            {
                Layout.customiseWrite("Sorry please check your input option number!\n");
            }
            Layout.customiseWrite("Choose the menu by selecting the number from 1 - " + menu.Length + ": ");
            validate = int.TryParse(Console.ReadLine(), out defaultSelection);
            validityCount++;

        } while (!validate || defaultSelection > menu.Length || defaultSelection <= 0);

        if(validate)
        {
            selectedOption = defaultSelection;
        }
        
        return selectedOption;
    }

	
}
