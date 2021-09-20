using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace WorkScheduleBOT
{
    public static class Menu
    {

        public static async void ViewShift(TelegramBotClient client, MessageEventArgs e, string LastUpDateWorkShedule, List<string> WorkScheduleShift)
        {
            var msg = e.Message;
            try
            {
                
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, ex.ToString());
            }
        }
        public static async void CreatingUsersInScheduleFirstStart(TelegramBotClient client, List<List<object>> ExcelArrayObject, string nameRead,string listTable)
        {
            try
            {

            List<UserInSchedule> userIn = new();
            var arrNameRead = nameRead.Split(" ");
           
            
            string WeekNow = arrNameRead[2];
            int YearNow = Int32.Parse(arrNameRead[5]);
            int MountNow = 0;
            foreach (Program.MountsUA mount in (Program.MountsUA[])Enum.GetValues(typeof(Program.MountsUA)))
            {
                MountNow++;
                if (mount.ToString() == arrNameRead[4].ToLower()) break;
            }
                //List<string> arrDayNum = new();
                int indFirstNum = -1;
            for (int i = 2; i < 16; i++)
            {
                    if (ExcelArrayObject[1][i].ToString() == "1" || ExcelArrayObject[1][i].ToString() == "01") indFirstNum = i;
                   // if (ExcelArrayObject[1][i].ToString() is not "")
                       // arrDayNum.Add(ExcelArrayObject[1][i].ToString());
            }
            int countShift = 0;
            for (int i = 3; i < ExcelArrayObject.Count; i++)
            {
                if (ExcelArrayObject[i][1] is not null)
                {


                    if (ExcelArrayObject[i][0].ToString() == "1")
                        countShift++;
                    string tmp = ExcelArrayObject[i][1].ToString();
                    UserInSchedule usSch = new UserInSchedule
                    {
                        Name = ExcelArrayObject[i][1].ToString(),
                        
                        Shift = countShift,
                       
                    };
                    for (int j = 2; j < 16; j++)
                    {
                        if (ExcelArrayObject[i][j] is not null)
                        {
                            Shift shift = new();
                                if (indFirstNum == -1 || j >= indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString() + " " + ExcelArrayObject[0][j].ToString();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j - 1].ToString()));
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j].ToString()));
                                    }
                                }else if(j < indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString() + " " + ExcelArrayObject[0][j].ToString();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow-1, Int32.Parse(ExcelArrayObject[1][j - 1].ToString()));
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow-1, Int32.Parse(ExcelArrayObject[1][j].ToString()));
                                    }
                                }


                            if (ExcelArrayObject[i][j].ToString().Length > 0)
                            {
                                shift.PositionShift =  ExcelArrayObject[i][j].ToString();
                            }
                            else
                            {
                                shift.PositionShift = "вільно";
                            }
                            shift.Week = WeekNow;
                            usSch.AddShift(shift);
                        }

                    }

                    if (countShift == 1)
                    {
                        usSch.Shift = 1;
                        userIn.Add(usSch); 
                    }
                    else if (countShift == 2)
                    {
                        usSch.Shift = 2;
                            userIn.Add(usSch);
                        }
                    else if (countShift == 3)
                    {
                        usSch.Shift = 3;
                            userIn.Add(usSch);
                        }
                    else if (countShift == 4)
                    {
                        usSch.Shift = 4;
                            userIn.Add(usSch);
                        }
                }
                    if (listTable == "last") Program.userInSchedulesLastWeek = userIn;
                    else if (listTable == "last-1") Program.userInSchedulesPenunlimateWeek = userIn;
            }
            }
            catch (Exception Ex)
            {

                await client.SendTextMessageAsync(Program.Users[0].Id, Ex.Message);

            }
        }

        public static async void ViewWeek(TelegramBotClient client,long chatId, string message,int shift)
        {
            try
            {
                message = message.Split(" ")[0];
               if(message == Program.userInSchedulesLastWeek[0].Schedule[0].Week)
                {
                    foreach (var us in Program.userInSchedulesLastWeek)
                    {
                        if (us.Shift == shift)
                        {
                            await client.SendTextMessageAsync(chatId, us.ViewShifts());
                        }
                    }
                }
                else if (message == Program.userInSchedulesPenunlimateWeek[0].Schedule[0].Week)
                {
                    foreach (var us in Program.userInSchedulesPenunlimateWeek)
                    {
                        if (us.Shift == shift)
                        {
                            await client.SendTextMessageAsync(chatId, us.ViewShifts());
                        }
                    }
                }



            }
            catch (Exception Ex)
            {
                await client.SendTextMessageAsync(Program.Users[0].Id, Ex.Message);
            }
        }
        public static void CreatingUsersInScheduleFirstStartNew(TelegramBotClient client, List<List<object>> ExcelArrayObject, string nameRead)
        {
            try
            {

               
                var arrNameRead = nameRead.Split(" ");
                string WeekNow = arrNameRead[2].Trim();
                int YearNow = Int32.Parse(arrNameRead[5]);
                int MountNow = 0;
                foreach (Program.MountsUA mount in (Program.MountsUA[])Enum.GetValues(typeof(Program.MountsUA)))
                {
                    MountNow++;
                    if (mount.ToString() == arrNameRead[4].ToLower()) break;
                }
                Program.ListWeeks.Add(WeekNow);
                int indFirstNum = -1;
                for (int i = 2; i < 16; i++)
                {
                    if (ExcelArrayObject[1][i].ToString().Trim() == "1" || ExcelArrayObject[1][i].ToString().Trim() == "01") indFirstNum = i;
                }
                int countShift = 0;
                for (int i = 3; i < ExcelArrayObject.Count; i++)
                {
                    if (ExcelArrayObject[i][1] is not null)
                    {


                        if (ExcelArrayObject[i][0].ToString().Trim() == "1")
                            countShift++;
                        string tmp = ExcelArrayObject[i][1].ToString().Trim();
                        
                        UserInSchedule usSch = new UserInSchedule
                        {
                            Name = tmp,

                            Shift = countShift,
                            
                        };
                        for (int j = 2; j < 16; j++)
                        {
                            if (ExcelArrayObject[i][j] is not null)
                            {
                                Shift shift = new();
                                if (indFirstNum == -1 || j >= indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString().Trim() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString().Trim() + " " + ExcelArrayObject[0][j].ToString().Trim();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j - 1].ToString().Trim()), 23, 59, 59);
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString().Trim() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j].ToString().Trim()), 23, 59, 59);
                                    }
                                }
                                else if (j < indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString().Trim() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString().Trim() + " " + ExcelArrayObject[0][j].ToString().Trim();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow - 1, Int32.Parse(ExcelArrayObject[1][j - 1].ToString().Trim()), 23, 59, 59);
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString().Trim() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow - 1, Int32.Parse(ExcelArrayObject[1][j].ToString().Trim()), 23, 59, 59);
                                    }
                                }


                                if (ExcelArrayObject[i][j].ToString().Length > 0)
                                {
                                    shift.positionShift = ExcelArrayObject[i][j].ToString().Trim();
                                }
                                else
                                {
                                    shift.positionShift = "вільно";
                                }
                                shift.Week = WeekNow;

                                shift.NameOfOwner = tmp;
                                usSch.AddShift(shift);
                                //usSch.Schedule.Add(shift);
                            }

                        }

                        if (countShift == 1)
                        {
                            usSch.Shift = 1;
                            Program.ListUserSchedule.Add(usSch);
                        }
                        else if (countShift == 2)
                        {
                            usSch.Shift = 2;
                            Program.ListUserSchedule.Add(usSch);
                        }
                        else if (countShift == 3)
                        {
                            usSch.Shift = 3;
                            Program.ListUserSchedule.Add(usSch);
                        }
                        else if (countShift == 4)
                        {
                            usSch.Shift = 4;
                            Program.ListUserSchedule.Add(usSch);
                        }
                    }
                     
                   
                }
                
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                //await client.SendTextMessageAsync(Program.Users[0].Id, Ex.Message);

            }
            
        }
        public static List<string> CreatingUsersInScheduleSecondStartNew(TelegramBotClient client, List<List<object>> ExcelArrayObject, string nameRead)
        {
            List<string> listHospital = new();
            try
            {
                var arrNameRead = nameRead.Split(" ");
                string WeekNow = arrNameRead[2].Trim();
                int YearNow = Int32.Parse(arrNameRead[5]);
                int MountNow = 0;
                foreach (Program.MountsUA mount in (Program.MountsUA[])Enum.GetValues(typeof(Program.MountsUA)))
                {
                    MountNow++;
                    if (mount.ToString() == arrNameRead[4].ToLower()) break;
                }
                
                Program.ListWeeks.Add(WeekNow);
                
                int indFirstNum = -1;
                for (int i = 2; i < 16; i++)
                {
                    if (ExcelArrayObject[1][i].ToString().Trim() == "1" || ExcelArrayObject[1][i].ToString().Trim() == "01") indFirstNum = i;
                }
                int countShift = 0;
                for (int i = 3; i < ExcelArrayObject.Count; i++)
                {
                    if (ExcelArrayObject[i][1] is not null)
                    {
                        bool checkHospital = false;
                        if (ExcelArrayObject[i][0].ToString().Trim() == "1")
                            countShift++;
                        string tmp = ExcelArrayObject[i][1].ToString().Trim();
                       
                        
                        for (int j = 2; j < 16; j++)
                        {
                            if (ExcelArrayObject[i][j] is not null)
                            {
                                
                                Shift shift = new();
                                if (indFirstNum == -1 || j >= indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString().Trim() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString().Trim() + " " + ExcelArrayObject[0][j].ToString().Trim();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j - 1].ToString().Trim()), 23, 59, 59);
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString().Trim().Trim() + " день ";
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j].ToString().Trim()), 23, 59, 59);
                                    }
                                }
                                else if (j < indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString().Trim() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString().Trim() + " " + ExcelArrayObject[0][j].ToString().Trim();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow - 1, Int32.Parse(ExcelArrayObject[1][j - 1].ToString().Trim()), 23, 59, 59);
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString().Trim().Trim() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow - 1, Int32.Parse(ExcelArrayObject[1][j].ToString().Trim()), 23, 59, 59);
                                    }
                                }


                                if (ExcelArrayObject[i][j].ToString().Length > 0)
                                {
                                    shift.positionShift = ExcelArrayObject[i][j].ToString().Trim();
                                    if (ExcelArrayObject[i][j].ToString().ToLower().Trim() == "л") checkHospital = true;
                                }
                                else
                                {
                                    shift.positionShift = "вільно";
                                }
                                shift.Week = WeekNow;
                                
                                if (Program.ListUserSchedule.Find(us => us.Name == tmp) == null)
                                {
                                    UserInSchedule usSch = new UserInSchedule
                                    {
                                        Name = tmp,
                                        Shift = countShift,
                                    };
                                    shift.NameOfOwner = tmp;
                                    usSch.AddShift(shift);
                                    Program.ListUserSchedule.Add(usSch);
                                }
                                else
                                {
                                    var user = Program.ListUserSchedule.Find(us => us.Name == tmp);
                                    shift.NameOfOwner = tmp;
                                    user.AddShift(shift);
                                }

                                
                            }

                        }
                        if (checkHospital) listHospital.Add(tmp);
                    }


                }
            }
            catch (Exception Ex)
            {

                //await client.SendTextMessageAsync(Program.Users[0].Id, Ex.Message);
                Console.WriteLine(Ex.Message);
            }
            return listHospital;
        }
        public static List<string> CheckUpdateInScedule(TelegramBotClient client, List<List<object>> ExcelArrayObject, string nameRead)
        {
            List<string> listHospital = new();
            try
            {
                var arrNameRead = nameRead.Split(" ");
                string WeekNow = arrNameRead[2].Trim();
                int YearNow = Int32.Parse(arrNameRead[5]);
                int MountNow = 0;
                foreach (Program.MountsUA mount in (Program.MountsUA[])Enum.GetValues(typeof(Program.MountsUA)))
                {
                    MountNow++;
                    if (mount.ToString() == arrNameRead[4].ToLower()) break;
                }


                int indFirstNum = -1;
                for (int i = 2; i < 16; i++)
                {
                    if (ExcelArrayObject[1][i].ToString().Trim() == "1" || ExcelArrayObject[1][i].ToString().Trim() == "01") indFirstNum = i;
                }
                int countShift = 0;
                for (int i = 3; i < ExcelArrayObject.Count; i++)
                {
                    if (ExcelArrayObject[i][1] is not null)
                    {
                        bool checkHospital = false;

                        if (ExcelArrayObject[i][0].ToString().Trim() == "1")
                            countShift++;
                        string tmp = ExcelArrayObject[i][1].ToString().Trim();

                        for (int j = 2; j < 16; j++)
                        {
                            if (ExcelArrayObject[i][j] is not null)
                            {
                                Shift shift = new();
                                if (indFirstNum == -1 || j >= indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString().Trim() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString().Trim() + " " + ExcelArrayObject[0][j].ToString().Trim();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j - 1].ToString().Trim()),23,59,59);
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString().Trim() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow, Int32.Parse(ExcelArrayObject[1][j].ToString().Trim()), 23, 59, 59);
                                    }
                                }
                                else if (j < indFirstNum)
                                {
                                    if (ExcelArrayObject[0][j].ToString().Trim() == "ніч")
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j - 1].ToString().Trim() + " " + ExcelArrayObject[0][j].ToString().Trim();// + " " + ExcelArrayObject[1][j - 1].ToString() + ", ";
                                        shift.Date = new(YearNow, MountNow - 1, Int32.Parse(ExcelArrayObject[1][j - 1].ToString().Trim()), 23, 59, 59);
                                    }
                                    else
                                    {
                                        shift.NameOfShift = ExcelArrayObject[0][j].ToString().Trim() + " день ";// + ExcelArrayObject[1][j].ToString();
                                        shift.Date = new(YearNow, MountNow - 1, Int32.Parse(ExcelArrayObject[1][j].ToString().Trim()), 23, 59, 59);
                                    }
                                }


                                if (ExcelArrayObject[i][j].ToString().Length > 0)
                                {
                                    shift.positionShift = ExcelArrayObject[i][j].ToString().Trim();
                                    if (ExcelArrayObject[i][j].ToString().ToLower().Trim() == "л") checkHospital = true;
                                }
                                else
                                {
                                    shift.positionShift = "вільно";
                                }
                                shift.Week = WeekNow;

                                if(Program.ListUserSchedule.Find(us => us.Name == tmp) == null)
                                {
                                    UserInSchedule usSch = new UserInSchedule
                                    {
                                        Name = tmp,
                                        Shift = countShift,
                                    };
                                    shift.NameOfOwner = tmp;
                                    usSch.AddShift(shift);
                                    Program.ListUserSchedule.Add(usSch);
                                }
                                else
                                {
                                    var user = Program.ListUserSchedule.Find(us => us.Name == tmp);
                                    var shi = user.Schedule.Find(sh => sh == shift);

                                    if (shi is not null && shi.PositionShift != shift.PositionShift) shi.PositionShift = shift.PositionShift;
                                }
                            }

                        }
                        if(checkHospital)listHospital.Add(tmp);
                    }


                }
            }
            catch (Exception Ex)
            {
                //await client.SendTextMessageAsync(Program.Users[0].Id, Ex.Message);
                Console.WriteLine(Ex.Message);
            }
            return listHospital;
        }
        
    }
}
