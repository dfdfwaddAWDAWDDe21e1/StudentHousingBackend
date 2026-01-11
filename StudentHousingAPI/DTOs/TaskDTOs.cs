namespace StudentHousingAPI.DTOs;

public class CreateTaskRequest
{
    public required string Description { get; set; }
    public required string LocationOrSpace { get; set; }
    public int BuildingId { get; set; }
    public int? AssignedUserId { get; set; }
    public DateTime DueDate { get; set; }
}

public class CompleteTaskRequest
{
    public int TaskId { get; set; }
}

public class VerifyTaskRequest
{
    public int TaskId { get; set; }
}

public class CleaningTaskDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required string SharedSpace { get; set; }
    public required string Status { get; set; }
    public int BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public int? AssignedUserId { get; set; }
    public string? AssignedUsername { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GarbageTaskDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public required string Status { get; set; }
    public int BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public int? AssignedUserId { get; set; }
    public string? AssignedUsername { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
