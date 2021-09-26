using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Runtime.InteropServices;
using System.Timers;
using System.IO;
using System.Threading;

namespace WorkScheduleBOT
{
    [Serializable]
    class Program
    {
        public enum MountsUA { січень, лютий, березень, квітень, травень, червень, липень, серпень, вересень, жовтень, листопад, грудень };
        private static System.Timers.Timer aTimer;
        public static DateTime LastUpdateExcel;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        public static double StillProcent { get; set; }
        public static List<User> Users { get; set; }
        public static List<User> UsersOld { get; set; }
        public static List<List<object>> ExcelArrayObject;
        public static string LastUpDateWorkShedulePenunlimate;
        public static string LastUpDateWorkSheduleLast;
        public static List<string> WorkScheduleShift1;
        public static List<string> WorkScheduleShift2;
        public static List<string> WorkScheduleShift3;
        public static List<string> WorkScheduleShift4;
        public static List<string> WorkScheduleShift1_Last;
        public static List<string> WorkScheduleShift2_Last;
        public static List<string> WorkScheduleShift3_Last;
        public static List<string> WorkScheduleShift4_Last;
        //public static Dictionary<string, List<UserInSchedule>> WeekShedule = new();
        public static List<UserInSchedule> userInSchedulesPenunlimateWeek;
        public static List<UserInSchedule> userInSchedulesLastWeek;
        
        public static List<UserInSchedule> ListUserSchedule = new();
        public static List<string> usersList;
        public static List<string> ListWeeks { get; set; } = new();
        private static string token { get; set; } = "1973483435:AAEhUsog6N9nGLQ0SJ_GxJb4nXd2Mo40Blk";//Work Shedule spp
        //private static string token { get; set; } = "1912296215:AAHmxbSt7HtFRMTxiwLJ4okS6ummvUfu0Pg";//feature spp
        

        private static TelegramBotClient client;
        //public static Menu menu { get; set; }

        public static DataManager dataManager;
        public static string path = @"tel.bin";
        public static string pathJSON = @"UsersList.json";
        public static string pathXLS = @"D:\Documents\bot\WorkScheduleBOT\WorkScheduleBOT\bin\Debug\net5.0\2 ГРАФІК ОПЕРАТОРИ.xlsx";
        public static bool UpdateIsComplited = false;
        static void Main(string[] args)
        {

            var handle = GetConsoleWindow();
            dataManager = new(path, "empty",pathJSON);
            Users = new();
            UsersOld = new();
            UsersOld = dataManager.LoadData();
            StartBot();
            Users = dataManager.LoadDataJSON();
            
            
            foreach (var item in Users)
            {
                item.LastMessage = "empty";
                var list = item.ListFollowersForJson.Split('#');
                foreach (var it in list)
                {
                    if(it is not "")item.AddFollower(it);
                }
                item.Notify += DisplayMessageTelegram;
                foreach (var follower in item.GetListFollower())
                {
                    var usInSch = Program.ListUserSchedule.Find(us => us.Name == follower); 
                    if (usInSch is not null)
                    {
                        usInSch.NotifyShift += item.MessageFromUser;
                        usInSch.NotifyNewWeek += item.MessageFromUser;
                    }

                }
                //foreach (var us in ListUserSchedule)
                //{

                //        us.NotifyHospital += item.MessageFromUser;
                //}
            }
            aTimer = new System.Timers.Timer();
            try
            {
                client = new TelegramBotClient(token);
                client.StartReceiving();
                client.OnMessage += OnMessageHandler;
                Console.WriteLine("bot started");
            aTimer.Interval = 2000;
            //aTimer.Elapsed += OnTimedEvent;
            aTimer.Elapsed += OnTimedEvent2;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
                ShowWindow(handle, SW_HIDE);
                //ShowWindow(handle, SW_SHOW);
                Console.Read();
                client.StopReceiving();
            }
            catch (Exception)
            {}
            dataManager.SaveData(Users);
        }
        public static void StartReading()
        {
            Console.WriteLine("Thread.Sleep(10000);");
            Thread.Sleep(10000);
            try
            {
                using (StreamReader sr = new StreamReader(pathXLS))
                {
                    Console.WriteLine("The file be read for update:");
                    LastUpdateExcel = new FileInfo(pathXLS).LastWriteTime;
                }
                ExcelReader xslReader = new(pathXLS);
                string TableReadet = "";
                string nameRead = "last";
                ExcelArrayObject = new(xslReader.readX(ref TableReadet, ref nameRead));
                var arrNameRead = nameRead.Split(" ");
                if (arrNameRead[2] == ListWeeks[^1])
                {
                    Menu.CheckUpdateInScedule(client, ExcelArrayObject, nameRead);

                    Console.WriteLine("update last");
                    //second read table
                    TableReadet = "";
                    nameRead = "penunlimate";
                    ExcelArrayObject = new(xslReader.readX(ref TableReadet, ref nameRead));
                    List<string> listHospital = Menu.CheckUpdateInScedule(client, ExcelArrayObject, nameRead);
                    //foreach (var item in listHospital)
                    //{
                    //    var us = ListUserSchedule.Find(u => u.Name == item);
                    //    if (us is not null) us.MailingHospital();
                    //    Console.WriteLine("send hosp not.");
                    //}
                    Console.WriteLine("update penunlimited");
                }
                else
                {
                    List<string> listHospital = Menu.CreatingUsersInScheduleSecondStartNew(client, ExcelArrayObject, nameRead);
                    foreach (var item in ListUserSchedule)
                    {
                        item.MailingByAddNewWeek();
                    }
                    Console.WriteLine("Add new week");
                    //foreach (var item in listHospital)
                    //{
                    //    var us = ListUserSchedule.Find(u => u.Name == item);
                    //    if (us is not null) us.MailingHospital();
                    //    Console.WriteLine("send hosp not.");
                    //}
                }
            }
            catch (Exception Ex)
            {
                // Let the user know what went wrong.
                Console.WriteLine(Ex.Message);
                //await client.SendTextMessageAsync(Program.Users[0].Id, "The file could not be read:" + Ex.Message);
            }

        }
        public static void StartBot()
        {
                try
                {
                    using (StreamReader sr = new StreamReader(pathXLS))
                    {
                        Console.WriteLine("The file be read first time:");
                        LastUpdateExcel = new FileInfo(pathXLS).LastWriteTime;
                    }
                    ExcelReader xslReader = new(pathXLS);
                    string TableReadet = "";
                    string nameRead = "penunlimate";
                    ExcelArrayObject = new(xslReader.readX(ref TableReadet, ref nameRead));
                    Menu.CreatingUsersInScheduleFirstStartNew(client, ExcelArrayObject, nameRead);

                    Console.WriteLine("File update penunlimited StartBot(); ");
                   
                    //second read table
                    TableReadet = "";
                    nameRead = "last";
                    ExcelArrayObject = new(xslReader.readX(ref TableReadet, ref nameRead));
                    Menu.CreatingUsersInScheduleSecondStartNew(client, ExcelArrayObject, nameRead);
                    Console.WriteLine("File update table last StartBot(); ");
                  
                }
                catch (Exception Ex)
                {

                Console.WriteLine(Ex.Message);
                }
            
        }
        private static async void OnTimedEvent2(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine($"{LastUpdateExcel} {e.SignalTime}");
            if (LastUpdateExcel < new FileInfo(pathXLS).LastWriteTime)
            {

                try
                {
                    using (StreamReader sr = new StreamReader(pathXLS))
                    {
                        Console.WriteLine("The file be read:");
                        LastUpdateExcel = new FileInfo(pathXLS).LastWriteTime;
                    }
                    Thread thread = new Thread(new ThreadStart(StartReading));
                    thread.Start();
                       
                }
                catch (Exception Ex)
                {
                   
                    Console.WriteLine(Ex.Message);
                   
                }

            }
        }
        private static async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
           
