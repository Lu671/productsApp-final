# 🛒 Products App - Improved

A modern **ASP.NET Core MVC e-commerce platform** with real-time notifications, multi-language support, and PWA capabilities.

---

## 📋 Table of Contents

- [Tech Stack](#-tech-stack)
- [Project Overview](#-project-overview)
- [Project Structure](#-project-structure)
- [User Roles](#-user-roles)
- [Order Workflow](#-order-workflow)
- [Database Schema](#%EF%B8%8F-database-schema)
- [Quick Start](#-quick-start)
- [Notification System](#-notification-system)
- [Localization](#-localization)
- [Configuration](#-configuration)

---

## 📊 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core 7/8+, C# |
| **Database** | SQL Server, Entity Framework Core |
| **Frontend** | HTML, CSS, JavaScript, Bootstrap 5 |
| **Real-time** | SignalR, Web Push Notifications |
| **Auth** | ASP.NET Identity, Role-based Authorization |
| **i18n** | Multi-language (EN, AR) |

---

## 🎯 Project Overview

**Products App** is a full-featured e-commerce platform supporting:

- **👤 Customers**: Browse products, manage cart, place orders, receive notifications
- **🔧 Admins**: User management, order monitoring, audit logs

### Key Features:
✅ Real-time notifications (SignalR + Web Push)  
✅ Multi-language support (English & Arabic)  
✅ Shopping cart with session persistence  
✅ Order tracking & history  
✅ Audit logging for all user actions  
✅ Progressive Web App (PWA) support

---

## 📁 Project Structure

```
productsApp-improved/
├── Controllers/          # MVC Controllers (Products, Home, etc.)
├── Views/               # Razor templates
│   ├── Products/        # Product pages
│   ├── Home/            # Home page
│   └── Shared/          # Layout & shared templates
├── Models/              # Data models
├── Data/                # DbContext & migrations
├── Hubs/                # SignalR hubs (ChatHub, OrderNotiHub)
├── Resources/           # Localization files (EN-US, AR-SA)
├── wwwroot/             # Static files
│   ├── js/              # JavaScript
│   ├── css/             # Stylesheets
│   └── serviceworker.js # PWA service worker
├── Program.cs           # App configuration
└── appsettings.json     # Settings
```

---

## 🔐 User Roles

### **Customer**
- Browse & search products
- Manage shopping cart
- Place & track orders
- Real-time order notifications
- Live chat support

### **Admin**
- Manage users & roles
- Monitor all orders
- View audit logs
- System settings

---

## 🔄 Order Workflow

```
1. Customer browses products
   ↓
2. Adds items to cart
   ↓
3. Checkout & payment
   ↓
4. Order created in DB
   ↓
5. Real-time notifications sent:
   • SignalR (in-app)
   • Web Push (OS notification)
   ↓
6. Admin & Customer track order
```

---

## 🗄️ Database Schema

### Core Tables:

| Table | Description | Key Fields |
|-------|-------------|-----------|
| **Users** (Identity) | User accounts & authentication | Email, Username, Password, Roles |
| **Products** | Product catalog | Id, Name, Price, Stock, CreatedAt |
| **Orders** | Customer orders | Id, CustomerId, TotalAmount, Status |
| **OrderItems** | Order line items | OrderId, ProductId, Quantity, Price |
| **Notifications** | User notifications | Id, UserId, Title, Message, IsRead |
| **AuditLogs** | Activity tracking | EntityName, ActionType, UserId, Timestamp |

### Key Relationships:
- **Users → Orders**: One-to-Many (One customer has many orders)
- **Users → Notifications**: One-to-Many (One user receives many notifications)
- **Orders → OrderItems**: One-to-Many (One order has many items)
- **OrderItems → Products**: Many-to-One (Many order items reference products)
- **Users → AuditLogs**: One-to-Many (One user's actions create many audit logs)

---

## 🚀 Quick Start

### Prerequisites:
- .NET SDK 7.0 or 8.0 (install from https://dotnet.microsoft.com)
- SQL Server 2019+ or LocalDB (for development)
- Visual Studio 2022 / 2022 Preview or VS Code

### Setup:

1. Clone & Restore
```bash
git clone https://github.com/Lu671/productsApp-improved.git
cd productsApp-improved
dotnet restore
```

2. Apply Migrations
- If the repository already contains migrations (check the `Data/Migrations` folder), run:
```bash
# Run this from the project folder that contains Program.cs (or specify --project)
dotnet ef database update
```
- If there are no migrations included, create and apply the initial migration:
```bash
dotnet ef migrations add InitialCreatedotnet ef database update
```
If your solution has multiple projects, run the commands from the API/project folder or add `--project <PROJECT.csproj>` and `--startup-project <STARTUP_PROJECT.csproj>` as needed.

3. Run Application
```bash
# From the project directory that contains Program.cs
dotnet run
```
- After the app starts, look for the listening URL in the console output (usually `https://localhost:5001` or a similar port) and open that URL in your browser.
---

## 🔔 Notification System

### SignalR Integration
- Real-time order updates
- Live chat messaging
- Instant in-app alerts

### Web Push
- OS-level notifications via Service Worker
- Works even when app is closed
- User permission required on first visit

---

## 🌍 Localization

Supported Languages:
- 🇺🇸 **English** (en-US)
- 🇸🇦 **Arabic** (ar-SA)

Switching via URL culture parameter or user preference.

---
