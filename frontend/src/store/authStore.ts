import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { UserInfo, authService } from '../services/authService';

interface AuthState {
  token: string | null;
  user: UserInfo | null;
  isAuthenticated: boolean;

  setAuth: (token: string, user: UserInfo) => void;
  clearAuth: () => void;
  logout: () => Promise<void>;
  checkAuth: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      user: null,
      isAuthenticated: false,

      setAuth: (token: string, user: UserInfo) => {
        set({ token, user, isAuthenticated: true });
        localStorage.setItem('access_token', token);
        localStorage.setItem('user_info', JSON.stringify(user));
      },

      clearAuth: () => {
        set({ token: null, user: null, isAuthenticated: false });
        localStorage.removeItem('access_token');
        localStorage.removeItem('user_info');
      },

      logout: async () => {
        try {
          await authService.logout();
        } finally {
          get().clearAuth();
        }
      },

      checkAuth: () => {
        const token = localStorage.getItem('access_token');
        const userInfo = localStorage.getItem('user_info');

        if (token && userInfo && !get().isAuthenticated) {
          set({
            token,
            user: JSON.parse(userInfo),
            isAuthenticated: true,
          });
        }

        return !!token;
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);

export default useAuthStore;
