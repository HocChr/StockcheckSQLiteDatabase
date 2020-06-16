using System;
using System.Collections;
using StockcheckDatabase;
using System.Data;

namespace StockcheckSQLiteDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabaseAccess.DeleteTable("AAA");
            DataTable newTable = SQLiteDatabaseAccess.CreateTable("AAA");
            newTable.Rows.Add();
            newTable.Rows.Add();
            newTable.Rows[0]["Jahr"] = 2020;

            newTable.Rows[1]["Jahr"] = 2009;

            Console.WriteLine("Tabelle vorher:");
            PrintTable(newTable);

            AddRowInBetween(ref newTable);
            AddRowInBetween(ref newTable);
            AddRowInBetween(ref newTable);

            Console.WriteLine("Tabelle sortiert:");
            newTable.DefaultView.Sort = "Jahr Asc";
            newTable = newTable.DefaultView.ToTable();
            PrintTable(newTable);

            Console.WriteLine("\nTabelle speichern.\n");

            SQLiteDatabaseAccess.SaveTable(newTable);

            Console.WriteLine("\nTabelle ausgeben.\n");

            var tableList = SQLiteDatabaseAccess.GetTableNames();
            PrintTableList(tableList);
            Console.WriteLine("AAA");
            PrintTable(SQLiteDatabaseAccess.LoadDataTable("AAA"));

            Console.WriteLine("\nZeile löschen.\n");
            DeleteRow(newTable, 2020);

            Console.WriteLine("\nTabelle speichern.\n");

            SQLiteDatabaseAccess.SaveTable(newTable);

            Console.WriteLine("\nTabelle ausgeben.\n");
            PrintTable(SQLiteDatabaseAccess.LoadDataTable("AAA"));

            Console.WriteLine("\nTabelle wieder löschen.\n");

            SQLiteDatabaseAccess.DeleteTable("AAA");
            tableList = SQLiteDatabaseAccess.GetTableNames();
            PrintTableList(tableList);
        }

        static void DeleteRow(DataTable table, int year)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow dr = table.Rows[i];
                if ((int)dr["Jahr"] == year)
                    dr.Delete();
            }
            table.AcceptChanges();
        }

        // Bevorzugt wird eine "Lücke" befüllt. Gibt es keine "Lücke", wird am "Ende" angefügt.
        static void AddRow(ref DataTable table)
        {
            if(!AddRowInBetween(ref table))
            {
                AddRowAfterLastYear(ref table);
            }
        }

        // Fügt eine Zeile hinzu, wenn es eine "Lücke" in den Jahren gibt.
        // Gibt es mehrere "Lücken", so wird die früheste Lücke (kleinst möglichstes Jahr) befüllt.
        // return true: es gab eine Lücke und es wurde eine Zeile zugefügt
        // return false: es gab keine Lücke und es wurde keine Zeile zugefügt
        static bool AddRowInBetween(ref DataTable table)
        {
            table.DefaultView.Sort = "Jahr Asc";
            table = table.DefaultView.ToTable();

            int minYear = Convert.ToInt32(table.Compute("min([Jahr])", string.Empty));

            // ermittle die erste Lücke
            int yearBeforeGap = -1;
            for (int i = 1; i < table.Rows.Count; i++)
            {
                int yearBefore = (int)table.Rows[i - 1]["Jahr"];
                if ((int)table.Rows[i]["Jahr"] != yearBefore + 1)
                {
                    yearBeforeGap = (int)table.Rows[i-1]["Jahr"];
                    break;
                }
            }

            if (yearBeforeGap != -1)
            {
                DataRow row = table.NewRow();
                row["Jahr"] = yearBeforeGap + 1;
                table.Rows.Add(row);

                return true;
            }

            return false;
        }

        static void AddRowAfterLastYear(ref DataTable table)
        {
            int maxYear = Convert.ToInt32(table.Compute("max([Jahr])", string.Empty));

            DataRow row = table.NewRow();
            row["Jahr"] = maxYear + 1;
            table.Rows.Add(row);
        }

        static void AddRowBeforeEarliestYear(ref DataTable table)
        {
            int minYear = Convert.ToInt32(table.Compute("min([Jahr])", string.Empty));

            DataRow row = table.NewRow();
            row["Jahr"] = minYear - 1;
            table.Rows.Add(row);
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
