export enum EmployeeRole {
  Employee = 0,
  Leader = 1,
  Director = 2,
}

export enum PhoneType {
  Mobile = 0,
  Home = 1,
  Work = 2,
  Other = 3,
}

export interface Phone {
  number: string;
  type: PhoneType;
}

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  docNumber: string;
  phones: Phone[];
  managerId?: number;
  managerName?: string;
  role: EmployeeRole;
  dateOfBirth: string;
  age: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  docNumber: string;
  phones: Phone[];
  managerId?: number;
  role: EmployeeRole;
  password: string;
  dateOfBirth: string;
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phones: Phone[];
  managerId?: number;
  dateOfBirth: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  docNumber: string;
  phones: Phone[];
  role: EmployeeRole;
  password: string;
  dateOfBirth: string;
}

export interface AuthResponse {
  token: string;
  user: Employee;
  expiresAt: string;
}

export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: EmployeeRole;
}

