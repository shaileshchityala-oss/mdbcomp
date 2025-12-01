using System;
using System.Data;
using System.Data.OleDb;

namespace NMdb
{
    class Prg
    {

        static void Main()
        {
           // MdbComparer.CompareMdb(@"C:\Shailesh\misc\code\greekReg.mdb", "FintEstpwD", @"C:\Shailesh\misc\code\greekNav.mdb", "FintEstpwD");
            MdbComparerLog.CompareMdb(@"C:\Shailesh\misc\code\greekReg.mdb", "FintEstpwD", @"C:\Shailesh\misc\code\greekNav.mdb", "FintEstpwD");
            Console.ReadKey();
        }
    }
}