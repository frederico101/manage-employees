import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { apiService } from '../services/api';
import { Employee, EmployeeRole, PhoneType } from '../types';
import { useAuthStore } from '../store/authStore';
import './EmployeeDetails.css';

export const EmployeeDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { hasRole } = useAuthStore();
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadEmployee();
  }, [id]);

  const loadEmployee = async () => {
    try {
      const data = await apiService.getEmployee(parseInt(id!));
      setEmployee(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.error?.message || 'Failed to load employee');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this employee?')) {
      return;
    }

    try {
      await apiService.deleteEmployee(parseInt(id!));
      navigate('/employees');
    } catch (err: any) {
      alert(err.response?.data?.error?.message || 'Failed to delete employee');
    }
  };

  const getRoleName = (role: EmployeeRole) => {
    return EmployeeRole[role];
  };

  const getPhoneTypeName = (type: PhoneType) => {
    return PhoneType[type];
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error || !employee) {
    return <div className="error-message">{error || 'Employee not found'}</div>;
  }

  return (
    <div className="employee-details-container">
      <div className="details-header">
        <h1>Employee Details</h1>
        <div className="header-actions">
          {hasRole(EmployeeRole.Leader) && (
            <>
              <Link to={`/employees/${employee.id}/edit`} className="action-button edit">
                Edit
              </Link>
              <button onClick={handleDelete} className="action-button delete">
                Delete
              </button>
            </>
          )}
          <Link to="/employees" className="action-button back">
            Back to List
          </Link>
        </div>
      </div>

      <div className="details-card">
        <div className="details-section">
          <h2>Personal Information</h2>
          <div className="details-grid">
            <div className="detail-item">
              <label>First Name</label>
              <div>{employee.firstName}</div>
            </div>
            <div className="detail-item">
              <label>Last Name</label>
              <div>{employee.lastName}</div>
            </div>
            <div className="detail-item">
              <label>Email</label>
              <div>{employee.email}</div>
            </div>
            <div className="detail-item">
              <label>Document Number</label>
              <div>{employee.docNumber}</div>
            </div>
            <div className="detail-item">
              <label>Date of Birth</label>
              <div>{new Date(employee.dateOfBirth).toLocaleDateString()}</div>
            </div>
            <div className="detail-item">
              <label>Age</label>
              <div>{employee.age} years</div>
            </div>
          </div>
        </div>

        <div className="details-section">
          <h2>Employment Information</h2>
          <div className="details-grid">
            <div className="detail-item">
              <label>Role</label>
              <div>
                <span className={`role-badge role-${getRoleName(employee.role).toLowerCase()}`}>
                  {getRoleName(employee.role)}
                </span>
              </div>
            </div>
            <div className="detail-item">
              <label>Manager</label>
              <div>{employee.managerName || 'No Manager'}</div>
            </div>
            <div className="detail-item">
              <label>Created At</label>
              <div>{new Date(employee.createdAt).toLocaleString()}</div>
            </div>
            {employee.updatedAt && (
              <div className="detail-item">
                <label>Last Updated</label>
                <div>{new Date(employee.updatedAt).toLocaleString()}</div>
              </div>
            )}
          </div>
        </div>

        <div className="details-section">
          <h2>Contact Information</h2>
          <div className="phones-list">
            {employee.phones.map((phone, index) => (
              <div key={index} className="phone-item">
                <span className="phone-number">{phone.number}</span>
                <span className="phone-type">{getPhoneTypeName(phone.type)}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

