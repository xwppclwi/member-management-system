import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Form, Input, Button, Checkbox, message, Typography } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useAuthStore } from '../../store/authStore';
import { authService } from '../../services/authService';
import './style.css';

const { Title, Text } = Typography;

interface LoginFormValues {
  username: string;
  password: string;
  remember: boolean;
}

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { setAuth } = useAuthStore();
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const handleLogin = async (values: LoginFormValues) => {
    setLoading(true);
    try {
      const response = await authService.login({
        username: values.username,
        password: values.password,
      });

      if (response.success) {
        setAuth(response.data.token, response.data.user);

        if (values.remember) {
          localStorage.setItem('remember_me', 'true');
        }

        message.success('登录成功！');
        navigate('/dashboard');
      } else {
        message.error(response.message || '登录失败');
      }
    } catch (error: any) {
      message.error(error.message || '登录失败，请检查用户名和密码');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-background">
        <div className="login-shape shape-1"></div>
        <div className="login-shape shape-2"></div>
        <div className="login-shape shape-3"></div>
      </div>

      <Card className="login-card" bordered={false}>
        <div className="login-header">
          <Title level={3} className="login-title">
            会员管理系统
          </Title>
          <Text type="secondary" className="login-subtitle">
            欢迎回来，请登录您的账户
          </Text>
        </div>

        <Form
          form={form}
          name="login"
          onFinish={handleLogin}
          autoComplete="off"
          size="large"
        >
          <Form.Item
            name="username"
            rules={[
              { required: true, message: '请输入用户名' },
              { min: 3, message: '用户名至少3个字符' },
            ]}
          >
            <Input
              prefix={<UserOutlined className="login-icon" />}
              placeholder="用户名"
              autoFocus
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[
              { required: true, message: '请输入密码' },
              { min: 6, message: '密码至少6个字符' },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="login-icon" />}
              placeholder="密码"
            />
          </Form.Item>

          <Form.Item>
            <div className="login-options">
              <Form.Item name="remember" valuePropName="checked" noStyle>
                <Checkbox>记住我</Checkbox>
              </Form.Item>
              <a href="#" className="forgot-password">
                忘记密码？
              </a>
            </div>
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              block
              className="login-button"
            >
              {loading ? '登录中...' : '登 录'}
            </Button>
          </Form.Item>
        </Form>

        <div className="login-footer">
          <Text type="secondary">
            © 2024 会员管理系统 · 版本 v1.0.0
          </Text>
        </div>
      </Card>
    </div>
  );
};

export default LoginPage;
