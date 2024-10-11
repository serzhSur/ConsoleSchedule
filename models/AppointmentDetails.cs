
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
            Console.WriteLine($"{a.Id,-5}{a.Date,-25}{a.Date.Add(a.Duration).ToString("HH:mm"),-10}{a.Duration,-10}" +
                $"{a.ServiceName,-20}{a.MasterName,-20}{a.UserName,-20}{a.UserPhone,-15}{a.Cancellation,-10}");
        }
    }
    
}
