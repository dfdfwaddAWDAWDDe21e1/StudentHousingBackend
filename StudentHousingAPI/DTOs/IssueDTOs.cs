namespace StudentHousingAPI.DTOs;

public class CreateIssueRequest
{
    public required string Description { get; set; }
    public string? SharedSpace { get; set; }
    public string? PhotoProof { get; set; }
    public int BuildingId { get; set; }
}

public class UpdateIssueRequest
{
    public string? Status { get; set; }
    public int? AssignedToUserId { get; set; }
}

public class IssueDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public string? SharedSpace { get; set; }
    public string? PhotoProof { get; set; }
    public required string Status { get; set; }
    public int CreatedByUserId { get; set; }
    public string? CreatedByUsername { get; set; }
    public int BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
