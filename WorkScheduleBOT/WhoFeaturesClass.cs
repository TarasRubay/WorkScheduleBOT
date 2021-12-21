using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace WorkScheduleBOT
{
    class WhoFeaturesClass
    {
        public static string Exit { get; private set; }

        public static IReplyMarkup GetAvailableListData()
        {
            List<List<KeyboardButton>> l = new();
            List<KeyboardButton> button = new();
            button.Add(new KeyboardButton { Text = Exit });
            l.Add(button);

            if (Program.ListUserSchedule is not null)
                
                    foreach (var shift in Program.ListUserSchedule.FirstOrDefault().Schedule)
                    {
                    List<KeyboardButton> buttons = new();
                    buttons.Add(new KeyboardButton { Text = "jyh" });
                    l.Add(buttons);
                    }
                

            return new ReplyKeyboardMarkup
            {
                Keyboard = l
            };

        }
        public static string WhoToday(string inMassage)
        {
            string vs = "";
            string nameShift;
            if (inMassage == MenuUser.WhoTodayDay) nameShift = "день";
            else nameShift = "ніч";
            foreach (var item in Program.ListUserSchedule)
            {
                foreach (var shift in item.Schedule)
                {
                    if (shift.Date.ToShortDateString() == DateTime.Now.ToShortDateString() && shift.positionShift == "12" && shift.NameOfShift.Split(' ')[1] == nameShift)
                        vs += $"{shift.NameOfShift.Split(' ')[1]} {shift.NameOfOwner}\n";
                }
            }
            return vs;
        }
        public static string WhoTomorrow(string inMassage)
        {
            string vs = "";
            string nameShift;
            if (inMassage == MenuUser.WhoTomorrowDay) nameShift = "день";
            else nameShift = "ніч";
            foreach (var item in Program.ListUserSchedule)
            {
                foreach (var shift in item.Schedule)
                {
                    if (shift.Date.ToShortDateString() == DateTime.Now.AddDays(1).ToShortDateString() && shift.positionShift == "12" && shift.NameOfShift.Split(' ')[1] == nameShift)
                        vs += $"{shift.NameOfShift.Split(' ')[1]} {shift.NameOfOwner}\n";
                }
            }
            return vs;
        }
        public static string WhoYesterday(string inMassage)
        {
            string vs = "";
            string nameShift;
            if (inMassage == MenuUser.WhoYesterdayDay) nameShift = "день";
            else nameShift = "ніч";
            foreach (var item in Program.ListUserSchedule)
            {
                foreach (var shift in item.Schedule)
                {
                    if (shift.Date.ToShortDateString() == DateTime.Now.AddDays(-1).ToShortDateString() && shift.positionShift == "12" && shift.NameOfShift.Split(' ')[1] == nameShift)
                        vs += $"{shift.NameOfShift.Split(' ')[1]} {shift.NameOfOwner}\n";
                }
            }
            return vs;
        }
    }
}
