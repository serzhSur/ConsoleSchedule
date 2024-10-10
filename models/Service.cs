
namespace ConsoleSchedule.Models
{
    internal class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public int Master_id { get; set; }
    }
}
