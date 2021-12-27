using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
    class DataManagerJSON
    {
        public static void SaveData(string path, List<UserInSchedule> users)
        {
            try
            {
                File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(users));
                Console.WriteLine($"Save data in {path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                BagReport.SetBagAndSave(e.Message, Program.pathBagJSON);
            }
        }
        public static List<UserInSchedule> LOadData(string path)
        {
            List<UserInSchedule> users = new();
            try
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    users = JsonConvert.DeserializeObject<List<UserInSchedule>>(stream.ReadToEnd());
                }
                Console.WriteLine($"Load data from {path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                BagReport.SetBagAndSave(e.Message, Program.pathBagJSON);
            }
            return users;
        }
        public static void SaveDataWeek(string path, List<string> listWeek)
        {
            try
            {
                File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(listWeek));
                Console.WriteLine($"Save data in {path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                BagReport.SetBagAndSave(e.Message, Program.pathBagJSON);
            }
        }
        public static List<string> LOadDataWeek(string path)
        {
            List<string> listWeek = new();
            try
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    listWeek = JsonConvert.DeserializeObject<List<string>>(stream.ReadToEnd());
                }
                Console.WriteLine($"Load data from {path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                BagReport.SetBagAndSave(e.Message, Program.pathBagJSON);
            }
            return listWeek;
        }
    }
}
