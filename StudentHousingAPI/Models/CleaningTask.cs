namespace StudentHousingAPI.Models;

public class CleaningTask
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required string SharedSpace { get; set; }
    public required string Status { get; set; } // Pending, Completed, Verified
    public int BuildingId { get; set; }
    public Building? Building { get; set; }
    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
