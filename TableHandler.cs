using System;
using System.Collections;
using System.Data;
using StockcheckDatabase;

public class TableHandler
{
    public DataTable CreateTable(string tableName)
    {
        return SQLiteDatabaseAccess.CreateTable("AAA");
    }
    public void DeleteTable(DataTable table)
    {
        SQLiteDatabaseAccess.DeleteTable(table.TableName);
    }
    public void SaveTable(DataTable table)
    {
        SQLiteDatabaseAccess.SaveTable(table);
    }

    public ArrayList GetTableNames()
    {
        return SQLiteDatabaseAccess.GetTableNames();
    }

    public DataTable LoadDataTable(string tableName)
    {
        return SQLiteDatabaseAccess.LoadDataTable(tableName);
    }
    public void DeleteRow(DataTable table, int year)
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
    public void AddRow(ref DataTable table)
    {
        if (!AddRowInBetween(ref table))
        {
            AddRowAfterLastYear(ref table);
        }
    }
    public void AddRowAfterLastYear(ref DataTable table)
    {
        int maxYear = Convert.ToInt32(table.Compute("max([Jahr])", string.Empty));

        DataRow row = table.NewRow();
        row["Jahr"] = maxYear + 1;
        table.Rows.Add(row);
    }

    public void AddRowBeforeEarliestYear(ref DataTable table)
    {
        int minYear = Convert.ToInt32(table.Compute("min([Jahr])", string.Empty));

        DataRow row = table.NewRow();
        row["Jahr"] = minYear - 1;
        table.Rows.Add(row);
    }

    // Fügt eine Zeile hinzu, wenn es eine "Lücke" in den Jahren gibt.
    // Gibt es mehrere "Lücken", so wird die früheste Lücke (kleinst möglichstes Jahr) befüllt.
    // return true: es gab eine Lücke und es wurde eine Zeile zugefügt
    // return false: es gab keine Lücke und es wurde keine Zeile zugefügt
    private bool AddRowInBetween(ref DataTable table)
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
                yearBeforeGap = (int)table.Rows[i - 1]["Jahr"];
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
}