using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Set_Creator_CLI
{
    class Record
    {
        private string[] flags;
        public int time { get; private set; }
        public string name { get; set; }
        public string timeRepr;// {get; private set;}
        public long ID { get; private set; }

        public Record(int len, string title, string[] fl)
        {
            time = len;
            name = title;
            flags = fl;
            //if (fl == null) {flags = null} else {flags = fl}
            //flags = null;
            timeRepr = getTimeRepr();
            ID = SetID();
        }

        public Record(int len, string title, string[] fl, long id)
        {
            time = len;
            name = title;
            flags = fl;
            //if (fl == null) {flags = null} else {flags = fl}
            //flags = null;
            timeRepr = getTimeRepr();
            ID = id;
        }

        private string getTimeRepr()
        {
            int[] com = { 0, 0, 0 };
            string[] txt = { "", "", "" };
            int tvar = time;
            string output = "";
            com[0] = (int)time / 3600;
            tvar = tvar - com[0] * 3600;
            com[1] = (int)tvar / 60;
            com[2] = tvar - com[1] * 60;

            //foreach (int num in com)
            for (int i = 0; i < 3; i++)
            {
                string repr = Convert.ToString(com[i]);
                repr = repr.Length >= 2 ? repr : "0" + repr;
                txt[i] = repr;
                output += repr;
                if (i < 2) { output += ":"; }
            }
            if (CheckFlag("fout"))
            {
                Console.WriteLine(output);
                Console.ReadKey();
            }

            return output;
        }

        public void Modify(string name)
        {
            this.name = name;
        }

        public void Modify(int time)
        {
            this.time = time;
            this.timeRepr = getTimeRepr();
        }

        public void Modify(int time, string name)
        {
            this.time = time;
            this.timeRepr = getTimeRepr();
            this.name = name;
        }

        private long SetID()
        {
            var time = DateTime.Now;
            long stamp = time.Ticks;
            if (CheckFlag("fout"))
            {
                Console.WriteLine(stamp);
                Console.ReadKey();
            }
            //Console.WriteLine(stamp);
            //stamp = DateTime.Ticks;
            //Console.WriteLine(stamp);
            return stamp;
        }

        public string Print()
        {
            string txt = "";
            txt = name + "\t" + time + "\t" + timeRepr + "\t" + ID;
            if (CheckFlag("fout"))
            {
                Console.WriteLine(txt);
                Console.ReadKey();
            }
            return txt;
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

        public string[] SerializeRecord()
        {
            string t = time.ToString();
            string n = name;
            string id = ID.ToString();
            string[] serialized = { t, n, id };
            return serialized;
        }

        public long GetRecordID()
        {
            return ID;
        }
    }
}
