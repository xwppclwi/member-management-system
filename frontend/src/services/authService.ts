import { apiClient } from './apiClient';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  data: {
    token: string;
    user: {
      id: number;
      username: string;
      email: string;
      role: string;
    };
  };
}

export interface UserInfo {
  id: number;
  username: string;
  email: string;
  role: string;
  avatar?: string;
}

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/api/auth/login', credentials);

    if (response.success && response.data.token) {
      localStorage.setItem('access_token', response.data.token);
      localStorage.setItem('user_info', JSON.stringify(response.data.user));
    }

    return response;
  },

  async logout(): Promise<void> {
    try {
      await apiClient.post('/api/auth/logout', {});
    } finally {
      localStorage.removeItem('access_token');
      localStorage.removeItem('user_info');
      localStorage.removeItem('remember_me');
    }
  },

  async getCurrentUser(): Promise<any> {
    return apiClient.get('/api/auth/me');
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('access_token');
  },

  getToken(): string | null {
    return localStorage.getItem('access_token');
  },

  getUserInfo(): UserInfo | null {
    const userInfo = localStorage.getItem('user_info');
    return userInfo ? JSON.parse(userInfo) : null;
  },
};

export default authService;
