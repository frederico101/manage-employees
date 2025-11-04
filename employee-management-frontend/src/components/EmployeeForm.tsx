import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { apiService } from '../services/api';
import { CreateEmployeeRequest, UpdateEmployeeRequest, Employee, PhoneType, EmployeeRole } from '../types';
import { useAuthStore } from '../store/authStore';
import './EmployeeForm.css';

export const EmployeeForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();
  const { canCreateRole } = useAuthStore();

  const [formData, setFormData] = useState<CreateEmployeeRequest | UpdateEmployeeRequest>({
    firstName: '',
    lastName: '',
    email: '',
    docNumber: '',
    phones: [{ number: '', type: PhoneType.Mobile }],
    managerId: undefined,
    role: EmployeeRole.Employee,
    password: '',
    dateOfBirth: '',
  } as CreateEmployeeRequest);

  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isEdit) {
      loadEmployee();
    }
    loadEmployees();
  }, [id]);

  const loadEmployee = async () => {
    try {
      const employee = await apiService.getEmployee(parseInt(id!));
      setFormData({
        firstName: employee.firstName,
        lastName: employee.lastName,
        email: employee.email,
        phones: employee.phones,
        managerId: employee.managerId,
        dateOfBirth: employee.dateOfBirth.split('T')[0],
      } as UpdateEmployeeRequest);
    } catch (err: any) {
      setError(err.response?.data?.error?.message || 'Failed to load employee');
    }
  };

  const loadEmployees = async () => {
    try {
      const data = await apiService.getEmployees({ pageSize: 1000 });
      setEmployees(data);
    } catch (err) {
      console.error('Failed to load employees for manager selection');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (isEdit) {
        await apiService.updateEmployee(parseInt(id!), formData as UpdateEmployeeRequest);
      } else {
        await apiService.createEmployee(formData as CreateEmployeeRequest);
      }
      navigate('/employees');
    } catch (err: any) {
      setError(err.response?.data?.error?.message || `Failed to ${isEdit ? 'update' : 'create'} employee`);
    } finally {
      setLoading(false);
    }
  };

  const handlePhoneChange = (index: number, field: 'number' | 'type', value: string | PhoneType) => {
    const newPhones = [...formData.phones];
    newPhones[index] = { ...newPhones[index], [field]: value };
    setFormData({ ...formData, phones: newPhones });
  };

  const addPhone = () => {
    setFormData({
      ...formData,
      phones: [...formData.phones, { number: '', type: PhoneType.Mobile }],
    });
  };

  const removePhone = (index: number) => {
    if (formData.phones.length > 1) {
      const newPhones = formData.phones.filter((_, i) => i !== index);
      setFormData({ ...formData, phones: newPhones });
    }
  };

  return (
    <div className="employee-form-container">
      <h1>{isEdit ? 'Edit Employee' : 'Create Employee'}</h1>
      {error && <div className="error-message">{error}</div>}
      <form onSubmit={handleSubmit} className="employee-form">
        <div className="form-row">
          <div className="form-group">
            <label>First Name *</label>
            <input
              type="text"
              value={formData.firstName}
              onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
              required
              disabled={loading}
            />
          </div>
          <div className="form-group">
            <label>Last Name *</label>
            <input
              type="text"
              value={formData.lastName}
              onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
              required
              disabled={loading}
            />
          </div>
        </div>

        <div className="form-group">
          <label>Email *</label>
          <input
            type="email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
            required
            disabled={loading || isEdit}
          />
        </div>

        {!isEdit && (
          <div className="form-group">
            <label>Document Number *</label>
            <input
              type="text"
              value={(formData as CreateEmployeeRequest).docNumber}
              onChange={(e) => setFormData({ ...formData, docNumber: e.target.value } as CreateEmployeeRequest)}
              required
              disabled={loading}
            />
          </div>
        )}

        <div className="form-group">
          <label>Date of Birth *</label>
          <input
            type="date"
            value={formData.dateOfBirth}
            onChange={(e) => setFormData({ ...formData, dateOfBirth: e.target.value })}
            required
            max={new Date(new Date().setFullYear(new Date().getFullYear() - 18)).toISOString().split('T')[0]}
            disabled={loading}
          />
        </div>

        <div className="form-group">
          <label>Phones *</label>
          {formData.phones.map((phone, index) => (
            <div key={index} className="phone-input-group">
              <input
                type="tel"
                placeholder="Phone number"
                value={phone.number}
                onChange={(e) => handlePhoneChange(index, 'number', e.target.value)}
                required
                disabled={loading}
              />
              <select
                value={phone.type}
                onChange={(e) => handlePhoneChange(index, 'type', parseInt(e.target.value))}
                disabled={loading}
              >
                <option value={PhoneType.Mobile}>Mobile</option>
                <option value={PhoneType.Home}>Home</option>
                <option value={PhoneType.Work}>Work</option>
                <option value={PhoneType.Other}>Other</option>
              </select>
              {formData.phones.length > 1 && (
                <button type="button" onClick={() => removePhone(index)} disabled={loading}>
                  Remove
                </button>
              )}
            </div>
          ))}
          <button type="button" onClick={addPhone} disabled={loading} className="add-phone-button">
            Add Phone
          </button>
        </div>

        <div className="form-group">
          <label>Manager</label>
          <select
            value={formData.managerId ?? ''}
            onChange={(e) => setFormData({ ...formData, managerId: e.target.value ? parseInt(e.target.value) : undefined })}
            disabled={loading}
          >
            <option value="">No Manager</option>
            {employees
              .filter((emp) => !id || emp.id !== parseInt(id))
              .map((emp) => (
                <option key={emp.id} value={emp.id}>
                  {emp.firstName} {emp.lastName} ({emp.email})
                </option>
              ))}
          </select>
        </div>

        {!isEdit && (
          <div className="form-group">
            <label>Role *</label>
            <select
              value={(formData as CreateEmployeeRequest).role}
              onChange={(e) => setFormData({ ...formData, role: parseInt(e.target.value) as EmployeeRole } as CreateEmployeeRequest)}
              required
              disabled={loading}
            >
              <option value={EmployeeRole.Employee}>Employee</option>
              {canCreateRole(EmployeeRole.Leader) && <option value={EmployeeRole.Leader}>Leader</option>}
              {canCreateRole(EmployeeRole.Director) && <option value={EmployeeRole.Director}>Director</option>}
            </select>
          </div>
        )}

        {!isEdit && (
          <div className="form-group">
            <label>Password *</label>
            <input
              type="password"
              value={(formData as CreateEmployeeRequest).password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value } as CreateEmployeeRequest)}
              required
              minLength={8}
              disabled={loading}
            />
            <small>Must be at least 8 characters with uppercase, lowercase, digit, and special character</small>
          </div>
        )}

        <div className="form-actions">
          <button type="submit" disabled={loading} className="submit-button">
            {loading ? (isEdit ? 'Updating...' : 'Creating...') : (isEdit ? 'Update' : 'Create')}
          </button>
          <button type="button" onClick={() => navigate('/employees')} className="cancel-button">
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

