using System.Collections;
using System.Data;
using System.Data.SQLite;
using System;

namespace StockcheckDatabase
{
    public static class SQLiteDatabaseAccess
    {
        public static DataTable LoadDataTable(string tableName)
        {
            SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=" + "C:\\Users\\chochheim\\Documents\\CSharpProjects\\StockcheckSQLiteDatabase\\bin\\Debug\\netcoreapp3.1\\database\\Deutschland.db");

            sQLiteConnection.Open();
            DataTable table;
            try
            {
                SQLiteDataAdapter sQLiteAdapter = new SQLiteDataAdapter("SELECT * FROM [" + tableName + "]", sQLiteConnection);
                table = new DataTable();
                sQLiteAdapter.Fill(table);
                new SQLiteCommandBuilder(sQLiteAdapter);
            }
            catch (System.Exception)
            {
                sQLiteConnection.Close();
                return null;
            }
            sQLiteConnection.Close();

            table.TableName = tableName;
            return ConvenienceTableFromDatabaseTable(table);
        }

        public static DataTable CreateTable(string tablename)
        {
            SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=" + "C:\\Users\\chochheim\\Documents\\CSharpProjects\\StockcheckSQLiteDatabase\\bin\\Debug\\netcoreapp3.1\\database\\Deutschland.db");

            sQLiteConnection.Open();
            try
            {
                using (SQLiteCommand mCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS ["
                    + tablename + "] " +
                    "('year' INTEGER," +
                    " 'earning_per_share' FLOAT," +
                    " 'div_per_share' FLOAT);",
                    sQLiteConnection))
                {
                    int result=mCmd.ExecuteNonQuery();
                }
            }
            catch(System.Exception)
            {
                 sQLiteConnection.Close();
            }
            sQLiteConnection.Close();

            return LoadDataTable(tablename);
        }

        public static ArrayList GetTableNames()
        {
            SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=" + "C:\\Users\\chochheim\\Documents\\CSharpProjects\\StockcheckSQLiteDatabase\\bin\\Debug\\netcoreapp3.1\\database\\Deutschland.db");
            ArrayList itemsList = new ArrayList();

            sQLiteConnection.Open();
            using (DataTable mTables = sQLiteConnection.GetSchema("Tables"))
            {
                for (int i = 0; i < mTables.Rows.Count; i++)
                {
                    itemsList.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                }
            }
            sQLiteConnection.Close();

            return itemsList;
        }

        public static bool SaveTable(DataTable table)
        {
            Execute(string.Format("DELETE FROM {0}", table.TableName));

            SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=" + "C:\\Users\\chochheim\\Documents\\CSharpProjects\\StockcheckSQLiteDatabase\\bin\\Debug\\netcoreapp3.1\\database\\Deutschland.db");

            sQLiteConnection.Open();
            try
            {
                var cmd = sQLiteConnection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM [{0}]", table.TableName);
                SQLiteDataAdapter _sQLiteAdapter = new SQLiteDataAdapter(cmd);
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(_sQLiteAdapter);
                builder.GetInsertCommand();

                var ret = _sQLiteAdapter.Update(DatabaseTableFromConvenienceTable(table));
                sQLiteConnection.Close();
                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                sQLiteConnection.Close();
                return false;
            }
        }

        public static bool DeleteTable(string tablename)
        {
            SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=" + "C:\\Users\\chochheim\\Documents\\CSharpProjects\\StockcheckSQLiteDatabase\\bin\\Debug\\netcoreapp3.1\\database\\Deutschland.db");

            sQLiteConnection.Open();
            try
            {
                string query = string.Format("DROP TABLE '{0}'", tablename);
                using (SQLiteCommand mCmd = new SQLiteCommand(query, sQLiteConnection))
                {
                    int result = mCmd.ExecuteNonQuery();
                    if (result == -1) return false;
                }
            }
            catch (System.Exception)
            {
                sQLiteConnection.Close();
                return false;
            }
            sQLiteConnection.Close();
            return true;
        }

        private static DataTable ConvenienceTableFromDatabaseTable(DataTable databaseTable)
        {
            var convenienceTable = new DataTable();

            convenienceTable.Columns.Add("Jahr", typeof(int));
            convenienceTable.Columns.Add("Gewinn pro Aktie", typeof(float));
            convenienceTable.Columns.Add("Dividende pro Aktie", typeof(float));

            for (int i = 0; i < databaseTable.Rows.Count; i++)
            {
                convenienceTable.Rows.Add();
                convenienceTable.Rows[i]["Jahr"] = databaseTable.Rows[i]["year"];
                convenienceTable.Rows[i]["Gewinn pro Aktie"] = databaseTable.Rows[i]["earning_per_share"];
                convenienceTable.Rows[i]["Dividende pro Aktie"] = databaseTable.Rows[i]["div_per_share"];
            }

            convenienceTable.TableName = databaseTable.TableName;
            return convenienceTable;
        }
        private static DataTable DatabaseTableFromConvenienceTable(DataTable databaseTable)
        {
            var convenienceTable = new DataTable();

            convenienceTable.Columns.Add("year", databaseTable.Columns["Jahr"].DataType);
            convenienceTable.Columns.Add("earning_per_share", databaseTable.Columns["Gewinn pro Aktie"].DataType);
            convenienceTable.Columns.Add("div_per_share", databaseTable.Columns["Dividende pro Aktie"].DataType);

            for (int i = 0; i < databaseTable.Rows.Count; i++)
            {
                convenienceTable.Rows.Add();
                convenienceTable.Rows[i]["year"] = databaseTable.Rows[i]["Jahr"];
                convenienceTable.Rows[i]["earning_per_share"] = databaseTable.Rows[i]["Gewinn pro Aktie"];
                convenienceTable.Rows[i]["div_per_share"] = databaseTable.Rows[i]["Dividende pro Aktie"];
            }

            return convenienceTable;
        }

        private static int Execute(string sql_statement)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=" + "C:\\Users\\chochheim\\Documents\\CSharpProjects\\StockcheckSQLiteDatabase\\bin\\Debug\\netcoreapp3.1\\database\\Deutschland.db");

            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = sql_statement;
            int row_updated;
            try
            {
                row_updated = cmd.ExecuteNonQuery();
            }
            catch
            {
                con.Close();
                return 0;
            }
            con.Close();
            return row_updated;
        }
    }
}
