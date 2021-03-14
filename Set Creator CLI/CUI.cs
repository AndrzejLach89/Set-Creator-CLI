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
    class CUI
    {
                readonly int sepLength = Console.WindowWidth;
        const int nameLength = 32;
        bool status;
        string[] flags;
        RecordManager Manager;
        
        public CUI (string[] args)
        {
            status = true;
            flags = args;
            Manager = new RecordManager(flags);
            clear();
            if (!CheckFlag("nointro"))
            {
                ShowIntro();
            }
            if (CheckFlag("delayed"))
            {
                ShowIntro(2000);
            }
            while (status == true)
            {
                status = MainLoop();
            }
        }
        
        private void ShowIntro()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            write("SETLIST MANAGER V 0.2");
            Console.ResetColor();
            Console.ReadKey();
        }
        
        private void ShowIntro(int x)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            write("SETLIST MANAGER V 0.2");
            Console.ResetColor();
            Thread.Sleep(x);
        }
        
        private void write(string line)
        {
            Console.WriteLine(line);
        }
        
        private void separator()
        {
            Console.WriteLine("".PadRight(sepLength, '-'));
        }
        
        private void clear()
        {
            Console.Clear();
            //Console.SetCursorPosition(0, Console.CursorTop);
            //Console.Write(new String(' ', Console.BufferWidth));
            //Console.SetCursorPosition(0, Console.CursorTop);
        }
        
        private string GetInput()
        {
            Console.Write(">>");
            string input = Console.ReadLine();
            return input;
        }
        
        private int? NumInput(string str)
        {
            int? vl;
            try
            {
                vl = Convert.ToInt32(str);
            }
            catch
            {
                vl = null;
            }
            return vl;
        }
        
        private int GetInputFromList(int range)
        {
            // test
            //return (int)NumInput(GetInput());
            // /test
            if (range < 1)
            {
                return -1;
            }
            int[] listRange = new int[range];
            int cnt = 1;
            while (cnt < range+1)
            {
                listRange[cnt-1] = cnt;
                cnt++;
            }
            int? input = NumInput(GetInput());
            if (input == null)
            {
                return -1;
            }
            else if (Array.IndexOf(listRange, input) == -1)
            {
                return -1;
            }
            else
            {
                return (int)input;
            }
        }
        
        private void header(string title)
        {
            clear();
            write(title);
            separator();
        }
        
        private void checkAction(bool state)
        {
            if (state == false)
            {
                write("Error occured");
                Console.ReadKey();
            }
        }
        
        private bool checkSymbols(string txt)
        {
            return String.IsNullOrWhiteSpace(txt);
        }
        
        private bool MainLoop()
        {
            header("MAIN MENU");
            write("1 - Edit library");
            write("2 - Edit set");
            write("3 - Exit");
            write("Chose option from list");
            int input = GetInputFromList(3);
            bool inAction = true;
            switch (input)
            {
                case -1:
                    //break;
                    return true;
                case 3:
                    clear();
                    return false;
                case 1:
                    while (inAction == true)
                    {
                        inAction = EditLibrary();
                    }
                    return true;
                case 2:
                    while (inAction == true)
                    {
                        inAction = EditSet();
                    }
                    return true;
                default:
                    return false;
            }
        }
        
        private void PrintRecords(List<Record> recordList, int color=0)
        {
            int cnt = 1;
            foreach (Record rec in recordList)
            {
                string line = Convert.ToString(cnt) + "\t";
                string nm = "";
                nm += rec.name;
                while (nm.Length < nameLength)
                {
                    nm += " ";
                }
                line = line + nm +  "\t" + rec.timeRepr;
                cnt++;
                if (color == 0)
                {
                    Console.ResetColor();
                }
                else
                {
                    if (CheckIfAdded(rec))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ResetColor();
                    }
                }
                Console.WriteLine(line);
            }
            Console.ResetColor();
        }
        
        private bool CheckIfAdded(Record rec)
        {
            bool added = false;
            long id = rec.ID;
            foreach (Record i in Manager.setlist.setlist)
            {
                if (i.ID == id)
                {
                    added = true;
                    break;
                }
            }
            return added;
        }
        
        
        /*private void PrintLibrary()
        {
            int cnt = 1;
            foreach (Record rec in Manager.library.RecordList)
            {
                string line = Convert.ToString(cnt) + "\t";
                string nm = "";
                nm += rec.name;
                while (nm.Length < 32)
                {
                    nm += " ";
                }
                line = line + nm +  "\t" + rec.timeRepr;
                cnt++;
                Console.WriteLine(line);
            }
        }*/
        
        private bool EditLibrary()
        {
            header("EDIT LIBRARY");
            write("Current library");
            
            PrintRecords(Manager.library.RecordList);
            Console.WriteLine("");
            write("1 - Add record");
            write("2 - Remove record");
            write("3 - Modify record");
            write("4 - Import library");
            write("5 - Export library");
            write("6 - Go back");
            write("Chose option from list");
            
            int input = GetInputFromList(6);
            bool inAction = true; 
            switch (input)
            {
                case -1:
                    return true;
                case 6:
                    return false;
                case 1:
                    while (inAction == true)
                    {
                        inAction = LibraryAdd();
                    }
                    return true;
                case 2:
                    while (inAction == true)
                    {
                        inAction = LibraryRemove();
                    }
                    return true;
                case 3:
                    while (inAction == true)
                    {
                        inAction = LibraryModify();
                    }
                    return true;
                case 4:
                    checkAction(ImportLibrary());
                    return true;
                case 5:
                    checkAction(ExportLibrary());
                    return true;
                default:
                    return false;
            }
        }
        
        private Hashtable CreateRecord()
        {
            Hashtable datas = new Hashtable();
            datas["name"] = "";
            datas["hh"] = null;
            datas["mm"] = null;
            datas["ss"] = null;
            clear();
            write("Enter name");
            while (checkSymbols(Convert.ToString(datas["nm"])) == true)
            {
                datas["nm"] = GetInput();
            }
            //nm = GetInput();
            write("Enter time - hours");
            while (datas["hh"] == null || Convert.ToInt32(datas["hh"]) < 0)
            {
                datas["hh"] = NumInput(GetInput());
            }
            write("Enter time - minutes");
            while (datas["mm"] == null || Convert.ToInt32(datas["mm"]) < 0)
            {
                datas["mm"] = NumInput(GetInput());
            }
            write("Enter time - seconds");
            while (datas["ss"] == null || Convert.ToInt32(datas["ss"]) < 0)
            {
                datas["ss"] = NumInput(GetInput());
            }
            return datas;
        }
        
        private bool LibraryAdd()
        {
            /*string nm = "";
            int? hh = null;
            int? mm = null;
            int? ss = null;
            
            clear();
            write("Enter name");
            while (checkSymbols(nm) == true)
            {
                nm = GetInput();
            }
            //nm = GetInput();
            write("Enter time - hours");
            while (hh == null || hh < 0)
            {
                hh = NumInput(GetInput());
            }
            write("Enter time - minutes");
            while (mm == null || hh < 0)
            {
                mm = NumInput(GetInput());
            }
            write("Enter time - seconds");
            while (ss == null || hh < 0)
            {
                ss = NumInput(GetInput());
            }*/
            Hashtable datas = CreateRecord();
            string nm = Convert.ToString(datas["nm"]);
            int? hh = Convert.ToInt32(datas["hh"]);
            int? mm = Convert.ToInt32(datas["mm"]);
            int? ss = Convert.ToInt32(datas["ss"]);
            int t = 3600*(int)hh + 60*(int)mm + (int)ss;
            if (t > 0)
            {
                Manager.AddToLibrary(t, nm);
            }
            return false;
        }
        
        private bool LibraryRemove()
        {
            header("Select record to delete or select " + (Manager.library.RecordList.Count+1) + " to go back");
            PrintRecords(Manager.library.RecordList);
            write((Manager.library.RecordList.Count+1) + "\t Go back");
            int input = GetInputFromList(Manager.library.RecordList.Count+1);
            if (input==Manager.library.RecordList.Count+1)
            {
                return false;
            }
            else
            {
                Manager.DelFromLibrary(input);
                return false;
            }
        }
        
        private bool LibraryModify()
        {
            header("Chose record to modify or " + (Manager.library.RecordList.Count+1) + " to go back");
            PrintRecords(Manager.library.RecordList);
            write((Manager.library.RecordList.Count+1) + "\t Go back");
            int input = GetInputFromList(Manager.library.RecordList.Count+1);
            if (input == Manager.library.RecordList.Count+1)
            {
                return false;
            }
            else
            {
                Hashtable datas = CreateRecord();
                string nm = Convert.ToString(datas["nm"]);
                int? hh = Convert.ToInt32(datas["hh"]);
                int? mm = Convert.ToInt32(datas["mm"]);
                int? ss = Convert.ToInt32(datas["ss"]);
                int t = 3600*(int)hh + 60*(int)mm + (int)ss;
                if (t > 0)
                {
                    if(Manager.ModifyRecord(input-1, t, nm))
                    {
                        return false;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        write("Record could not be modified");
                        Console.ResetColor();
                        Console.ReadKey();
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        
        private bool ImportLibrary()
        {
            if (Manager.ImportLibrary())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                write("Library imported! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Library import failed! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
        }
        
        private bool ExportLibrary()
        {
            if (Manager.ExportLibrary())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                write("Library exported! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Library export failed! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
        }
        
        private bool EditSet()
        {
            header("EDIT SET");
            write("Set name: " + Manager.setlist.title);
            write("Set time: " + Manager.setlist.timeRepr);
            PrintRecords(Manager.setlist.setlist);
            Console.WriteLine("");
            write("1 - Add record");
            write("2 - Remove record");
            write("3 - Move up");
            write("4 - Move down");
            write("5 - Import set");
            write("6 - Export set");
            write("7 - Save to txt");
            write("8 - Save to PDF");
            write("9 - Rename set");
            write("10- Go back");
            write("Chose option from list");
            
            int input = GetInputFromList(10);
            bool inAction = true;
            switch (input)
            {
                case -1:
                    return true;
                case 10:
                    return false;
                case 1:
                    while (inAction == true)
                    {
                        inAction = SetAdd();
                    }
                    return true;
                case 2:
                    while (inAction == true)
                    {
                        inAction = SetRemove();
                    }
                    return true;
                case 3:
                    while (inAction == true)
                    {
                        inAction = SetMove('u');
                    }
                    return true;
                case 4:
                    while (inAction == true)
                    {
                        inAction = SetMove('d');
                    }
                    return true;
                case 5:
                    checkAction(ImportSet());
                    return true;
                case 6:
                    checkAction(ExportSet());
                    return true;
                case 7:
                    checkAction(PrintToText());
                    return true;
                case 8:
                    checkAction(PrintToPdf());
                    return true;
                case 9:
                    checkAction(RenameSet());
                    return true;
                default:
                    return false;
            }
        }
        
        private bool ImportSet()
        {
            if (Manager.ImportSetList())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                write("Set imported! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Set import failed! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
        }
        
        private bool ExportSet()
        {
            if (Manager.ExportSetList())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                write("Set exported! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Set export failed! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
        }
        
        private bool PrintToText()
        {
            if (Manager.PrintToText())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                write("Set saved to " + Manager.setlist.title + ".txt! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Operation failed! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
        }
        
        private bool PrintToPdf()
        {
            if (Manager.PrintToPdf())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                write("Set saved to " + Manager.setlist.title + ".pdf! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Operation failed! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return true;
            }
        }
        
        private bool SetAdd()
        {
            header("Select record to delete or select " + (Manager.library.RecordList.Count+1) + " to go back");
            PrintRecords(Manager.library.RecordList, 1);
            write((Manager.library.RecordList.Count+1) + "\tGo back");
            int input = GetInputFromList(Manager.library.RecordList.Count+1);
            if (input == Manager.library.RecordList.Count+1)
            {
                return false;
            }
            else
            {
                Manager.AddToSet(input-1);
                return false;
            }
        }
        
        private bool SetRemove()
        {
            header("Select record to delete or select " + (Manager.setlist.setlist.Count+1) + " to go back");
            PrintRecords(Manager.setlist.setlist);
            write((Manager.setlist.setlist.Count+1) + "\nGo back");
            int input = GetInputFromList(Manager.setlist.setlist.Count+1);
            if (input == Manager.setlist.setlist.Count+1)
            {
                return false;
            }
            else
            {
                Manager.RemoveFromSet(input-1);
                return false;
            }
        }
        
        private bool SetMove(char dir)
        {
            header("Select record to move or select " + (Manager.setlist.setlist.Count+1) + " to go back");
            PrintRecords(Manager.setlist.setlist);
            write((Manager.setlist.setlist.Count+1) + " Go back");
            int input = GetInputFromList(Manager.setlist.setlist.Count+1);
            if (input == Manager.setlist.setlist.Count+1)
            {
                return false;
            }
            else
            {
                if (dir == 'u')
                {
                    Manager.MoveUp(input-1);
                    return false;
                }
                else if (dir == 'd')
                {
                    Manager.MoveDown(input-1);
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
        
        private bool RenameSet()
        {
            header("Enter new name");
            string input = GetInput();
            if (Manager.RenameSet(input))
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                write("Can't rename set! Press key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return false;
            }
            
        }
        
        private bool CheckFlag(string fl)
        {
            bool exist = false;
            foreach (string f in flags)
            {
                if (fl.Equals(f)) {
                    exist = true;
                    break;
                }
            }
            return exist;
        }
    }
}
