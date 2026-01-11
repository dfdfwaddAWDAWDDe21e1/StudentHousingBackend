using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHousingAPI.Data;
using StudentHousingAPI.DTOs;
using StudentHousingAPI.Models;
using StudentHousingAPI.Extensions;

namespace StudentHousingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Cleaning Tasks
    [HttpGet("cleaning")]
    public async Task<ActionResult<IEnumerable<CleaningTaskDto>>> GetCleaningTasks()
    {
        var userId = User.GetUserId();
        var userRole = User.GetUserRole();

        var query = _context.CleaningTasks
            .Include(t => t.Building)
            .Include(t => t.AssignedUser)
            .AsQueryable();

        // Students can only see tasks assigned to them
        if (userRole == "Student")
        {
            query = query.Where(t => t.AssignedUserId == userId);
        }

        var tasks = await query
            .OrderBy(t => t.DueDate)
            .ToListAsync();

        var taskDtos = tasks.Select(t => new CleaningTaskDto
        {
            Id = t.Id,
            Description = t.Description,
            SharedSpace = t.SharedSpace,
            Status = t.Status,
            BuildingId = t.BuildingId,
            BuildingName = t.Building?.Name,
            AssignedUserId = t.AssignedUserId,
            AssignedUsername = t.AssignedUser?.Username,
            DueDate = t.DueDate,
            CompletedAt = t.CompletedAt,
            CreatedAt = t.CreatedAt
        });

        return Ok(taskDtos);
    }

    [HttpGet("garbage")]
    public async Task<ActionResult<IEnumerable<GarbageTaskDto>>> GetGarbageTasks()
    {
        var userId = User.GetUserId();
        var userRole = User.GetUserRole();

        var query = _context.GarbageTasks
            .Include(t => t.Building)
            .Include(t => t.AssignedUser)
            .AsQueryable();

        // Students can only see tasks assigned to them
        if (userRole == "Student")
        {
            query = query.Where(t => t.AssignedUserId == userId);
        }

        var tasks = await query
            .OrderBy(t => t.DueDate)
            .ToListAsync();

        var taskDtos = tasks.Select(t => new GarbageTaskDto
        {
            Id = t.Id,
            Description = t.Description,
            Location = t.Location,
            Status = t.Status,
            BuildingId = t.BuildingId,
            BuildingName = t.Building?.Name,
            AssignedUserId = t.AssignedUserId,
            AssignedUsername = t.AssignedUser?.Username,
            DueDate = t.DueDate,
            CompletedAt = t.CompletedAt,
            CreatedAt = t.CreatedAt
        });

        return Ok(taskDtos);
    }

    [HttpPost("cleaning")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<CleaningTaskDto>> CreateCleaningTask(CreateTaskRequest request)
    {
        var building = await _context.Buildings.FindAsync(request.BuildingId);
        if (building == null)
        {
            return BadRequest(new { message = "Building not found" });
        }

        if (request.AssignedUserId.HasValue)
        {
            var user = await _context.Users.FindAsync(request.AssignedUserId.Value);
            if (user == null)
            {
                return BadRequest(new { message = "Assigned user not found" });
            }
        }

        var task = new CleaningTask
        {
            Description = request.Description,
            SharedSpace = request.LocationOrSpace,
            Status = "Pending",
            BuildingId = request.BuildingId,
            AssignedUserId = request.AssignedUserId,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.CleaningTasks.Add(task);
        await _context.SaveChangesAsync();

        // Load navigation properties
        await _context.Entry(task).Reference(t => t.Building).LoadAsync();
        await _context.Entry(task).Reference(t => t.AssignedUser).LoadAsync();

        var taskDto = new CleaningTaskDto
        {
            Id = task.Id,
            Description = task.Description,
            SharedSpace = task.SharedSpace,
            Status = task.Status,
            BuildingId = task.BuildingId,
            BuildingName = task.Building?.Name,
            AssignedUserId = task.AssignedUserId,
            AssignedUsername = task.AssignedUser?.Username,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt
        };

        return CreatedAtAction(nameof(GetCleaningTasks), taskDto);
    }

    [HttpPost("garbage")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<GarbageTaskDto>> CreateGarbageTask(CreateTaskRequest request)
    {
        var building = await _context.Buildings.FindAsync(request.BuildingId);
        if (building == null)
        {
            return BadRequest(new { message = "Building not found" });
        }

        if (request.AssignedUserId.HasValue)
        {
            var user = await _context.Users.FindAsync(request.AssignedUserId.Value);
            if (user == null)
            {
                return BadRequest(new { message = "Assigned user not found" });
            }
        }

        var task = new GarbageTask
        {
            Description = request.Description,
            Location = request.LocationOrSpace,
            Status = "Pending",
            BuildingId = request.BuildingId,
            AssignedUserId = request.AssignedUserId,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.GarbageTasks.Add(task);
        await _context.SaveChangesAsync();

        // Load navigation properties
        await _context.Entry(task).Reference(t => t.Building).LoadAsync();
        await _context.Entry(task).Reference(t => t.AssignedUser).LoadAsync();

        var taskDto = new GarbageTaskDto
        {
            Id = task.Id,
            Description = task.Description,
            Location = task.Location,
            Status = task.Status,
            BuildingId = task.BuildingId,
            BuildingName = task.Building?.Name,
            AssignedUserId = task.AssignedUserId,
            AssignedUsername = task.AssignedUser?.Username,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt
        };

        return CreatedAtAction(nameof(GetGarbageTasks), taskDto);
    }

    [HttpPost("cleaning/{id}/complete")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult> CompleteCleaningTask(int id)
    {
        var userId = User.GetUserId();
        var task = await _context.CleaningTasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.AssignedUserId != userId)
        {
            return Forbid();
        }

        task.Status = "Completed";
        task.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task completed successfully" });
    }

    [HttpPost("garbage/{id}/complete")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult> CompleteGarbageTask(int id)
    {
        var userId = User.GetUserId();
        var task = await _context.GarbageTasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.AssignedUserId != userId)
        {
            return Forbid();
        }

        task.Status = "Completed";
        task.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task completed successfully" });
    }

    [HttpPost("cleaning/{id}/verify")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult> VerifyCleaningTask(int id)
    {
        var task = await _context.CleaningTasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.Status != "Completed")
        {
            return BadRequest(new { message = "Task must be completed before verification" });
        }

        task.Status = "Verified";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task verified successfully" });
    }

    [HttpPost("garbage/{id}/verify")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult> VerifyGarbageTask(int id)
    {
        var task = await _context.GarbageTasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.Status != "Completed")
        {
            return BadRequest(new { message = "Task must be completed before verification" });
        }

        task.Status = "Verified";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task verified successfully" });
    }
}
