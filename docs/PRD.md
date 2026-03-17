# 会员管理系统 - 产品需求文档 (PRD)

## 1. 项目概述

### 1.1 项目背景
会员管理系统是一套用于企业管理会员信息、会员卡、积分系统的完整解决方案。系统支持会员的注册、信息维护、等级管理、积分累计与兑换等功能。

### 1.2 目标用户
- **系统管理员**：负责系统配置、用户权限管理
- **运营人员**：负责会员管理、活动策划
- **客服人员**：负责会员服务、问题处理
- **会员用户**：使用会员服务、查看积分/卡片

### 1.3 技术栈
- **后端**：.NET Core 8 + Entity Framework Core + SQL Server/SQLite
- **前端**：React 18 + TypeScript + Ant Design 5
- **认证**：JWT Token
- **API风格**：RESTful API

---

## 2. 功能需求

### 2.1 用户认证模块 (mod-1)
| 功能 | 描述 | 优先级 |
|------|------|--------|
| 用户注册 | 管理员创建账号，分配角色 | P0 |
| 用户登录 | 用户名/密码登录，返回JWT | P0 |
| 角色管理 | 管理员、运营、客服三种角色 | P0 |
| 权限控制 | 基于角色的API访问控制 | P0 |
| 修改密码 | 用户可修改自己的密码 | P1 |

### 2.2 会员管理模块 (mod-2)
| 功能 | 描述 | 优先级 |
|------|------|--------|
| 会员注册 | 录入会员基本信息 | P0 |
| 会员列表 | 分页查询、搜索、筛选 | P0 |
| 会员详情 | 查看完整会员信息 | P0 |
| 会员编辑 | 修改会员信息 | P0 |
| 会员删除 | 软删除会员 | P0 |
| 等级管理 | 普通/银卡/金卡/钻石等级 | P1 |
| 积分查看 | 显示当前积分余额 | P0 |

### 2.3 会员卡管理模块 (mod-3)
| 功能 | 描述 | 优先级 |
|------|------|--------|
| 开卡 | 为会员开通会员卡 | P1 |
| 卡列表 | 查询所有会员卡 | P1 |
| 挂失/解挂 | 会员卡状态管理 | P2 |
| 补卡 | 补办新卡 | P2 |
| 注销 | 注销会员卡 | P2 |

### 2.4 积分系统模块 (mod-4)
| 功能 | 描述 | 优先级 |
|------|------|--------|
| 积分增加 | 消费返积分、活动赠送 | P1 |
| 积分扣减 | 积分兑换礼品/优惠券 | P1 |
| 积分记录 | 查询积分变动历史 | P1 |
| 积分规则 | 配置积分获取规则 | P2 |

---

## 3. 数据模型设计

### 3.1 实体关系图
```
User (1) ────────< (N) Member
                    │
                    ├─< (N) MemberCard
                    │
                    └─< (N) PointsRecord
```

### 3.2 实体定义

#### User (系统用户)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| Username | string(50) | 用户名 |
| PasswordHash | string(256) | 密码哈希 |
| Email | string(100) | 邮箱 |
| Role | string(20) | 角色: Admin/Operator/Service |
| IsActive | bool | 是否启用 |
| CreatedAt | datetime | 创建时间 |

#### Member (会员)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| MemberNo | string(20) | 会员卡号 |
| Name | string(50) | 姓名 |
| Phone | string(20) | 手机号 |
| Email | string(100) | 邮箱 |
| Gender | int | 性别: 0-未知, 1-男, 2-女 |
| Birthday | date | 生日 |
| Level | int | 等级: 0-普通, 1-银卡, 2-金卡, 3-钻石 |
| Points | int | 当前积分 |
| Status | int | 状态: 0-正常, 1-冻结, 2-注销 |
| Address | string(200) | 地址 |
| Remark | string(500) | 备注 |
| CreatedAt | datetime | 创建时间 |
| UpdatedAt | datetime | 更新时间 |

#### MemberCard (会员卡)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| CardNo | string(20) | 卡号 |
| MemberId | int | 会员ID |
| Type | int | 卡类型: 0-实体卡, 1-电子卡 |
| Status | int | 状态: 0-正常, 1-挂失, 2-注销 |
| IssueDate | date | 发卡日期 |
| ExpiryDate | date | 有效期 |
| CreatedAt | datetime | 创建时间 |

