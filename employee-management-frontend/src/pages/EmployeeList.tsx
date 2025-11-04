import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { apiService } from '../services/api';
import { Employee, EmployeeRole } from '../types';
import { useAuthStore } from '../store/authStore';
import './EmployeeList.css';

export const EmployeeList: React.FC = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [filterRole, setFilterRole] = useState<number | undefined>();
  const { hasRole } = useAuthStore();

  useEffect(() => {
    loadEmployees();
  }, [searchTerm, filterRole]);

  const loadEmployees = async () => {
    try {
      setLoading(true);
      const data = await apiService.getEmployees({
        searchTerm: searchTerm || undefined,
        role: filterRole,
        pageNumber: 1,
        pageSize: 100,
      });
      setEmployees(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.error?.message || 'Failed to load employees');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this employee?')) {
      return;
    }

    try {
      await apiService.deleteEmployee(id);
      loadEmployees();
    } catch (err: any) {
      alert(err.response?.data?.error?.message || 'Failed to delete employee');
    }
  };

  const getRoleName = (role: EmployeeRole) => {
    return EmployeeRole[role];
  };

  return (
    <div className="employee-list-container">
      <div className="page-header">
        <h1>Employees</h1>
        {hasRole(EmployeeRole.Leader) && (
          <Link to="/employees/new" className="add-button">
            + Add Employee
          </Link>
        )}
      </div>

      <div className="filters">
        <input
          type="text"
          placeholder="Search by name, email, or document number..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        <select
          value={filterRole ?? ''}
          onChange={(e) => setFilterRole(e.target.value ? parseInt(e.target.value) : undefined)}
          className="filter-select"
        >
          <option value="">All Roles</option>
          <option value={EmployeeRole.Employee}>Employee</option>
          <option value={EmployeeRole.Leader}>Leader</option>
          <option value={EmployeeRole.Director}>Director</option>
        </select>
      </div>

      {error && <div className="error-message">{error}</div>}

      {loading ? (
        <div className="loading">Loading...</div>
      ) : (
        <div className="employee-table-container">
          <table className="employee-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Document</th>
                <th>Role</th>
                <th>Manager</th>
                <th>Age</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {employees.length === 0 ? (
                <tr>
                  <td colSpan={7} className="no-data">No employees found</td>
                </tr>
              ) : (
                employees.map((employee) => (
                  <tr key={employee.id}>
                    <td>{employee.firstName} {employee.lastName}</td>
                    <td>{employee.email}</td>
                    <td>{employee.docNumber}</td>
                    <td>
                      <span className={`role-badge role-${getRoleName(employee.role).toLowerCase()}`}>
                        {getRoleName(employee.role)}
                      </span>
                    </td>
                    <td>{employee.managerName || '-'}</td>
                    <td>{employee.age}</td>
                    <td>
                      <div className="action-buttons">
                        <Link to={`/employees/${employee.id}`} className="action-button view">
                          View
                        </Link>
                        {hasRole(EmployeeRole.Leader) && (
                          <>
                            <Link to={`/employees/${employee.id}/edit`} className="action-button edit">
                              Edit
                            </Link>
                            <button
                              onClick={() => handleDelete(employee.id)}
                              className="action-button delete"
                            >
                              Delete
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

