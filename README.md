# 💖 LoveConnect - Modern Dating Platform

**Developer:** Oğuzhan Berke Özdil  
**Date:** July 7, 2025  
**Tech Stack:** ASP.NET Core 8.0, SQLite, Bootstrap 5

Modern dating web application with love-themed UI design and comprehensive administrative tools.

---

## 📚 **Project Background**

### **Original Project (2023):**
**WAT-Dating-WebSite**

This repository was originally created for a dating website project developed as a **group assignment** for the 3rd year, 6th semester 'Web Application Technologies' course at AGH University of Science and Technology, Krakow.

**Academic Details:**
- **Institution:** AGH University of Science and Technology, Krakow
- **Faculty:** Computer Science (Bachelor's Degree)  
- **Year:** 3rd Year, 6th Semester
- **Course:** Web Application Technologies
- **Created:** 2023

**Original Features (2023):**
- User authentication
- User profile creation
- Matchmaking algorithm
- Chat functionality

**How Original Version Worked:**
1. Users create an account and fill out their profile information
2. Users can browse through potential matches based on their preferences
3. Users can like or dislike potential matches
4. If two users like each other, they can start chatting

---

### **Current Version - Complete Redesign (2025):**
**LoveConnect - Professional Dating Platform**

This project has been completely rebuilt from scratch by **Oğuzhan Berke Özdil** (one of the original group members) with modern technologies, professional-grade features, and a comprehensive love-themed aesthetic design.

**Redesign Details:**
- **Developer:** Oğuzhan Berke Özdil
- **Redesign Date:** July 7, 2025
- **Status:** Completely modernized and enhanced
- **New Framework:** ASP.NET Core 8.0 (upgraded from original)
- **New Features:** Advanced admin dashboard, modern UI/UX, comprehensive user management, security enhancements

---

## ✨ **Core Features**

### 👤 **User Management System**
- **🔐 Secure Authentication** - Registration, login, password recovery with email verification
- **📝 Rich Profile Customization** - Detailed bio, interest tags, age preferences, multiple photo uploads
- **⚙️ Account Control** - Email updates, password changes, secure account deletion
- **🚫 Suspension Protection** - Automatic ban detection with appeals system

### 💖 **Advanced Dating Features**
- **🎯 Smart Matching Algorithm** - Age-based filtering with preference compatibility
- **🔍 Profile Discovery** - Explore potential matches with intelligent recommendations
- **❤️ Interactive Match System** - Like/dislike functionality with mutual match notifications
- **📱 Responsive Design** - Optimized experience across all devices

### 🛡️ **Comprehensive Admin Dashboard**
- **👥 User Management** - View, edit, ban/unban users with detailed profiles
- **📊 Real-time Analytics** - User statistics, registration trends, activity monitoring
- **🚫 Moderation Tools** - Content management, user reports, safety controls
- **🔧 System Maintenance** - Data export, test user cleanup, system health monitoring

### 🎨 **Modern UI/UX Design**
- **💕 Love-themed Color Palette** - Warm gradients, romantic aesthetics
- **✨ Smooth Animations** - Hover effects, transitions, loading states
- **🌈 Consistent Styling** - Unified design language across all pages
- **♿ Accessibility Features** - WCAG compliant, keyboard navigation support

---

## 🛠️ **Technical Stack**

### **Backend**
- **ASP.NET Core 8.0** - Modern web framework
- **Entity Framework Core** - Database ORM with SQLite
- **MVC Architecture** - Clean separation of concerns
- **Authentication & Authorization** - Role-based access control

### **Frontend**
- **Razor Pages** - Server-side rendering
- **Bootstrap 5** - Responsive grid system
- **Font Awesome** - Icon library
- **Custom CSS3** - Modern styling with CSS Grid/Flexbox
- **JavaScript ES6** - Interactive functionality

### **Database**
- **SQLite** - Lightweight, embedded database
- **Code-First Migrations** - Version controlled schema
- **Optimized Queries** - Performance tuned data access

---

## � **Project Structure**

```
LoveConnect/
├── Controllers/           # MVC Controllers
│   ├── AdminController.cs     # Admin dashboard & user management
│   ├── HomeController.cs      # Public pages & information
│   ├── LoginController.cs     # Authentication & security
│   ├── MyAccountController.cs # User profile management
│   ├── RegisterController.cs  # User registration
│   └── UserController.cs      # Dating features & matching
├── Models/               # Data models & entities
│   ├── User.cs               # User profile data
│   ├── BannedUser.cs        # Moderation system
│   └── ViewModels/          # Page-specific models
├── Views/                # Razor view templates
│   ├── Admin/               # Admin dashboard pages
│   ├── Home/                # Public information pages
│   ├── Login/               # Authentication pages
│   ├── MyAccount/           # Profile management
│   ├── Register/            # Registration flow
│   ├── User/                # Dating features
│   └── Shared/              # Layout & common views
├── Services/             # Business logic layer
│   ├── UserService.cs       # User operations
│   ├── BanService.cs        # Moderation logic
│   └── Repository.cs        # Data access
├── Data/                 # Database context & migrations
└── wwwroot/              # Static assets
    ├── css/site.css         # Custom styling
    ├── images/              # Profile photos & assets
    └── js/site.js           # Client-side functionality
```

---

## 🚀 **Key Achievements**

### **User Experience**
- ✅ Intuitive navigation with role-based menus
- ✅ Smooth form interactions with real-time validation
- ✅ Mobile-first responsive design
- ✅ Fast loading times with optimized assets

### **Security & Safety**
- ✅ Secure password handling with encryption
- ✅ CSRF protection on all forms
- ✅ SQL injection prevention
- ✅ User content moderation system

### **Performance**
- ✅ Optimized database queries with Entity Framework
- ✅ Efficient CSS/JS bundling
- ✅ Image optimization and lazy loading
- ✅ Minimal server response times

### **Maintainability**
- ✅ Clean, documented code structure
- ✅ Separation of concerns with MVC pattern
- ✅ Modular component design
- ✅ Comprehensive error handling

---

## 🎨 **Design Philosophy**

The application embraces a **love-themed aesthetic** with:
- **Warm Color Palette** - Soft pinks, corals, and romantic gradients
- **Emotional Design** - Heart icons, romantic imagery, welcoming typography
- **User-Centric Interface** - Intuitive workflows that reduce friction
- **Consistent Branding** - Unified visual language throughout the platform

---

## 🔧 **Installation & Setup**

1. **Clone the repository**

   ```bash
   git clone [repository-url]
   cd WAT-Dating-WebSite
   ```

2. **Install dependencies**

   ```bash
   dotnet restore
   ```

3. **Run database migrations**

   ```bash
   dotnet ef database update
   ```

4. **Start the application**

   ```bash
   dotnet run --project webappproject
   ```

5. **Access the application**
   - Application will start on available ports (usually `https://localhost:7273` or `http://localhost:5136`)
   - Default admin: `admin@loveconnect.com` / `Admin123!`

---

## 📊 **Development Notes**

### **Code Quality**

- Clean, maintainable codebase with comprehensive commenting
- Follows ASP.NET Core best practices and conventions
- Proper error handling and validation throughout
- Security-first approach with CSRF protection and input sanitization

### **Performance Optimizations**

- Efficient database queries with Entity Framework Core
- Optimized CSS/JS delivery with bundling and minification
- Responsive images and lazy loading implementation
- Minimal server response times with async operations

### **Testing & Deployment**

- Development environment configured for rapid iteration
- SQLite database for easy local development
- Ready for deployment to production environments
- Comprehensive logging and error tracking

---

**Evolution:** From university group assignment (2023) to professional-grade application (2025) showcasing modern web development best practices by one of the original team members.


