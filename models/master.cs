using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSchedule.models
{
    internal class  Master
    {
        public int Id { get; set; }
        public string Name { get; set; } = "undefined";
        public string Speciality { get; set; }
        public TimeSpan Day_interval { get; set; }
    }
}
