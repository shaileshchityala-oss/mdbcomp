using System;
using System.Data;
using System.Data.OleDb;

namespace NMdb
{
    class MdbComparer
    {
        static string GetTypeName(int dataType)
        {
            switch (dataType)
            {
                case 2: return "SmallInt";
                case 3: return "Integer";
                case 4: return "Single";
                case 5: return "Double";
                case 6: return "Currency";
                case 7: return "Date";
                case 11: return "Boolean";
                case 17: return "Byte";
                case 72: return "GUID";
                case 128: return "Binary";
                case 130: return "Text(Unicode)";
                case 131: return "Decimal";
                case 202: return "Text(ANSI)";
                case 203: return "Memo";
                default: return "Unknown(" + dataType + ")";
            }
        }

        static bool IsTextType(int t)
        {
            return (t == 130 || t == 202); // WChar and VarChar = TEXT
        }

        //static DataTable GetColumns(string mdbPath)
        //{
        //    string connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";";
        //    using (OleDbConnection con = new OleDbConnection(connStr))
        //    {
        //        con.Open();
        //        return con.GetSchema("Columns");
        //    }
        //}


        static DataTable GetColumns(string mdbPath, string password)
        {
            string connStr =
                @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + mdbPath + ";" +
                "Jet OLEDB:Database Password=" + password + ";";

            using (OleDbConnection con = new OleDbConnection(connStr))
            {
                con.Open();
                return con.GetSchema("Columns");
            }
        }

      public  static void CompareMdb(string mdb1, string pPwd1, string mdb2, string pPwd2)
        {
            DataTable dt1 = GetColumns(mdb1, pPwd1);
            DataTable dt2 = GetColumns(mdb2, pPwd2);

            Console.WriteLine("=== Missing fields (MDB1 → MDB2) ===");

            foreach (DataRow r1 in dt1.Rows)
            {
                string table1 = r1["TABLE_NAME"].ToString();
                string col1 = r1["COLUMN_NAME"].ToString();

                bool found = false;
                foreach (DataRow r2 in dt2.Rows)
                {
                    if (r2["TABLE_NAME"].ToString() == table1 &&
                        r2["COLUMN_NAME"].ToString() == col1)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    Console.WriteLine(table1 + "." + col1);
            }

            Console.WriteLine("\n=== Extra fields (MDB2 → MDB1) ===");

            foreach (DataRow r2 in dt2.Rows)
            {
                string table2 = r2["TABLE_NAME"].ToString();
                string col2 = r2["COLUMN_NAME"].ToString();

                bool found = false;
                foreach (DataRow r1 in dt1.Rows)
                {
                    if (r1["TABLE_NAME"].ToString() == table2 &&
                        r1["COLUMN_NAME"].ToString() == col2)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    Console.WriteLine(table2 + "." + col2);
            }

            Console.WriteLine("\n=== Datatype OR Size Mismatch ===");

            foreach (DataRow r1 in dt1.Rows)
            {
                string table1 = r1["TABLE_NAME"].ToString();
                string col1 = r1["COLUMN_NAME"].ToString();
                int type1 = Convert.ToInt32(r1["DATA_TYPE"]);
                int size1 = r1["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? -1 :
                            Convert.ToInt32(r1["CHARACTER_MAXIMUM_LENGTH"]);

                foreach (DataRow r2 in dt2.Rows)
                {
                    if (r2["TABLE_NAME"].ToString() == table1 &&
                        r2["COLUMN_NAME"].ToString() == col1)
                    {
                        int type2 = Convert.ToInt32(r2["DATA_TYPE"]);
                        int size2 = r2["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? -1 :
                                    Convert.ToInt32(r2["CHARACTER_MAXIMUM_LENGTH"]);

                        // treat 130 and 202 as same (Text)
                        bool typeMismatch = !(IsTextType(type1) && IsTextType(type2)) &&
                                            (type1 != type2);

                        bool sizeMismatch = size1 != size2;

                        if (typeMismatch || sizeMismatch)
                        {
                            Console.WriteLine(
                                table1 + "." + col1 +
                                "  TYPE1=" + GetTypeName(type1) +
                                "  TYPE2=" + GetTypeName(type2) +
                                "  SIZE1=" + size1 +
                                "  SIZE2=" + size2
                            );
                        }
                    }
                }
            }

            Console.WriteLine("\nDONE.");
        }
    }
}