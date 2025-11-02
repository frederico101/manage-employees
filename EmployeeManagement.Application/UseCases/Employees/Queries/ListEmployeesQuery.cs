using EmployeeManagement.Application.DTOs.Responses;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Queries;

public class ListEmployeesQuery : IRequest<List<EmployeeResponse>>
{
    public int? ManagerId { get; set; }
    public int? Role { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

