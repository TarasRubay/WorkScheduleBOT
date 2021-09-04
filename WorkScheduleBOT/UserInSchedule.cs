using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
    public class UserInSchedule
    {
        public string Name { get; set; }
        public int Shift { get; set; }
        public List<string> Schedule{ get; set; }
    }
}
