using ConsoleSchedule.Models;

namespace ConsoleSchedule
{
    internal class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public int Master_id { get; set; }
        public int Service_id { get; set; }
        public int User_id { get; set; }
        public bool Cancellation { get; set; } = false;

        public Appointment() { }
        public Appointment(DateTime dateTime, Master master, Service service, User user)
        {
            Date = dateTime;
            Duration = service.Duration;
            Master_id = master.Id;
            Service_id = service.Id;
            User_id = user.Id;
        }

    }
}
