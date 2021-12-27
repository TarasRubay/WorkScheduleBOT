using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
namespace WorkScheduleBOT
{
    [Serializable]
    
    public class Shift
    {
        public delegate void ShiftHandler(string message);
        public event ShiftHandler Notify;              // 1.Определение события
        public string NameOfShift { get; set; }
        public string NameOfOwner { get; set; }
        public string Week { get; set; }
        public DateTime Date { get; set; }
        public string positionShift;
        public string PositionShift { get => positionShift;
            set
            {
                if(positionShift != value)
                {
                    positionShift = value;
                    Notify?.Invoke($"Оновленння!!!!!! тиждень {Week}\n{Date.ToShortDateString()} {NameOfShift} {PositionShift}");
                }
            }
        }
        public override string ToString()
        {
            string tmp = "00/00/0000 понеділок день ";
            return $"{Date.ToShortDateString()} {NameOfShift}{new string(' ',(tmp.Length - NameOfShift.Length - Date.ToShortDateString().Length) *2)}{PositionShift}";
        }
        public static bool operator ==(Shift a, Shift b)
        {
            if (a.Date == b.Date && a.NameOfShift == b.NameOfShift) return true;
            else return false;
        }
        public static bool operator !=(Shift a, Shift b)
        {
            if (a.Date == b.Date && a.NameOfShift == b.NameOfShift) return false;
            else return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
