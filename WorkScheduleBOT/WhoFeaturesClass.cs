using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
    class WhoFeaturesClass
    {
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
