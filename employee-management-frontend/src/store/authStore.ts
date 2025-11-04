import { create } from 'zustand';
import { User, EmployeeRole } from '../types';

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (token: string, user: User) => void;
  logout: () => void;
  hasRole: (role: EmployeeRole) => boolean;
  canCreateRole: (targetRole: EmployeeRole) => boolean;
}

export const useAuthStore = create<AuthState>()((set, get) => ({
  user: null,
  token: null,
  isAuthenticated: false,

  login: (token: string, user: User) => {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    set({ user, token, isAuthenticated: true });
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    set({ user: null, token: null, isAuthenticated: false });
  },

  hasRole: (role: EmployeeRole) => {
    const currentRole = get().user?.role ?? EmployeeRole.Employee;
    return currentRole >= role;
  },

  canCreateRole: (targetRole: EmployeeRole) => {
    const currentRole = get().user?.role ?? EmployeeRole.Employee;
    return currentRole >= targetRole;
  },
}));
