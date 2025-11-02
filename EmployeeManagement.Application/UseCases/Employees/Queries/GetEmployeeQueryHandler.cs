using AutoMapper;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Domain.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Queries;

public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, EmployeeResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeeQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<EmployeeResponse> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found");
        }

        return _mapper.Map<EmployeeResponse>(employee);
    }
}

