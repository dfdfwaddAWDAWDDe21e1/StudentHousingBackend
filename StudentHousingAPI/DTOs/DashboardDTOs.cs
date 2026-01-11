namespace StudentHousingAPI.DTOs;

public class DashboardDto
{
    public int OpenIssues { get; set; }
    public int InProgressIssues { get; set; }
    public int ResolvedIssues { get; set; }
    public int ClosedIssues { get; set; }
    public int OverdueTasks { get; set; }
    public int TasksDueToday { get; set; }
}
