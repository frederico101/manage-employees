using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.UseCases.Employees.Commands;
using EmployeeManagement.Application.UseCases.Employees.Queries;
using EmployeeManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployees(
        [FromQuery] int? managerId,
        [FromQuery] int? role,
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting employees list with filters: ManagerId={ManagerId}, Role={Role}, SearchTerm={SearchTerm}, Page={PageNumber}, Size={PageSize}",
            managerId, role, searchTerm, pageNumber, pageSize);

        var query = new ListEmployeesQuery
        {
            ManagerId = managerId,
            Role = role,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var employees = await _mediator.Send(query);
        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeResponse>> GetEmployee(int id)
    {
        _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);

        var query = new GetEmployeeQuery { EmployeeId = id };
        var employee = await _mediator.Send(query);
        return Ok(employee);
    }

    /// <summary>
    /// Create a new employee (requires appropriate role permissions)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        _logger.LogInformation("Creating new employee with email: {Email}", request.Email);

        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        var command = new CreateEmployeeCommand
        {
            Request = request,
            CurrentUserId = currentUserId,
            CurrentUserRole = currentUserRole
        };

        var employee = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    /// <summary>
    /// Update an existing employee (requires appropriate role permissions)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        var command = new UpdateEmployeeCommand
        {
            EmployeeId = id,
            Request = request,
            CurrentUserId = currentUserId,
            CurrentUserRole = currentUserRole
        };

        var employee = await _mediator.Send(command);
        return Ok(employee);
    }

    /// <summary>
    /// Delete an employee (requires appropriate role permissions)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        var command = new DeleteEmployeeCommand
        {
            EmployeeId = id,
            CurrentUserId = currentUserId,
            CurrentUserRole = currentUserRole
        };

        await _mediator.Send(command);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("EmployeeId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private EmployeeRole GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst("Role")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return Enum.TryParse<EmployeeRole>(roleClaim, out var role) ? role : EmployeeRole.Employee;
    }
}

