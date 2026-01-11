using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHousingAPI.Data;
using StudentHousingAPI.DTOs;
using StudentHousingAPI.Models;
using System.Security.Claims;

namespace StudentHousingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IssuesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public IssuesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues(
        [FromQuery] string? status = null,
        [FromQuery] int? buildingId = null)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        var query = _context.Issues
            .Include(i => i.CreatedByUser)
            .Include(i => i.Building)
            .Include(i => i.AssignedToUser)
            .AsQueryable();

        // Students can only see their own issues
        if (userRole == "Student")
        {
            query = query.Where(i => i.CreatedByUserId == userId);
        }

        // Apply filters
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(i => i.Status == status);
        }

        if (buildingId.HasValue)
        {
            query = query.Where(i => i.BuildingId == buildingId.Value);
        }

        var issues = await query
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var issueDtos = issues.Select(i => new IssueDto
        {
            Id = i.Id,
            Description = i.Description,
            SharedSpace = i.SharedSpace,
            PhotoProof = i.PhotoProof,
            Status = i.Status,
            CreatedByUserId = i.CreatedByUserId,
            CreatedByUsername = i.CreatedByUser?.Username,
            BuildingId = i.BuildingId,
            BuildingName = i.Building?.Name,
            AssignedToUserId = i.AssignedToUserId,
            AssignedToUsername = i.AssignedToUser?.Username,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        });

        return Ok(issueDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IssueDto>> GetIssue(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        var issue = await _context.Issues
            .Include(i => i.CreatedByUser)
            .Include(i => i.Building)
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null)
        {
            return NotFound();
        }

        // Students can only view their own issues
        if (userRole == "Student" && issue.CreatedByUserId != userId)
        {
            return Forbid();
        }

        var issueDto = new IssueDto
        {
            Id = issue.Id,
            Description = issue.Description,
            SharedSpace = issue.SharedSpace,
            PhotoProof = issue.PhotoProof,
            Status = issue.Status,
            CreatedByUserId = issue.CreatedByUserId,
            CreatedByUsername = issue.CreatedByUser?.Username,
            BuildingId = issue.BuildingId,
            BuildingName = issue.Building?.Name,
            AssignedToUserId = issue.AssignedToUserId,
            AssignedToUsername = issue.AssignedToUser?.Username,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt
        };

        return Ok(issueDto);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IssueDto>> CreateIssue(CreateIssueRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var building = await _context.Buildings.FindAsync(request.BuildingId);
        if (building == null)
        {
            return BadRequest(new { message = "Building not found" });
        }

        var issue = new Issue
        {
            Description = request.Description,
            SharedSpace = request.SharedSpace,
            PhotoProof = request.PhotoProof,
            Status = "Open",
            CreatedByUserId = userId,
            BuildingId = request.BuildingId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        // Load navigation properties
        await _context.Entry(issue)
            .Reference(i => i.CreatedByUser)
            .LoadAsync();
        await _context.Entry(issue)
            .Reference(i => i.Building)
            .LoadAsync();

        var issueDto = new IssueDto
        {
            Id = issue.Id,
            Description = issue.Description,
            SharedSpace = issue.SharedSpace,
            PhotoProof = issue.PhotoProof,
            Status = issue.Status,
            CreatedByUserId = issue.CreatedByUserId,
            CreatedByUsername = issue.CreatedByUser?.Username,
            BuildingId = issue.BuildingId,
            BuildingName = issue.Building?.Name,
            AssignedToUserId = issue.AssignedToUserId,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt
        };

        return CreatedAtAction(nameof(GetIssue), new { id = issue.Id }, issueDto);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<IssueDto>> UpdateIssue(int id, UpdateIssueRequest request)
    {
        var issue = await _context.Issues
            .Include(i => i.CreatedByUser)
            .Include(i => i.Building)
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null)
        {
            return NotFound();
        }

        if (request.Status != null)
        {
            issue.Status = request.Status;
        }

        if (request.AssignedToUserId.HasValue)
        {
            var assignedUser = await _context.Users.FindAsync(request.AssignedToUserId.Value);
            if (assignedUser == null)
            {
                return BadRequest(new { message = "Assigned user not found" });
            }
            issue.AssignedToUserId = request.AssignedToUserId.Value;
        }

        issue.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Reload navigation properties
        await _context.Entry(issue)
            .Reference(i => i.AssignedToUser)
            .LoadAsync();

        var issueDto = new IssueDto
        {
            Id = issue.Id,
            Description = issue.Description,
            SharedSpace = issue.SharedSpace,
            PhotoProof = issue.PhotoProof,
            Status = issue.Status,
            CreatedByUserId = issue.CreatedByUserId,
            CreatedByUsername = issue.CreatedByUser?.Username,
            BuildingId = issue.BuildingId,
            BuildingName = issue.Building?.Name,
            AssignedToUserId = issue.AssignedToUserId,
            AssignedToUsername = issue.AssignedToUser?.Username,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt
        };

        return Ok(issueDto);
    }
}
