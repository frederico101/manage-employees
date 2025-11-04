import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from './store/authStore';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Layout } from './components/Layout';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { EmployeeList } from './pages/EmployeeList';
import { EmployeeDetails } from './pages/EmployeeDetails';
import { EmployeeForm } from './components/EmployeeForm';
import './App.css';

function App() {
  const { isAuthenticated, user } = useAuthStore();

  useEffect(() => {
    // Initialize auth state from localStorage
    const token = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');
    if (token && storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser);
        useAuthStore.getState().login(token, parsedUser);
      } catch (error) {
        console.error('Failed to parse stored user data');
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
  }, []);

  return (
    <Router>
      <Routes>
        <Route path="/login" element={!isAuthenticated ? <Login /> : <Navigate to="/employees" replace />} />
        <Route path="/register" element={!isAuthenticated ? <Register /> : <Navigate to="/employees" replace />} />
        <Route
          path="/employees"
          element={
            <ProtectedRoute>
              <Layout>
                <EmployeeList />
              </Layout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/employees/new"
          element={
            <ProtectedRoute>
              <Layout>
                <EmployeeForm />
              </Layout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/employees/:id"
          element={
            <ProtectedRoute>
              <Layout>
                <EmployeeDetails />
              </Layout>
            </ProtectedRoute>
          }
        />
        <Route
          path="/employees/:id/edit"
          element={
            <ProtectedRoute>
              <Layout>
                <EmployeeForm />
              </Layout>
            </ProtectedRoute>
          }
        />
        <Route path="/" element={<Navigate to={isAuthenticated ? '/employees' : '/login'} replace />} />
      </Routes>
    </Router>
  );
}

export default App;
