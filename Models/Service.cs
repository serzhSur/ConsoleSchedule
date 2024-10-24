
namespace ConsoleSchedule.Models
{
    internal class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public int Master_id { get; set; }
        public Service() 
        { 
        }
        public Service(string name, TimeSpan duration, Master master) 
        { 
            Name = name;
            Duration = duration;
            Master_id = master.Id;
        }
    }
}
