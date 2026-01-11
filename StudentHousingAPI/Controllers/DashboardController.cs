using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHousingAPI.Data;
using StudentHousingAPI.DTOs;

namespace StudentHousingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var openIssues = await _context.Issues.CountAsync(i => i.Status == "Open");
        var inProgressIssues = await _context.Issues.CountAsync(i => i.Status == "InProgress");
        var resolvedIssues = await _context.Issues.CountAsync(i => i.Status == "Resolved");
        var closedIssues = await _context.Issues.CountAsync(i => i.Status == "Closed");

        var overdueTasks = await _context.CleaningTasks
            .Where(t => t.Status == "Pending" && t.DueDate < DateTime.UtcNow)
            .CountAsync() +
            await _context.GarbageTasks
            .Where(t => t.Status == "Pending" && t.DueDate < DateTime.UtcNow)
            .CountAsync();

        var tasksDueToday = await _context.CleaningTasks
            .Where(t => t.Status == "Pending" && t.DueDate >= today && t.DueDate < tomorrow)
            .CountAsync() +
            await _context.GarbageTasks
            .Where(t => t.Status == "Pending" && t.DueDate >= today && t.DueDate < tomorrow)
            .CountAsync();

        var dashboard = new DashboardDto
        {
            OpenIssues = openIssues,
            InProgressIssues = inProgressIssues,
            ResolvedIssues = resolvedIssues,
            ClosedIssues = closedIssues,
            OverdueTasks = overdueTasks,
            TasksDueToday = tasksDueToday
        };

        return Ok(dashboard);
    }
}
