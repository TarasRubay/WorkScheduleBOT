using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
    internal class BagReport
    {
        private static List<string> listBags = new();
        public static void LoadBags(string pathBag)
        {
            List<string> vs = new();
            vs = DataManagerJSON.LOadDataWeek(pathBag);
            foreach (var item in vs)
            {
                listBags.Add(item);
            }
        }
        public static void SetBagAndSave(string exception, string pathBag)
        {
            listBags.Add($"{DateTime.Now}/n{exception}");
            DataManagerJSON.SaveDataWeek(pathBag, listBags);
        }
        public static List<string> GetBag()
        {
            List<string> tmp = new(listBags);
            tmp.Reverse();
            List<string> lastBag = new();

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    lastBag.Add(tmp[i]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (lastBag.Count == 0)
            {
                lastBag.Add("List empty");
                return lastBag;
            }
            return lastBag;    
        }
    }
}
