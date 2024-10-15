using ConsoleSchedule.Models;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
