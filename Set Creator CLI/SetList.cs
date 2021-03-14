using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Threading;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf.IO;

namespace Set_Creator_CLI
{
    class SetList
    {
        public List<Record> setlist { get; private set; }
        public string title { get; private set; }
        public int time { get; private set; }
        public string timeRepr { get; private set; }
        private string[] flags;
        public RecordManager Manager;

        public SetList(RecordManager Manager, string[] flags)
        {
            this.Manager = Manager;
            this.flags = flags;
            setlist = new List<Record>();
            time = 0;
            title = GetDefaultTitle();
        }

        private string GetDefaultTitle()
        {
            //DateTime now = new DateTime();
            string format = Convert.ToString(DateTime.Now).Substring(0, 10);//now.GetDateTimeFormats()[1];
            if (CheckFlag("fout"))
            {
                Console.WriteLine(format);
                Console.ReadKey();
            }
            return format;
        }

        private void GetTimeRepr()
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

            timeRepr = output;
        }

        public void AddRecord(Record record)
        {
            setlist.Add(record);
            UpdateTime();
        }

        public void RemoveRecord(int ind)
        {
            setlist.RemoveAt(ind);
            UpdateTime();
        }

        private void UpdateTime()
        {
            time = 0;
            foreach (Record rec in setlist)
            {
                time += rec.time;
            }
            GetTimeRepr();
        }

        public List<string[]> PackSet()
        {
            List<string[]> records = new List<string[]>();
            foreach (Record rec in setlist)
            {
                records.Add(rec.SerializeRecord());
            }
            return records;
        }

        public void ExportSet()
        {
            List<string[]> lib = PackSet();
            //JsonSerializer serializer = new JsonSerializer()
            //string json = Json.JsonConvert.SerializeObject(lib);
            var serializer = new JavaScriptSerializer();
            var serialized = serializer.Serialize(lib);
            File.WriteAllText("./Setlist.json", serialized);
        }

        public void ImportSet()
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
                //AddRecord(time, name, id);
                //int cnt = 1;
                foreach (Record rec in Manager.library.RecordList)
                {
                    if (rec.ID == id)
                    {
                        AddRecord(rec);
                        break;
                    }
                }
            }
        }

        private string PrepareString()
        // Prepare output string to save to txt/PDF
        // Added in V1.1
        {
            string text = "";
            int cnt = 0;
            string line;
            foreach (Record rec in setlist)
            {
                cnt += 1;
                //line = "";
                line = Convert.ToString(cnt) + "\t" + rec.name;
                text += line;
                if (cnt < setlist.Count)
                {
                    text += "\n";
                }
            }
            return text;
        }

        public void PrintToText()
        {
            string filename = "./" + title + ".txt";
            string text = PrepareString();
            File.WriteAllText(filename, text);
        }

        public void PrintToPdf()
        // Print setlist to PDF file
        // Supported from V1.1
        {
            string filename = "./" + title + ".pdf"; //get file name
            string text = PrepareString(); // prepare output string
            int FontSize = GetFontSize(); // get font size - based on record count

            PdfDocument file = new PdfDocument();
            PdfPage page = file.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Courier New", FontSize);
            XTextFormatter tf = new XTextFormatter(gfx);


            // Draw text to file
            tf.DrawString(text, font, XBrushes.Black,
                           new XRect(60, 60, page.Width-60, page.Height-60),
                           XStringFormats.TopLeft);
            // Save file
            file.Save(filename);

            //Console.WriteLine("PDF export not supported... yet."); -- it is supported now ;)
            //Console.ReadKey();
            return;
        }

        private int GetFontSize()
        // Set font size value according to setlist length
        // (>=) ITEMS: FONT_SIZE
        // 10: 40, 13: 36, 16: 28, 21: 20, 25: 16, 25+: 12
        {
            int[] count = { 10, 13, 16, 21, 25 };
            int[] size = { 42, 36, 28, 20, 16 };
            int items = setlist.Count;
            int cnt = 0;
            bool ind = false;
            foreach (int i in count)
            {
                if (items <= i)
                {
                    ind = true;
                    break;
                }
                cnt++;
            }
            if (ind == false) { return 12; } //if more than 25 items return size 12
            else
            {
                return size[cnt]; // return appropriate font size
            }
        }

        public bool Move(int index, char dir)
        {
            if (dir.Equals('u'))
            {
                if (index >= 0)
                {
                    return MoveUp(index);
                }
                else
                {
                    return false;
                }
            }
            else if (dir.Equals('d'))
            {
                if (index <= setlist.Count - 1)
                {
                    return MoveDown(index);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool MoveUp(int index)
        {
            try
            {
                Record rec = setlist[index];
                setlist.RemoveAt(index);
                setlist.Insert(index - 1, rec);
                return true;
            }
            catch// (Exception e)
            {
                if (CheckFlag("fout"))
                {
                    Console.WriteLine("SetList MoveUp error");
                    Console.ReadKey();
                }
                return false;
            }
        }

        private bool MoveDown(int index)
        {
            try
            {
                Record rec = setlist[index];
                setlist.RemoveAt(index);
                setlist.Insert(index + 1, rec);
                return true;
            }
            catch// (Exception e)
            {
                if (CheckFlag("fout"))
                {
                    Console.WriteLine("SetList MoveDown error");
                    Console.ReadKey();
                }
                return false;
            }
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

        public bool SetNewTitle(string newTitle)
        {
            title = newTitle;
            return true;
        }
    }
}
