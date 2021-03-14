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
    class RecordManager
    {
        public RecordLibrary library;
        public SetList setlist;
        public string[] flags;

        public RecordManager(string[] flags)
        {
            this.flags = flags;
            library = new RecordLibrary(this, flags);
            setlist = new SetList(this, flags);
        }

        public bool AddToLibrary(int time, string name)
        {
            try
            {
                library.AddRecord(time, name);
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool DelFromLibrary(int index)
        {
            try
            {
                library.RemoveRecord(index);
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool ModifyRecord(int index, int t, string nm)
        {
            try
            {
                library.RecordList[index].Modify(t, nm);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ImportLibrary()
        {
            try
            {
                library.ImportLibrary();
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool ExportLibrary()
        {
            try
            {
                library.ExportLibrary();
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool ImportSetList()
        {
            if (ImportLibrary())
            {
                try
                {
                    setlist.ImportSet();
                    return true;
                }
                catch
                {
                    try
                    {
                        library.ImportLibrary(true);
                        setlist.ImportSet();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    library.ImportLibrary(true);
                    setlist.ImportSet();
                    return true;
                }
                catch// (Exception e)
                {
                    return false;
                }
            }

        }

        public bool ExportSetList()
        {
            try
            {
                //library.ImportLibrary(true);
                ExportLibrary();
                setlist.ExportSet();
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool PrintToText()
        {
            try
            {
                setlist.PrintToText();
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool PrintToPdf()
        {
            try
            {
                setlist.PrintToPdf();
                return true;
            }
            catch// (Exception e)
            {
                //return false;
                setlist.PrintToPdf();
                return true;
            }
        }

        public bool MoveUp(int index)
        {
            if (index < 1)
            {
                return false;
            }
            else
            {
                try
                {
                    setlist.Move(index, 'u');
                    return true;
                }
                catch// (Exception e)
                {
                    return false;
                }
            }

        }

        public bool MoveDown(int index)
        {
            if (index >= setlist.setlist.Count - 1)
            {
                return false;
            }
            else
            {
                try
                {
                    setlist.Move(index, 'd');
                    return true;
                }
                catch// (Exception e)
                {
                    return false;
                }
            }

        }

        public bool AddToSet(int index)
        {
            try
            {
                Record record = library.GetRecord(index);
                setlist.AddRecord(record);
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool RemoveFromSet(int index)
        {
            try
            {
                setlist.RemoveRecord(index);
                return true;
            }
            catch// (Exception e)
            {
                return false;
            }
        }

        public bool RenameSet(string newName)
        {
            try
            {
                setlist.SetNewTitle(newName);
                return true;
            }
            catch// (Exception e)
            {
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

        private static bool CheckFlag(string fl, string[] flags)
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
    }
}

