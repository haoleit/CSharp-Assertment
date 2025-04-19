# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy the solution file and restore dependencies for all projects
COPY TaskManagementSystem.sln .
COPY backend/TaskManagerAPI/*.csproj ./backend/TaskManagerAPI/


# Copy the rest of the backend source code
COPY backend/TaskManagerAPI/. ./backend/TaskManagerAPI/

RUN dotnet restore 

# Build the TaskManagerAPI project
WORKDIR /source/backend/TaskManagerAPI
RUN dotnet build -c Release --no-restore

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release --no-build -o /app/publish

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the application runs on (check appsettings or Program.cs if needed, assuming 80/443 or 5000/5001 by default)
# ASP.NET Core apps typically listen on port 8080 inside containers by default with .NET 8+
EXPOSE 5069

# Define the entry point for the container
ENTRYPOINT ["dotnet", "TaskManagerAPI.dll"]
