using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSchedule.models
{
    internal class AppointmentDetails: Appointment
    {
        public string MasterName {  get; set; }
        public string ServiceName {  get; set; }   
        public TimeSpan ServiceDuration { get; set; }
        public TimeSpan EndTime { get; set; }
        public string UserName {  get; set; }
        public string UserPhone {  get; set; }
    }
}
