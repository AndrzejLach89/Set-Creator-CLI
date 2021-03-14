using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Threading;

namespace Set_Creator_CLI
{
    class RecordLibrary
    {
        public List<Record> RecordList { get; private set; }
        //private ArrayList RecordList;
        private RecordManager Manager;
        private string[] flags;
        //public Assembly pdfHandler;

        public RecordLibrary(RecordManager Manager, string[] flags)
        {
            this.Manager = Manager;
            RecordList = new List<Record>();
            this.flags = flags;
            //RecordList = new ArrayList();
            // test pdf
            //pdfHandler = LoadPdfLibrary();
            //PdfDocument doc = new pdfHandler.PdfDocument();
            //doc.Info.Title = "Testowy";

        }

        /*private Assembly LoadPdfLibrary()
        {
            return Assembly.Load("PdfSharp-WPF.dll");
            
            
        }*/

        public void GetRecordsData(int nameLength)
        {
            int cnt = 1;
            foreach (Record rec in RecordList)
            {
                string line = Convert.ToString(cnt) + "\t";
                string nm = "";
                nm += rec.name;
                while (nm.Length < nameLength)
                {
                    nm += " ";
                }
                line = line + nm + "\t" + rec.timeRepr;
                cnt++;
                if (CheckFlag("fout"))
                {
                    Console.WriteLine(line);
                    Console.ReadKey();
                }
            }
        }

        public void AddRecord(int time, string name)
        {
            Record record = new Record(time, name, flags);
            RecordList.Add(record);
            if (CheckFlag("fout"))
            {
                foreach (Record i in RecordList) { Console.WriteLine(i.Print()); }
                Console.ReadKey();
            }
        }

        public void AddRecord(int time, string name, long id)
        {
            if (VerifyID(id))
            {
                Record record = new Record(time, name, flags, id);
                RecordList.Add(record);
            }
            else
            {
                if (CheckFlag("fout"))
                {
                    Console.WriteLine(name + " already exists.");
                    Console.ReadKey();
                }
            }
        }

        private bool VerifyID(long id)
        {
            long currentId;
            bool value = true;
            foreach (Record rec in RecordList)
            {
                if (CheckFlag("fout")) { Console.WriteLine(rec.name); }
                currentId = rec.GetRecordID();
                if (currentId == id)
                {
                    if (CheckFlag("fout"))
                    {
                        Console.WriteLine(id + " exists");
                        Console.ReadKey();
                    }
                    value = false;
                    break;
                }
            }
            return value;
        }

        public Record GetRecord(int index)
        {
            return RecordList[index];
        }

        private bool CheckFlag(string fl)
        {
            bool exist = false;
            foreach (string f in flags)
            {
                if (fl.Equals(f))
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        public void ModifyRecord(int index, int time, string name)
        {
            Record record = GetRecord(index);
            record.Modify(time, name);
        }

        public void ModifyRecord(int index, int time)
        {
            Record record = GetRecord(index);
            record.Modify(time);
        }

        public void ModifyRecord(int index, string name)
        {
            Record record = GetRecord(index);
            record.Modify(name);
        }

        public List<string[]> PackLibrary()
        {
            List<string[]> records = new List<string[]>();
            foreach (Record rec in RecordList)
            {
                records.Add(rec.SerializeRecord());
            }
            return records;
        }

        public void ExportLibrary()
        {
            List<string[]> lib = PackLibrary();
            //JsonSerializer serializer = new JsonSerializer()
            //string json = Json.JsonConvert.SerializeObject(lib);
            var serializer = new JavaScriptSerializer();
            var serialized = serializer.Serialize(lib);
            File.WriteAllText("./Records.json", serialized);
        }

        public void ImportLibrary()
        {
            string raw = File.ReadAllText("./Records.json");
            var serializer = new JavaScriptSerializer();
            var RecordsData = serializer.Deserialize<List<string[]>>(raw);
            //RecordList.Clear();
            int time;
            long id;
            string name;
            foreach (string[] data in RecordsData)
            {
                time = Convert.ToInt32(data[0]);
                name = data[1];
                id = Convert.ToInt64(data[2]);
                AddRecord(time, name, id);
            }
        }

        public void ImportLibrary(bool fromSet)
        {
            string raw = File.ReadAllText("./Setlist.json");
            var serializer = new JavaScriptSerializer();
            var RecordsData = serializer.Deserialize<List<string[]>>(raw);
            //RecordList.Clear();
            int time;
            long id;
            string name;
            foreach (string[] data in RecordsData)
            {
                time = Convert.ToInt32(data[0]);
                name = data[1];
                id = Convert.ToInt64(data[2]);
                AddRecord(time, name, id);
            }
        }

        public void CopyRecord(int ind)
        {
            Record record = GetRecord(ind);
            string name = record.name;
            int time = record.time;
            AddRecord(time, name);
        }

        public bool RemoveRecord(int index)
        {
            RecordList.RemoveAt(index);
            return true;
        }
    }
}
