
namespace ConsoleSchedule.Models
{
    internal class AppointmentDetails: Appointment
    {
        public string MasterName {  get; set; }
        public string ServiceName {  get; set; }   
        public TimeSpan ServiceDuration { get; set; }
        public TimeSpan EndTime { get; set; }
        public string UserName {  get; set; }
        public string UserPhone {  get; set; }
        
       
        public void ShowConsole(AppointmentDetails a)
        {
            Console.WriteLine($"{a.Id}\t{a.Date}\t{a.Date.Add(a.Duration).ToString("HH:mm")}\t{a.Duration}" +
                $"\t{a.ServiceName}\t{a.MasterName}\t{a.UserName}\t\t{a.UserPhone}\t{a.Cancellation}");
        }
    }
    
}
