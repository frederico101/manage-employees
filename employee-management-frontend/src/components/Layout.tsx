import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import './Layout.css';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="nav-brand">
          <Link to="/employees">Employee Management</Link>
        </div>
        <div className="nav-links">
          <Link to="/employees">Employees</Link>
          {user && (
            <div className="user-info">
              <span>{user.firstName} {user.lastName}</span>
              <span className="role-badge">{user.role}</span>
              <button onClick={handleLogout} className="logout-button">Logout</button>
            </div>
          )}
        </div>
      </nav>
      <main className="main-content">
        {children}
      </main>
    </div>
  );
};

