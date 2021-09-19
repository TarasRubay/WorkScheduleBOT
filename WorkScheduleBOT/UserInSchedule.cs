using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
    [Serializable]
    public class UserInSchedule 
    {
        public delegate void UserScheduleShiftHandler(string message);
        public event UserScheduleShiftHandler NotifyShift;
        public delegate void AddNewWeekHandler(string message);
        public event AddNewWeekHandler NotifyNewWeek;
        public delegate void HospitalNotHandler(string message);
        public event HospitalNotHandler NotifyHospital;


        public string Name { get; set; }
        public int Shift { get; set; }

        public List<Shift> Schedule {  get; private set; } = new();

        public void AddShift(Shift shift)
        {
            Schedule.Add(shift);
            Schedule[^1].Notify += ShiftChange;
        }
        

        public string ViewShifts()
        {
            
            string seq = $"{Name} [зм {Shift}]\n";
            foreach (var item in Schedule)
            {
                
                
                if (item.PositionShift != "вільно" && item.Date > DateTime.Now)
                {
                    seq += item + "\n";
                    
                }
                
            }
            return seq;
        }
        public override string ToString()
        {
            return ViewShifts();
        }
        public void ShiftChange(string message)
        {

            NotifyShift?.Invoke($"{Name}\n{message}");
        }
        public void MailingByAddNewWeek()
        {
            var lastWeek = Program.ListWeeks[^1];
            string message = "";
            foreach (var item in Schedule)
            {
                if (item.Week == lastWeek && item.positionShift != "вільно")
                {    
                    message += item + "\n";
                }
            }
            NotifyNewWeek?.Invoke($"{Name}\nдодано {lastWeek} тиждень\n{message}");
        }
        public void MailingHospital()
        {  
            NotifyHospital?.Invoke($"{Name}\n лікарняне");
        }
    }
}
