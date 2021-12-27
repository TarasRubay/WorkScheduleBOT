using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
    class DataManager
    {
        public string Path { get; private set; } = "";
        public string PathJSON { get; private set; } = "";
        public string PathStillProcent { get; private set; } = "";
        public DataManager(string path, string stillProcent , string jsonPath)
        {
            Path = path;
            PathStillProcent = stillProcent;
            PathJSON = jsonPath;
        }

        public void SaveData(List<User> users)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(Path, FileMode.Create, FileAccess.Write))
                    formatter.Serialize(stream, users);
                Console.WriteLine($"Save data in {Path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
               
            }
        }
        public void SaveDataJSON(List<User> users)
        {
            try
            {
                
                //using FileStream fileStream = File.Create(PathJSON);
                string jsonUs = System.Text.Json.JsonSerializer.Serialize(users);
                File.WriteAllText(PathJSON,jsonUs);
                Console.WriteLine($"Save data in {PathJSON}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public List<User> LoadDataJSON()
        {
            List<User> users = new();
            try
            {
                using (StreamReader r = new StreamReader(PathJSON))
                {
                    string json = r.ReadToEnd();
                    users = JsonConvert.DeserializeObject<List<User>>(json);
                }
                Console.WriteLine($"Load data from {PathJSON}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return users;
        }
        public List<User> LoadData()
        {
            List<User> users = new();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
                    users = formatter.Deserialize(stream) as List<User>;
                Console.WriteLine($"Load data from {Path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return users;
        }
        public void SaveDataStillProcent(double procent)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(PathStillProcent, FileMode.Create, FileAccess.Write))
                    formatter.Serialize(stream, procent);
                Console.WriteLine($"Save data in {PathStillProcent}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public double LoadDataProcent()
        {
            double procent = 2.00;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(PathStillProcent, FileMode.Open, FileAccess.Read))
                    procent = (double)formatter.Deserialize(stream);
                Console.WriteLine($"Load data from {PathStillProcent}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return procent;
        }
    }
}
