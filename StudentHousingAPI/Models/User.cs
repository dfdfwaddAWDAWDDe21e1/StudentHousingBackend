namespace StudentHousingAPI.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; } // Student, Staff, Maintenance
    public int? BuildingId { get; set; }
    public Building? Building { get; set; }
    
    public ICollection<Issue> CreatedIssues { get; set; } = new List<Issue>();
    public ICollection<CleaningTask> AssignedCleaningTasks { get; set; } = new List<CleaningTask>();
    public ICollection<GarbageTask> AssignedGarbageTasks { get; set; } = new List<GarbageTask>();
}
