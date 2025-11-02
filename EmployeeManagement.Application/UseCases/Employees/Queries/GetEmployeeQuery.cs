using EmployeeManagement.Application.DTOs.Responses;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Queries;

public class GetEmployeeQuery : IRequest<EmployeeResponse>
{
    public int EmployeeId { get; set; }
}

