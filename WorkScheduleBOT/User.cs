using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduleBOT
{
        [Serializable]
    public class User
    {
        public long Id { get; set; }
        public long CountRequest { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdSchedule { get; set; }
        

    }
}
