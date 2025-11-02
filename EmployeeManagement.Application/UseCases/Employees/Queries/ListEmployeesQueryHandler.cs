using AutoMapper;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Queries;

public class ListEmployeesQueryHandler : IRequestHandler<ListEmployeesQuery, List<EmployeeResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public ListEmployeesQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<List<EmployeeResponse>> Handle(ListEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);

        // Filter by manager if specified
        if (request.ManagerId.HasValue)
        {
            employees = employees.Where(e => e.ManagerId == request.ManagerId.Value);
        }

        // Filter by role if specified
        if (request.Role.HasValue)
        {
            var role = (Domain.Enums.EmployeeRole)request.Role.Value;
            employees = employees.Where(e => e.Role == role);
        }

        // Search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            employees = employees.Where(e =>
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm) ||
                e.EmailAddress.ToLower().Contains(searchTerm) ||
                e.DocNumber.Contains(searchTerm));
        }

        // Pagination
        var paginatedEmployees = employees
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<EmployeeResponse>>(paginatedEmployees);
    }
}

