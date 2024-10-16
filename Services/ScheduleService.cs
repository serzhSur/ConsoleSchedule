using ConsoleSchedule.Models;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;

namespace ConsoleSchedule.Services
{
    internal class ScheduleService
    {
        private AppointmentService _appointmentService;
        
        public ScheduleService(AppointmentService appointmentService) 
        { 
            _appointmentService = appointmentService;
        }
        public async Task< List<(TimeSpan start, TimeSpan end, string status)>> CreateScheduleForUser(Master master) 
        {
            var startDayTime =  master.Start_Day_Time;// new TimeSpan(9, 0, 0);
            var endDayTime = master.End_Day_Time;//new TimeSpan(19, 00, 0);
            var intervalDay = master.Day_interval;
            List<(TimeSpan start, TimeSpan end, string status)> schedule = new List<(TimeSpan, TimeSpan, string)>();
            for (var i = startDayTime; i < endDayTime; i += intervalDay)
            {
                schedule.Add((i, i + intervalDay, "free"));
            }

            List<(TimeSpan start, TimeSpan end, string status)> occupitedTime = await _appointmentService.GetBusyTime(master.Id);

            schedule = schedule.Where(s =>
                    !occupitedTime.Any(o =>
                        s.start < o.end && s.end > o.start)).ToList();

            schedule.AddRange(occupitedTime);
            schedule.Sort((x, y) => x.start.CompareTo(y.start));

            return schedule;
        }
        public void ShowScheduleForUser(List<(TimeSpan start, TimeSpan end, string status)> schedule) 
        {
            Console.WriteLine("View for User");
            Console.WriteLine($"{"StartTime",8}{"EndTime",10}{"Status",8}");
            Console.WriteLine(new string('-', 30));
            foreach (var i in schedule)
            {
                Console.WriteLine($"{i.start.ToString(@"hh\:mm"),8} -- {i.end.ToString(@"hh\:mm"),-8}{i.status,5}");
            }
        }
        public async Task<List<AppointmentDetails>> CreateScheduleForMaster(AppointmentDetailsRepository repository, Master master) 
        {
            var appointments = new List<AppointmentDetails>(await repository.GetAll(master));
            appointments.Sort((x, y) => x.Date.CompareTo(y.Date));

            //var filteredAppointments = appointments.Where(a => a.Cancellation == false)
             //   .OrderBy(a => a.Date)
              //  .ToList();
              return appointments;
            
            
        }
        public void ShowScheduleDatail(List<AppointmentDetails> appointmentDetail) 
        {
            Console.WriteLine($"Appointments to Master: {appointmentDetail.Count}");
            Console.WriteLine($"{"Id",-5}{"Date",-25}{"End Time",-10}{"Duration",-10}{"Service Name",-20}" +
                $"{"Master Name",-20}{"User Name",-20}{"User Phone",-15}{"Cancellation",-10}");
            Console.WriteLine(new string('-', 120) + "\n"); 
            foreach (var a in appointmentDetail)
            {
                Console.WriteLine($"{a.Id,-5}{a.Date,-25}{a.Date.Add(a.Duration).ToString("HH:mm"),-10}{a.Duration,-10}" +
                 $"{a.ServiceName,-20}{a.MasterName,-20}{a.UserName,-20}{a.UserPhone,-15}{a.Cancellation,-10}");
            }
        }

    }
}
