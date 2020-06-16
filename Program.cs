using System;
using System.Collections;
using System.Data;

namespace StockcheckSQLiteDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            TableHandler tableHandler = new TableHandler();

            DataTable newTable = tableHandler.CreateTable("AAA");
            newTable.Rows.Add();
            newTable.Rows.Add();
            newTable.Rows[0]["Jahr"] = 2020;

            newTable.Rows[1]["Jahr"] = 2009;

            Console.WriteLine("Tabelle vorher:");
            PrintTable(newTable);

            tableHandler.AddRow(ref newTable);
            tableHandler.AddRow(ref newTable);
            tableHandler.AddRow(ref newTable);

            Console.WriteLine("Tabelle sortiert:");
            newTable.DefaultView.Sort = "Jahr Asc";
            newTable = newTable.DefaultView.ToTable();
            PrintTable(newTable);

            Console.WriteLine("\nTabelle speichern.\n");

            tableHandler.SaveTable(newTable);

            Console.WriteLine("\nTabelle ausgeben.\n");

            var tableList = tableHandler.GetTableNames();
            PrintTableList(tableList);
            Console.WriteLine("AAA");
            PrintTable(tableHandler.LoadDataTable("AAA"));

            Console.WriteLine("\nZeile löschen.\n");
            tableHandler.DeleteRow(newTable, 2020);

            Console.WriteLine("\nTabelle speichern.\n");

            tableHandler.SaveTable(newTable);

            Console.WriteLine("\nTabelle ausgeben.\n");
            PrintTable(tableHandler.LoadDataTable("AAA"));

            Console.WriteLine("\nTabelle wieder löschen.\n");

            tableHandler.DeleteTable(newTable);
            tableList = tableHandler.GetTableNames();
            PrintTableList(tableList);
        }



        static void PrintTableList(ArrayList tableList)
        {
            foreach (var table in tableList)
            {
                Console.WriteLine(table);
            }
        }

        static void PrintTable(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Console.Write("Jahr: ");
                Console.Write(table.Rows[i]["Jahr"]);
                Console.Write(" Gewinn pro Aktie: ");
                Console.Write(table.Rows[i]["Gewinn pro Aktie"]);
                Console.Write(" Dividende pro Aktie: ");
                Console.Write(table.Rows[i]["Dividende pro Aktie"]);
                Console.Write("\n");
            }
        }
    }
}
