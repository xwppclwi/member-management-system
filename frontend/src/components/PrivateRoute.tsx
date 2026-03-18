import React, { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuthStore } from '../store/authStore';

interface PrivateRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({
  children,
  requiredRoles,
}) => {
  const location = useLocation();
  const { isAuthenticated, user, checkAuth } = useAuthStore();
  const [isChecking, setIsChecking] = useState(true);
  const [hasAuth, setHasAuth] = useState(false);

  useEffect(() => {
    const authenticated = checkAuth();
    setHasAuth(authenticated);
    setIsChecking(false);
  }, [checkAuth]);

  if (isChecking) {
    return (
      <div style={{ height: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <Spin size="large" tip="加载中..." />
      </div>
    );
  }

  if (!hasAuth && !isAuthenticated) {
    return <Navigate to="/login" state={{ from: location.pathname }} replace />;
  }

  if (requiredRoles && user && !requiredRoles.includes(user.role)) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

export default PrivateRoute;
