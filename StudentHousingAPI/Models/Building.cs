namespace StudentHousingAPI.Models;

public class Building
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    
    public ICollection<User> Residents { get; set; } = new List<User>();
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
    public ICollection<CleaningTask> CleaningTasks { get; set; } = new List<CleaningTask>();
    public ICollection<GarbageTask> GarbageTasks { get; set; } = new List<GarbageTask>();
}
