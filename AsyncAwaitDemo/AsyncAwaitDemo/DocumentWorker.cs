using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    public class DocumentWorker
    {
       public int TotalCount;
        List<Task> MainThreads = new List<Task>();
        List<Task> DocThreads = new List<Task>();
        public async Task DoWork(int NoOfDocworker, int noOfInnerDocs)
        {
            TotalCount = NoOfDocworker * noOfInnerDocs;
           await Logger.Instance.Log(000, 000, "000", "000");
           for(int i=1;i<= NoOfDocworker;i++)
            {
                MainThreads.Add(wrapperMainT(i, noOfInnerDocs));
               
            }

           await Task.WhenAll(MainThreads);
           
        }

        public async Task DocProcesser(int tid,int NoOfDocs)
        {
            for(int i=1;i<=NoOfDocs;i++)
            {
                DocThreads.Add(wrapperDocs(tid, i));
                Console.WriteLine("COUNT ::::" + --TotalCount);
            }

            await Task.WhenAll(DocThreads);
        }
        public async Task PushDoc(int tid,int did,string Docguid)
        {
            await Task.Delay(10000);
            await Logger.Instance.Log(tid,did,Docguid, "DOC INSERTED");
        }
        public async Task<string> GenerateDoc()
        {
           return new Guid().ToString();
        }

        public async Task wrapperDocs(int tid,int i)
        {
            await Logger.Instance.Log(tid, i, "NA", "DocProcesser START", null);
            string docId = await GenerateDoc();
            await PushDoc(tid, i, docId);
            await Logger.Instance.Log(tid, i, docId, "DocProcesser END", null);
        }
        public async Task wrapperMainT(int i, int noOfInnerDocs)
        {
            await Logger.Instance.Log(i, -1, "", "THREAD START", null);
            await DocProcesser(i, noOfInnerDocs);
            await Logger.Instance.Log(i, -1, "", "THREAD COMPLETED", null);
        }
    }
}
