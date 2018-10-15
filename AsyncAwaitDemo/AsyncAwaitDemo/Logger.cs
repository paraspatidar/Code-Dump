using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy =
        new Lazy<Logger>(() => new Logger());

       // StreamWriter sw = new StreamWriter(@"mylog1.csv", true);
        static ReaderWriterLock locker = new ReaderWriterLock();
        private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
        public static Logger Instance { get { return lazy.Value; } }
        private Logger()
        {

        }

        //public async Task Log(int threadId,int did, string DocId,string Phase,string aditionalmsg="")
        //{
        //    string message = $"{threadId} , {did}  , {DocId} , {Phase} , {aditionalmsg}";
        //    Console.WriteLine(message);
        //    try
        //    {
        //        locker.AcquireWriterLock(10000); //You might wanna change timeout value 
        //        //await sw.WriteLineAsync(message);
        //    }
        //    finally
        //    {
        //        if(locker.IsWriterLockHeld)
        //            locker.ReleaseWriterLock();
        //    }
        //}

        public async Task Log(int threadId, int did, string DocId, string Phase, string aditionalmsg = "")
        {
            string message = $"{threadId} , {did}  , {DocId} , {Phase} , {aditionalmsg}"+Environment.NewLine;
            Console.WriteLine(message);
            lock_.EnterWriteLock();
            try
            {
                using (var fs = new FileStream(@"mylog1.csv", FileMode.Append, FileAccess.Write))
                {
                    byte[] dataAsByteArray = new UTF8Encoding(true).GetBytes(message);
                    fs.Write(dataAsByteArray, 0, message.Length);
                }
            }
            finally
            {
                lock_.ExitWriteLock();
            }
        }

        public void CloseStream()
        {
            //sw.Flush();
            //sw.Close();
        }
    }
}
