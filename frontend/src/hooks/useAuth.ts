import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { message } from 'antd';
import { useAuthStore } from '../store/authStore';
import { authService, LoginRequest } from '../services/authService';

interface UseAuthReturn {
  user: any;
  isAuthenticated: boolean;
  login: (credentials: LoginRequest) => Promise<boolean>;
  logout: () => Promise<void>;
}

export const useAuth = (): UseAuthReturn => {
  const navigate = useNavigate();
  const { user, isAuthenticated, setAuth, clearAuth } = useAuthStore();

  const login = useCallback(async (credentials: LoginRequest): Promise<boolean> => {
    try {
      const response = await authService.login(credentials);
      if (response.success) {
        setAuth(response.data.token, response.data.user);
        message.success('登录成功！');
        return true;
      }
      return false;
    } catch (error: any) {
      message.error(error.message || '登录失败');
      return false;
    }
  }, [setAuth]);

  const logout = useCallback(async (): Promise<void> => {
    await authService.logout();
    clearAuth();
    message.success('已退出');
    navigate('/login');
  }, [clearAuth, navigate]);

  return { user, isAuthenticated, login, logout };
};

export const useRequireAuth = (redirectTo: string = '/login'): boolean => {
  const navigate = useNavigate();
  const { isAuthenticated, checkAuth } = useAuthStore();
  const [isChecked, setIsChecked] = useState(false);

  useEffect(() => {
    const hasAuth = checkAuth();
    if (!hasAuth) {
      navigate(redirectTo, { replace: true });
    }
    setIsChecked(true);
  }, [checkAuth, navigate, redirectTo]);

  return isChecked && isAuthenticated;
};

export default useAuth;
