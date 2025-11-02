using AutoMapper;
using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Employee mappings
        CreateMap<Employee, EmployeeResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? $"{src.Manager.FirstName} {src.Manager.LastName}" : null))
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones.Select(p => new PhoneResponse
            {
                Number = p.Number,
                Type = p.Type
            })));

        // Request to Entity (for create/update)
        CreateMap<CreateEmployeeRequest, Employee>()
            .ConstructUsing(src => new Employee(
                src.FirstName,
                src.LastName,
                Email.Create(src.Email),
                src.DocNumber,
                src.DateOfBirth,
                src.Role,
                string.Empty, // Password hash will be set separately
                src.ManagerId));

        CreateMap<PhoneRequest, Phone>()
            .ConstructUsing(src => Phone.Create(src.Number, src.Type));
    }
}

