import axios from 'axios';
import { message } from 'antd';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
});

// 请求拦截器
axiosInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// 响应拦截器
axiosInstance.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (error.response?.status === 401) {
      message.error('登录已过期，请重新登录');
      localStorage.removeItem('access_token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const apiClient = {
  get: <T>(url: string, config?: any) => axiosInstance.get(url, config) as Promise<T>,
  post: <T>(url: string, data?: any, config?: any) => axiosInstance.post(url, data, config) as Promise<T>,
  put: <T>(url: string, data?: any, config?: any) => axiosInstance.put(url, data, config) as Promise<T>,
  delete: <T>(url: string, config?: any) => axiosInstance.delete(url, config) as Promise<T>,
};

export default apiClient;
