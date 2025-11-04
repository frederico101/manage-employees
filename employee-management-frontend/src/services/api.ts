import axios, { AxiosInstance, AxiosError } from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8080/api';

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor to add token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor to handle errors
    this.api.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        if (error.response?.status === 401) {
          // Unauthorized - clear token and redirect to login
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async login(email: string, password: string) {
    const response = await this.api.post('/auth/login', { email, password });
    return response.data;
  }

  async register(data: any) {
    const response = await this.api.post('/auth/register', data);
    return response.data;
  }

  // Employee endpoints
  async getEmployees(params?: {
    managerId?: number;
    role?: number;
    searchTerm?: string;
    pageNumber?: number;
    pageSize?: number;
  }) {
    const response = await this.api.get('/employees', { params });
    return response.data;
  }

  async getEmployee(id: number) {
    const response = await this.api.get(`/employees/${id}`);
    return response.data;
  }

  async createEmployee(data: any) {
    const response = await this.api.post('/employees', data);
    return response.data;
  }

  async updateEmployee(id: number, data: any) {
    const response = await this.api.put(`/employees/${id}`, data);
    return response.data;
  }

  async deleteEmployee(id: number) {
    await this.api.delete(`/employees/${id}`);
  }
}

export const apiService = new ApiService();