            Console.WriteLine($"{LastUpdateExcel} {e.SignalTime}");
            if (LastUpdateExcel < new FileInfo(pathXLS).LastWriteTime)
            {
                
                try
                {
                    using (StreamReader sr = new StreamReader(pathXLS))
                    {
                        Console.WriteLine("The file be read:");
                        LastUpdateExcel = new FileInfo(pathXLS).LastWriteTime;
                    }
                    try
                    {
                        
                        ExcelReader xslReader = new(pathXLS);
                        string TableReadet = "";
                        string nameRead = "penunlimate";

                        ExcelArrayObject = new(xslReader.readX(ref TableReadet,ref nameRead));
                       
                        LastUpDateWorkSheduleLast = TableReadet;
                        WorkScheduleShift1_Last = new();
                        WorkScheduleShift2_Last = new();
                        WorkScheduleShift3_Last = new();
                        WorkScheduleShift4_Last = new();
                        usersList = new();

                        int countShift = 0;
                        for (int i = 3; i < ExcelArrayObject.Count; i++)
                        {
                            if (ExcelArrayObject[i][1] is not null)
                            {
                                if (ExcelArrayObject[i][0].ToString() == "1")
                                    countShift++;
                                string tmp = ExcelArrayObject[i][1].ToString() + ": ";
                                usersList.Add(tmp);
                                for (int j = 2; j < 16; j++)
                                {
                                    if (ExcelArrayObject[i][j] is not null)
                                    {
                                        if (ExcelArrayObject[i][j].ToString() == "12")
                                        {
                                            if (ExcelArrayObject[0][j].ToString() == "ніч")
                                                tmp += ExcelArrayObject[0][j - 1].ToString() + " " + ExcelArrayObject[0][j].ToString() + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                            else
                                                tmp += ExcelArrayObject[0][j].ToString() + " день " + ExcelArrayObject[1][j].ToString() + ", ";
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "в")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "В")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "Л")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "л")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                    }

                                }

                                if (countShift == 1)
                                    WorkScheduleShift1_Last.Add(tmp);
                                else if (countShift == 2)
                                    WorkScheduleShift2_Last.Add(tmp);
                                else if (countShift == 3)
                                    WorkScheduleShift3_Last.Add(tmp);
                                else if (countShift == 4)
                                    WorkScheduleShift4_Last.Add(tmp);
                            }
                        }
                        Console.WriteLine("File update table last");
                        await client.SendTextMessageAsync(Program.Users[0].Id, "File update last table");
                       
                    }
                    catch (Exception Ex)
                    {

                        await client.SendTextMessageAsync(Program.Users[0].Id, "Exception " + Ex.Message);
                    }
                    //second read table
                    try
                    {

                        ExcelReader xslReader = new(pathXLS);
                        string TableReadet = "";
                        string nameRead = "last";
                        ExcelArrayObject = new(xslReader.readX(ref TableReadet,ref nameRead));
                      
                       
                        LastUpDateWorkShedulePenunlimate = TableReadet;

                        WorkScheduleShift1 = new();
                        WorkScheduleShift2 = new();
                        WorkScheduleShift3 = new();
                        WorkScheduleShift4 = new();
                        

                        int countShift = 0;
                        for (int i = 3; i < ExcelArrayObject.Count; i++)
                        {
                            if (ExcelArrayObject[i][1] is not null)
                            {
                                if (ExcelArrayObject[i][0].ToString() == "1")
                                    countShift++;

                                string tmp = ExcelArrayObject[i][1].ToString() + ": ";
                                for (int j = 2; j < 16; j++)
                                {
                                    if (ExcelArrayObject[i][j] is not null)
                                    {
                                        if (ExcelArrayObject[i][j].ToString() == "12")
                                        {
                                            if (ExcelArrayObject[0][j].ToString() == "ніч")
                                                tmp += ExcelArrayObject[0][j - 1].ToString() + " " + ExcelArrayObject[0][j].ToString() + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                            else
                                                tmp += ExcelArrayObject[0][j].ToString() + " день " + ExcelArrayObject[1][j].ToString() + ", ";
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "в")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "В")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "Л")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                        else if (ExcelArrayObject[i][j].ToString() == "л")
                                        {
                                            tmp += ExcelArrayObject[i][j].ToString();
                                        }
                                    }

                                }

                                if (countShift == 1)
                                    WorkScheduleShift1.Add(tmp);
                                else if (countShift == 2)
                                    WorkScheduleShift2.Add(tmp);
                                else if (countShift == 3)
                                    WorkScheduleShift3.Add(tmp);
                                else if (countShift == 4)
                                    WorkScheduleShift4.Add(tmp);
                            }
                        }
                        Console.WriteLine("File update penunlimited");
                        await client.SendTextMessageAsync(Program.Users[0].Id, "File update penunlimited");
                       
                    UpdateIsComplited = true;
                    }
                    catch (Exception Ex)
                    {
                        await client.SendTextMessageAsync(Program.Users[0].Id,  Ex.Message);
                    }
                }
                catch (Exception Ex)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The file could not be read:");
                    await client.SendTextMessageAsync(Program.Users[0].Id, "The file could not be read:" + Ex.Message);
                }


            }
        }
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
          
            var msg = e.Message;
            User us = Users.Find(u => u.Id == msg.From.Id);
            if (msg.Text == null) msg.Text = "";
         
            try
            {
                if (us is not null)
                {

                    MenuUser.IncomenMessage(client,us,e);
                    
                }
                else
                {
                     
                    if (msg.Text == "0000") {
                        await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Новий користувач {msg.Chat.FirstName} {msg.Chat.LastName}\naccepted",
                            replyMarkup: MenuUser.ButtonStart()
                            );
                        if (!Users.Exists(it => it.Id == msg.From.Id))
                        {
                            User user = new User
                            {
                                Id = e.Message.From.Id,
                                Name = e.Message.From.FirstName,
                                Surname = e.Message.From.LastName,
                                CountRequest = 0
                            };
                            Users.Add(user);
                            Users[^1].Notify += DisplayMessageTelegram;
                            //dataManager.SaveData(Users);
                            dataManager.SaveDataJSON(Users);
                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(
                         msg.Chat.Id,
                         $"Новий користувач {msg.Chat.FirstName} {msg.Chat.LastName}\nenter pincode:"
                         );
                    }
                }
            }
            catch (Exception Ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, Ex.Message);
            }
        }
            private static void DisplayMessageTelegram(object sender, string e)
            {
                User user = sender as User;
                if (user is not null)
                {
                client.SendTextMessageAsync(user.Id,e);
                }
            }
        public static string WriteMessage;
        public static void WriteAllUsers()
        {
            foreach (var item in Program.Users)
            {
                client.SendTextMessageAsync(item.Id, WriteMessage);
                Thread.Sleep(1000);
            }
        }
        public static string WriteMessage2;
        public static void WriteAllUsers2()
        {
            foreach (var item in UsersOld)
            {
                client.SendTextMessageAsync(item.Id, WriteMessage2);
                Thread.Sleep(1000);

            }
        }
    }
}
