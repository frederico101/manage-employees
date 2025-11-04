import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiService } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { EmployeeRole, PhoneType, RegisterRequest } from '../types';
import './Register.css';

export const Register: React.FC = () => {
  const [formData, setFormData] = useState<RegisterRequest>({
    firstName: '',
    lastName: '',
    email: '',
    docNumber: '',
    phones: [{ number: '', type: PhoneType.Mobile }],
    role: EmployeeRole.Employee,
    password: '',
    dateOfBirth: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login, canCreateRole } = useAuthStore();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await apiService.register(formData);
      login(response.token, response.user);
      navigate('/employees');
    } catch (err: any) {
      setError(err.response?.data?.error?.message || 'Registration failed');
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
    <div className="register-container">
      <div className="register-card">
        <h2>Register</h2>
        {error && <div className="error-message">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="firstName">First Name *</label>
              <input
                type="text"
                id="firstName"
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                required
                disabled={loading}
              />
            </div>
            <div className="form-group">
              <label htmlFor="lastName">Last Name *</label>
              <input
                type="text"
                id="lastName"
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                required
                disabled={loading}
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="email">Email *</label>
            <input
              type="email"
              id="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="docNumber">Document Number *</label>
            <input
              type="text"
              id="docNumber"
              value={formData.docNumber}
              onChange={(e) => setFormData({ ...formData, docNumber: e.target.value })}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="dateOfBirth">Date of Birth *</label>
            <input
              type="date"
              id="dateOfBirth"
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
            <label htmlFor="role">Role *</label>
            <select
              id="role"
              value={formData.role}
              onChange={(e) => setFormData({ ...formData, role: parseInt(e.target.value) as EmployeeRole })}
              required
              disabled={loading}
            >
              <option value={EmployeeRole.Employee}>Employee</option>
              {canCreateRole(EmployeeRole.Leader) && <option value={EmployeeRole.Leader}>Leader</option>}
              {canCreateRole(EmployeeRole.Director) && <option value={EmployeeRole.Director}>Director</option>}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="password">Password *</label>
            <input
              type="password"
              id="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              required
              minLength={8}
              disabled={loading}
            />
            <small>Must be at least 8 characters with uppercase, lowercase, digit, and special character</small>
          </div>

          <button type="submit" disabled={loading} className="submit-button">
            {loading ? 'Registering...' : 'Register'}
          </button>
        </form>
        <p className="login-link">
          Already have an account? <a href="/login">Login here</a>
        </p>
      </div>
    </div>
  );
};

