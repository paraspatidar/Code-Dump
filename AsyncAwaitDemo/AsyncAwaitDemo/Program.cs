using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BEGIN!!!");
            DocumentWorker documentWorker = new DocumentWorker();
            Console.WriteLine("Enter ThreadCount & Doc Count!!!");
            int Threadcount=Convert.ToInt32( Console.ReadLine());

            int DocCount = Convert.ToInt32(Console.ReadLine());
            try
            {
                documentWorker.DoWork(Threadcount, DocCount).GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR : {ex.Message}");
            }
            finally
            {
                Logger.Instance.CloseStream();
            }

            Console.WriteLine(documentWorker.TotalCount);
            Console.WriteLine("END!!!");
            Console.ReadLine();
        }
    }
}
