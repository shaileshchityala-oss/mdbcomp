using System;
using System.Data;
using System.Data.OleDb;

namespace NMdb
{
    class Prg
    {

        static void Main()
        {
            MdbComparerLog.mOutPath = @"C:\Shailesh\bse\mdb\res.txt";
            string file1 = @"C:\Shailesh\bse\mdb\reg.mdb";
            string file2 = @"C:\Shailesh\bse\mdb\bse.mdb";
            string pwd = @"FintEstpwD";

            MdbComparerLog.CompareMdb(file1, pwd, file2, pwd);
            Console.ReadKey();
        }
    }
}