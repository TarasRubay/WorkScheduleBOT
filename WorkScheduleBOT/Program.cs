using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Runtime.InteropServices;
using System.Timers;
using System.IO;

namespace WorkScheduleBOT
{
    [Serializable]
    class Program
    {
        private static Timer aTimer;
        private static DateTime LastUpdateExcel;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        public static double StillProcent { get; set; }
        public static List<User> Users { get; set; }
        public static List<List<object>> ExcelArrayObject;
        public static string LastUpDateWorkShedule;
        public static List<string> WorkScheduleShift1;
        public static List<string> WorkScheduleShift2;
        public static List<string> WorkScheduleShift3;
        public static List<string> WorkScheduleShift4;
        private static string token { get; set; } = "1912296215:AAHmxbSt7HtFRMTxiwLJ4okS6ummvUfu0Pg";//specialfeaturespp

        //private static string token { get; set; } = "1953282621:AAFyelbHR2cqIQVsLD2TUp1awTJPj4Nqbq8";//calculateSalary
        private static TelegramBotClient client;
        //public static Menu menu { get; set; }

        public static DataManager dataManager;
        public static string path = @"tel.bin";
        public static string pathStillProcent = @"StillProcent.bin";
        public static string pathXLS = @"2 ГРАФІК ОПЕРАТОРИ.xlsx";
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            dataManager = new(path, pathStillProcent);
            Users = new();
            Users = dataManager.LoadData();
            StillProcent = dataManager.LoadDataProcent();
            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 2000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;

            try
            {
                client = new TelegramBotClient(token);
                client.StartReceiving();
                client.OnMessage += OnMessageHandler;
                Console.WriteLine("bot started");
                //Console.ReadKey();
                // Hide
                //ShowWindow(handle, SW_HIDE);

                // Show
                ShowWindow(handle, SW_SHOW);
                Console.Read();
                client.StopReceiving();
            }
            catch (Exception)
            {}
            dataManager.SaveData(Users);
            dataManager.SaveDataStillProcent(StillProcent);
        }
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine($"{LastUpdateExcel} {e.SignalTime}");
            if (LastUpdateExcel < new FileInfo(pathXLS).LastWriteTime)
            {
                try
                {
                    // Create an instance of StreamReader to read from a file.
                    // The using statement also closes the StreamReader.
                    using (StreamReader sr = new StreamReader(pathXLS))
                    {
                        Console.WriteLine("The file be read:");
                        LastUpdateExcel = new FileInfo(pathXLS).LastWriteTime;
                    }
                    try
                    {

                        ExcelReader xslReader = new(pathXLS);
                        string ret = "";
                        ExcelArrayObject = new(xslReader.readX(ref ret));
                        LastUpDateWorkShedule = DateTime.Now.ToString() + "              " + ret;

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
                        Console.WriteLine("File update");

                    }
                    catch (Exception)
                    {


                    }
                }
                catch (Exception ex)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The file could not be read:");
                    //Console.WriteLine(ex.Message);
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
                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"виберіть зміну",
                            replyMarkup: Shifts()
                            );
                    if (msg.Text.Contains("users"))
                    {
                        int num = 1;
                        foreach (var item in Program.Users)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, $"\n {num++} {item.Name} {item.Surname}");
                        }
                    }
                    else if (msg.Text.Contains("переглянути розклад"))
                    {
                        Menu.ViewSchedule(client, e, LastUpDateWorkShedule);
                    }
                    else if (msg.Text.Contains("зміна 1"))
                    {
                        Menu.ShiftOne(client, e, LastUpDateWorkShedule, WorkScheduleShift1);
                    }
                    else if (msg.Text.Contains("зміна 2"))
                    {
                        Menu.ShiftTwo(client, e, LastUpDateWorkShedule, WorkScheduleShift2);
                    }
                    else if (msg.Text.Contains("зміна 3"))
                    {
                        Menu.ShiftThree(client, e, LastUpDateWorkShedule, WorkScheduleShift3);
                    }
                    else if (msg.Text.Contains("зміна 4"))
                    {
                        Menu.ShiftFour(client, e, LastUpDateWorkShedule, WorkScheduleShift4);
                    }
                }
                else
                {
                        await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Новий користувач {msg.Chat.FirstName} {msg.Chat.LastName}",
                            replyMarkup: Shifts()
                            );
                        if (!Users.Exists(it => it.Id == msg.From.Id))
                        {
                            Users.Add(new User
                            {
                                Id = e.Message.From.Id,
                                Name = e.Message.From.FirstName,
                                Surname = e.Message.From.LastName
                            });
                            dataManager.SaveData(Users);
                        }
                }
            }
            catch (Exception)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "wrong comand");
            }
        }

        private static IReplyMarkup GetButtonsMenu()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    
                    new List<KeyboardButton>{ new KeyboardButton {Text = "переглянути розклад" } }

                }

            };
        }
        private static IReplyMarkup Shifts()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 1" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 2" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 3" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 4" } }

                }

            };
        }

    }
}
