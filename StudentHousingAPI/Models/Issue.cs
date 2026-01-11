namespace StudentHousingAPI.Models;

public class Issue
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public string? SharedSpace { get; set; }
    public string? PhotoProof { get; set; } // Optional: Base64 encoded or file path
    public required string Status { get; set; } // Open, InProgress, Resolved, Closed
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    public int BuildingId { get; set; }
    public Building? Building { get; set; }
    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
