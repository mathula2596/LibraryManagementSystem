using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class DataTables
{
    public static BinaryFormatter binaryFormatter = new BinaryFormatter();
    public static DataTable dataTable = new DataTable();

    //Used to create the table outline
    public static void dataTableStructure(string[] menus, string types = "string")
    {
        //https://www.c-sharpcorner.com/UploadFile/0f68f2/querying-a-data-table-using-select-method-and-lambda-express/
        int count = 0;
        foreach (var menu in menus)
        {
            if(types == "int")
            {
                if(count == 0)
                {
                    dataTable.Columns.Add(menu,typeof(int));
                }
                else
                {
                    dataTable.Columns.Add(menu, typeof(string));
                }
            }
            else
            {
                dataTable.Columns.Add(menu, typeof(string));
            }
            count++;

        }
    }

    // Display table heading
    public static void dataTableHeader(string[] menus)
    {
        int count = 0;
        foreach (var menu in menus)
        {
            if(count == 0)
            {
                Console.Write("\t");
            }
            dataTable.Columns[menu].ReadOnly = true;
            Console.Write("{0,-20}", dataTable.Columns[menu]);
            count++;
        }

        Console.WriteLine();
    }

    // Display saved data in table body
    public static void dataTableBody(string condition, string sort, string [] fields)
    {
        int count = 0;
        foreach (DataRow row in dataTable.Select(condition, sort))
        {
            Console.Write("\t");
            foreach (var field in fields)
            {
                Console.Write("{0,-20}",row[field] );
                count++;
            }
            Console.WriteLine();
        }
    }

    // Clear table structure
    public static void clearDataTable()
    {
        dataTable.Columns.Clear();
        dataTable.Clear();
    }
}
