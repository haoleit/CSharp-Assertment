# Task Management System

## Description

A simple task management system built with a React frontend, .NET Core backend, and PostgreSQL database. The application supports task creation, tracking, and status management, and is designed to be deployed either locally or via Docker.

## Getting Started

### Prerequisites

- Docker
- Docker Compose
- .NET SDK 9.0+
- Node.js (for frontend development)

### Running the Application

#### 🔧 Option 1: Run Locally

1.  **Start PostgreSQL Database**
    ```bash
    docker-compose -f docker-compose.db.yml up -d
    ```
2.  **Backend Setup**
    ```bash
    cd backend/TaskManagerAPI
    dotnet ef database update
    dotnet run
    ```
3.  **Frontend Setup**
    ```bash
    cd frontend/react
    npm install
    npm run dev
    ```

#### 🐳 Option 2: Run with Docker (Backend + Frontend)

```bash
docker-compose -f docker-compose.yml up -d
```

### Services and URLs

- **Frontend:** http://localhost:5173
- **Backend API:** http://localhost:5069
- **API Documentation (Swagger):** http://localhost:5069/swagger

## API Documentation

The API documentation is available via Swagger at the `/swagger` endpoint of the backend server.


