using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Set_Creator_CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            CUI ui = new CUI(args);
            /*if (!CheckFlag("gui", args))
            {
                CUI ui = new CUI(args);
            }*/
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
