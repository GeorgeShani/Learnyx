# Learnyx Frontend Application

A modern Angular 19 Learning Management System (LMS) frontend application built with TypeScript and SCSS.

## 🚀 Overview

Learnyx is a comprehensive SaaS Learning Management System that provides an intuitive interface for students, instructors, and administrators to manage courses, assignments, and learning progress. The frontend is built using Angular 19 with a feature-based architecture and modern development practices.

## 📋 Table of Contents

- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [Getting Started](#getting-started)
- [Development](#development)
- [Architecture](#architecture)
- [Components Overview](#components-overview)
- [Services](#services)
- [Routing](#routing)
- [Authentication](#authentication)
- [Styling](#styling)
- [Build & Deployment](#build--deployment)

## 🛠 Technology Stack

- **Framework**: Angular 19.2.0
- **Language**: TypeScript 5.7.2
- **Styling**: SCSS
- **State Management**: RxJS
- **Real-time Communication**: Microsoft SignalR 9.0.6
- **Authentication**: JWT with OAuth (Google, Facebook)
- **HTTP Client**: Angular HttpClient
- **Markdown Support**: ngx-markdown 19.1.1
- **Build Tool**: Angular CLI 19.2.1
- **Testing**: Jasmine & Karma

## 📁 Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/                    # Core functionality
│   │   │   ├── guards/              # Route guards
│   │   │   ├── interceptors/        # HTTP interceptors
│   │   │   ├── models/              # Core data models
│   │   │   └── services/            # Core services
│   │   ├── features/                # Feature modules
│   │   │   ├── auth/                # Authentication module
│   │   │   ├── dashboard/           # Dashboard module
│   │   │   ├── learning/            # Learning/Course module
│   │   │   └── main/                # Main public pages
│   │   ├── layout/                  # Layout components
│   │   │   ├── auth-layout/         # Authentication layout
│   │   │   ├── dashboard-layout/    # Dashboard layout
│   │   │   ├── error-layout/        # Error page layout
│   │   │   ├── learning-layout/     # Learning layout
│   │   │   └── main-layout/         # Main public layout
│   │   ├── shared/                  # Shared components
│   │   │   ├── components/          # Reusable components
│   │   │   ├── directives/          # Custom directives
│   │   │   ├── models/              # Shared models
│   │   │   ├── pipes/               # Custom pipes
│   │   │   └── services/            # Shared services
│   │   ├── app.component.*          # Root component
│   │   ├── app.config.ts            # App configuration
│   │   └── app.routes.ts            # Main routing
│   ├── assets/                      # Static assets
│   │   ├── fonts/                   # Custom fonts
│   │   ├── icons/                   # SVG icons
│   │   └── images/                  # Images
│   ├── environments/                # Environment configs
│   │   ├── environment.ts           # Production config
│   │   └── environment.development.ts # Development config
│   ├── styles/                      # Global styles
│   ├── index.html                   # Main HTML file
│   └── main.ts                      # Application entry point
├── public/                          # Public assets
├── angular.json                     # Angular CLI configuration
├── package.json                     # Dependencies and scripts
└── tsconfig.json                    # TypeScript configuration
```

## ✨ Features

### 🔐 Authentication & Authorization

- **Multi-provider Authentication**: Email/password, Google OAuth, Facebook OAuth
- **JWT-based Authorization**: Secure token-based authentication
- **Role-based Access Control**: Student, Instructor, Admin roles
- **Route Guards**: Protected routes with authentication checks
- **Password Reset**: Forgot password functionality

### 📊 Dashboard System

- **Role-specific Dashboards**: Customized views for different user types
- **Student Dashboard**: Course progress, assignments, grades
- **Instructor Dashboard**: Course management, student tracking, grading
- **Admin Dashboard**: User management, system overview, analytics

### 🎓 Learning Management

- **Course Browser**: Browse and search available courses
- **Course Details**: Detailed course information and enrollment
- **Video Player**: Integrated video player for course content
- **Assignment System**: Submit and grade assignments
- **Progress Tracking**: Visual progress indicators and completion tracking

### 💬 Real-time Communication

- **Live Chat**: Real-time messaging between users
- **Notifications**: Push notifications for important updates
- **SignalR Integration**: WebSocket-based real-time features

### 🎨 User Interface

- **Responsive Design**: Mobile-first responsive layout
- **Modern UI Components**: Clean, intuitive interface
- **Accessibility**: WCAG compliant components
- **Dark/Light Theme Support**: Theme switching capability

## 🚀 Getting Started

### Prerequisites

- Node.js (v18 or higher)
- npm (v9 or higher)
- Angular CLI 19.2.1

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/GeorgeShani/Learnyx.git
   cd learnyx/frontend
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Start the development server**

   ```bash
   npm start
   # or
   ng serve
   ```

4. **Open your browser**
   Navigate to `http://localhost:4200/`

## 🛠 Development

### Available Scripts

```bash
# Start development server
npm start
ng serve

# Build for production
npm run build
ng build

# Build with watch mode
npm run watch
ng build --watch --configuration development

# Run unit tests
npm test
ng test

# Run end-to-end tests
ng e2e
```

### Code Generation

Angular CLI provides powerful code generation tools:

```bash
# Generate a new component
ng generate component component-name

# Generate a new service
ng generate service service-name

# Generate a new guard
ng generate guard guard-name

# Generate a new interceptor
ng generate interceptor interceptor-name

# See all available schematics
ng generate --help
```

## 🏗 Architecture

### Feature-Based Architecture

The application follows a feature-based architecture pattern:

- **Core Module**: Contains shared functionality, guards, interceptors, and core services
- **Feature Modules**: Self-contained modules for specific features (auth, dashboard, learning, main)
- **Layout Components**: Reusable layout templates for different sections
- **Shared Module**: Common components, pipes, and utilities

### State Management

- **RxJS Observables**: Reactive programming for data flow
- **Service-based State**: Centralized state management through services
- **BehaviorSubjects**: For component communication and state sharing

### Dependency Injection

- **Hierarchical DI**: Angular's built-in dependency injection system
- **Service Providers**: Services are provided at appropriate levels
- **Interface-based Design**: Contracts defined through TypeScript interfaces

## 🧩 Components Overview

### Core Components

| Component         | Purpose                 | Location             |
| ----------------- | ----------------------- | -------------------- |
| `ApiService`      | HTTP client wrapper     | `core/services/`     |
| `AuthGuard`       | Route protection        | `core/guards/`       |
| `AuthInterceptor` | JWT token injection     | `core/interceptors/` |
| `SignalRService`  | Real-time communication | `core/services/`     |

### Feature Components

#### Authentication Module

- `LogInComponent` - User login form
- `SignUpComponent` - User registration form
- `ForgotPasswordComponent` - Password reset form
- `CallbackComponent` - OAuth callback handler

#### Dashboard Module

- `StudentDashboardComponent` - Student-specific dashboard
- `TeacherDashboardComponent` - Instructor dashboard
- `AdminDashboardComponent` - Administrative dashboard
- `CourseBuilderComponent` - Course creation interface
- `FilterModalComponent` - Data filtering modal

#### Learning Module

- `CourseComponent` - Course content viewer
- `AssignmentListComponent` - Assignment management
- `CourseProgressComponent` - Progress tracking
- `VideoPlayerComponent` - Video content player
- `MessagingComponent` - Chat interface

#### Main Module

- `HomeComponent` - Landing page
- `CoursesComponent` - Course catalog
- `CourseDetailsComponent` - Course information
- `ProfileComponent` - User profile management
- `AboutComponent` - About page
- `ContactComponent` - Contact form

### Layout Components

- `MainLayoutComponent` - Public pages layout
- `AuthLayoutComponent` - Authentication pages layout
- `DashboardLayoutComponent` - Dashboard pages layout
- `LearningLayoutComponent` - Learning interface layout
- `ErrorLayoutComponent` - Error pages layout

### Shared Components

- `HeaderComponent` - Navigation header
- `FooterComponent` - Site footer
- `NotificationCenterComponent` - Notification management

## 🔧 Services

### Core Services

#### ApiService

Centralized HTTP client with standardized methods:

```typescript
get<T>(endpoint: string, params?: HttpParams): Observable<T>
post<T>(endpoint: string, body: any): Observable<T>
put<T>(endpoint: string, body: any): Observable<T>
delete<T>(endpoint: string): Observable<T>
```

#### AuthService

Authentication and authorization:

- User registration and login
- OAuth integration (Google, Facebook)
- Token management
- Social authentication flows

#### TokenService

JWT token handling:

- Token storage and retrieval
- Token validation
- Automatic token refresh

#### SignalRService

Real-time communication:

- WebSocket connection management
- Message broadcasting
- Connection state handling

### Feature Services

#### ProfileService

User profile management:

- Profile data retrieval
- Profile updates
- Avatar management

#### ChatApiService

Chat functionality:

- Message history
- Real-time messaging
- Chat state management

#### ForgotPasswordService

Password recovery:

- Password reset requests
- Token validation
- Password update

## 🛣 Routing

### Route Structure

```typescript
const routes: Routes = [
  {
    path: "", // Public pages
    loadComponent: () => import("./layout/main-layout/main-layout.component"),
    loadChildren: () => import("./features/main/main.routes"),
  },
  {
    path: "auth", // Authentication pages
    loadComponent: () => import("./layout/auth-layout/auth-layout.component"),
    loadChildren: () => import("./features/auth/auth.routes"),
  },
  {
    path: "dashboard", // Dashboard (protected)
    canActivate: [authGuard],
    loadComponent: () => import("./layout/dashboard-layout/dashboard-layout.component"),
    loadChildren: () => import("./features/dashboard/dashboard.routes"),
  },
  {
    path: "learning", // Learning interface (protected)
    canActivate: [authGuard],
    loadComponent: () => import("./layout/learning-layout/learning-layout.component"),
    loadChildren: () => import("./features/learning/learning.routes"),
  },
  {
    path: "**", // 404 handler
    loadComponent: () => import("./layout/error-layout/error-layout.component"),
  },
];
```

### Lazy Loading

All feature modules use lazy loading for optimal performance:

- Reduced initial bundle size
- Faster application startup
- On-demand module loading

### Route Guards

- `AuthGuard`: Protects authenticated routes
- `RoleGuard`: Enforces role-based access control

## 🔐 Authentication

### Authentication Flow

1. **Login/Registration**: User provides credentials or uses OAuth
2. **Token Generation**: Backend generates JWT token
3. **Token Storage**: Token stored in browser storage
4. **Request Interception**: AuthInterceptor adds token to requests
5. **Route Protection**: Guards check authentication status

### OAuth Integration

#### Google OAuth

```typescript
// Configuration
clientId: "348733949068-hvcqp682kgdr6499lka5jsujvmb1f1ud.apps.googleusercontent.com";
redirectUri: "https://localhost:7188/api/auth/google/callback";
scopes: "openid email profile";
```

#### Facebook OAuth

```typescript
// Configuration
clientId: "1406683273763679";
redirectUri: "https://localhost:7188/api/auth/facebook/callback";
scopes: "email,public_profile";
```

### Security Features

- **JWT Token Management**: Secure token handling
- **HTTP Interceptors**: Automatic token injection
- **Route Guards**: Protected route access
- **CORS Configuration**: Cross-origin request handling
- **Input Validation**: Client-side form validation

## 🎨 Styling

### SCSS Architecture

```
src/styles/
├── styles.scss              # Main stylesheet
├── _variables.scss          # SCSS variables
├── _mixins.scss             # Reusable mixins
├── _base.scss               # Base styles
├── _components.scss         # Component styles
├── _layouts.scss            # Layout styles
└── _utilities.scss          # Utility classes
```

### Design System

- **Color Palette**: Consistent color scheme
- **Typography**: Standardized font families and sizes
- **Spacing**: Consistent margin and padding system
- **Components**: Reusable UI component library
- **Responsive Design**: Mobile-first approach

### Assets

- **Icons**: SVG icon library in `src/assets/icons/`
- **Images**: Optimized images in `src/assets/images/`
- **Fonts**: Custom fonts in `src/assets/fonts/`

## 🚀 Build & Deployment

### Build Configurations

#### Development Build

```bash
ng build --configuration development
```

- Source maps enabled
- No optimization
- Development environment variables

#### Production Build

```bash
ng build --configuration production
```

- Code optimization and minification
- Tree shaking
- Bundle optimization
- Production environment variables

### Build Output

The build process generates optimized files in the `dist/learnyx/` directory:

- `index.html` - Main HTML file
- `*.js` - Bundled JavaScript files
- `*.css` - Bundled stylesheets
- `assets/` - Static assets

### Performance Optimizations

- **Code Splitting**: Automatic route-based code splitting
- **Lazy Loading**: Feature modules loaded on demand
- **Tree Shaking**: Unused code elimination
- **Bundle Optimization**: Minimized bundle sizes
- **Asset Optimization**: Compressed images and fonts

### Deployment Considerations

- **Environment Variables**: Configure API endpoints for different environments
- **CORS Settings**: Ensure proper CORS configuration for production
- **HTTPS**: Enable HTTPS for production deployment
- **CDN**: Consider using CDN for static assets
- **Caching**: Implement proper caching strategies

## 📚 Additional Resources

- [Angular Documentation](https://angular.dev)
- [Angular CLI Guide](https://angular.dev/tools/cli)
- [RxJS Documentation](https://rxjs.dev)
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr)
- [JWT.io](https://jwt.io) - JWT token debugging

## 🤝 Contributing

1. Follow the established code structure and naming conventions
2. Use TypeScript strict mode
3. Write meaningful commit messages
4. Test your changes thoroughly
5. Update documentation as needed

## 📄 License

This project is part of the Learnyx Learning Management System.
