using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .FirstOrDefaultAsync(e => e.EmailAddress.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<Employee?> GetByDocNumberAsync(string docNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .FirstOrDefaultAsync(e => e.DocNumber == docNumber, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByManagerIdAsync(int managerId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .Where(e => e.ManagerId == managerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.AnyAsync(e => e.EmailAddress.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<bool> DocNumberExistsAsync(string docNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.AnyAsync(e => e.DocNumber == docNumber, cancellationToken);
    }
}