#### PointsRecord (积分记录)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| MemberId | int | 会员ID |
| Type | int | 类型: 1-增加, 2-扣减 |
| Points | int | 积分数量 |
| Balance | int | 变动后余额 |
| Source | string(50) | 来源/用途 |
| OrderNo | string(50) | 关联订单号 |
| Remark | string(200) | 备注 |
| CreatedAt | datetime | 创建时间 |
| CreatedBy | int | 操作人ID |

---

## 4. API接口设计

### 4.1 用户认证 (mod-1)
| 方法 | 路径 | 描述 |
|------|------|------|
| POST | /api/auth/login | 用户登录 |
| POST | /api/auth/register | 用户注册(Admin) |
| POST | /api/auth/logout | 用户登出 |
| GET | /api/auth/me | 获取当前用户 |
| PUT | /api/auth/password | 修改密码 |

### 4.2 会员管理 (mod-2)
| 方法 | 路径 | 描述 |
|------|------|------|
| GET | /api/members | 获取会员列表(分页) |
| GET | /api/members/{id} | 获取会员详情 |
| POST | /api/members | 创建会员 |
| PUT | /api/members/{id} | 更新会员 |
| DELETE | /api/members/{id} | 删除会员 |
| GET | /api/members/search | 搜索会员 |
| GET | /api/members/{id}/points | 获取会员积分 |

### 4.3 会员卡管理 (mod-3)
| 方法 | 路径 | 描述 |
|------|------|------|
| GET | /api/members/{id}/cards | 获取会员卡列表 |
| POST | /api/members/{id}/cards | 开通会员卡 |
| PUT | /api/cards/{cardId}/status | 更新卡状态 |
| POST | /api/cards/{cardId}/replace | 补办新卡 |

### 4.4 积分系统 (mod-4)
| 方法 | 路径 | 描述 |
|------|------|------|
| GET | /api/members/{id}/points-records | 获取积分记录 |
| POST | /api/members/{id}/points/add | 增加积分 |
| POST | /api/members/{id}/points/deduct | 扣减积分 |
| GET | /api/points-rules | 获取积分规则 |

---

## 5. 前端页面设计

### 5.1 页面清单

| 模块 | 页面 | 路径 | 优先级 |
|------|------|------|--------|
| mod-5 | 登录页 | /login | P0 |
| mod-6 | 会员列表 | /members | P0 |
| mod-7 | 会员详情 | /members/:id | P1 |
| mod-7 | 会员编辑 | /members/:id/edit | P1 |
| mod-8 | 仪表盘 | /dashboard | P1 |
| mod-8 | 会员卡管理 | /cards | P1 |
| mod-8 | 积分记录 | /points | P1 |

### 5.2 页面布局
- **登录页**：居中卡片式，用户名/密码输入框
- **主布局**：左侧导航 + 顶部Header + 内容区
- **会员列表**：搜索栏 + 筛选器 + 表格 + 分页
- **会员详情**：信息卡片 + 积分显示 + 操作按钮

---

## 6. 非功能需求

### 6.1 性能要求
- API响应时间 < 200ms (P95)
- 页面加载时间 < 2s
- 支持并发用户 100+

### 6.2 安全要求
- 密码使用BCrypt加密
- API使用JWT认证
- 输入参数验证
- SQL注入防护

### 6.3 兼容性
- 浏览器：Chrome 90+, Firefox 88+, Safari 14+
- 移动端：响应式设计支持

---

## 7. 开发计划

| 阶段 | 模块 | 描述 | 预计时间 |
|------|------|------|----------|
| Phase 1 | mod-1 | 用户认证后端 | 2-3天 |
| Phase 1 | mod-2 | 会员管理后端 | 3-4天 |
| Phase 1 | mod-5 | 登录页面前端 | 1-2天 |
| Phase 1 | mod-6 | 会员列表前端 | 2-3天 |
| Phase 2 | mod-3 | 会员卡后端 | 2-3天 |
| Phase 2 | mod-4 | 积分系统后端 | 2-3天 |
| Phase 2 | mod-7 | 会员详情前端 | 2-3天 |
| Phase 2 | mod-8 | 仪表盘前端 | 2-3天 |

---

*文档版本: 1.0*
*创建日期: 2026-03-17*
*作者: 军师 (AI Agent)*
