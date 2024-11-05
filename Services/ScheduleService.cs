using VizitConsole.Models;
using VizitConsole.Repositories;

namespace VizitConsole.Services
{
    internal class ScheduleService
    {
        private AppointmentService _appointmentService;
        private AppointmentDetailsRepository _appDetailsRepo;
        public ScheduleService(string connectionString) 
        { 
            _appointmentService = new AppointmentService(connectionString);
            _appDetailsRepo = new AppointmentDetailsRepository(connectionString);
        }
        public async Task< List<(TimeSpan start, TimeSpan end, string status)>> CreateScheduleForUser(Master master) 
        {
            var startDayTime =  master.Start_day_time;
            var endDayTime = master.End_day_time;
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
            Console.WriteLine("\nView for User");
            Console.WriteLine($"{"StartTime",8}{"EndTime",10}{"Status",8}");
            Console.WriteLine(new string('-', 30));
            foreach (var i in schedule)
            {
                Console.WriteLine($"{i.start.ToString(@"hh\:mm"),8} -- {i.end.ToString(@"hh\:mm"),-8}{i.status,5}");
            }
        }
        public async Task<List<AppointmentDetails>> CreateScheduleForMaster(Master master) 
        {
            var appointments = new List<AppointmentDetails>(await _appDetailsRepo.GetAll(master));
            appointments.Sort((x, y) => x.Date.CompareTo(y.Date));
            return appointments;  
        }
        public void ShowScheduleDatail(List<AppointmentDetails> appointmentDetail) 
        {
            Console.WriteLine($"\nAppointments to Master: {appointmentDetail.Count}");
            Console.WriteLine($"{"Id",-5}{"Date",-25}{"End Time",-10}{"Duration",-10}{"Service Name",-20}" +
                $"{"Master Name",-15}{"User Name",-15}{"User Phone",-15}{"Cancellation",-10}");
            Console.WriteLine(new string('-', 127)); 
            foreach (var a in appointmentDetail)
            {
                Console.WriteLine($"{a.Id,-5}{a.Date,-25}{a.Date.Add(a.Duration).ToString("HH:mm"),-10}{a.Duration,-10}" +
                 $"{a.ServiceName,-20}{a.MasterName,-15}{a.UserName,-15}{a.UserPhone,-15}{a.Cancellation,-10}");
            }
        }
    }
}
