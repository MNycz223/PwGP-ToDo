# ToDo App

> [!CAUTION]
> This project is for educational purposes only.

A simple ToDo application built for educational purposes.
This project demonstrates a basic full-stack setup using ASP.NET Core and Blazor WebAssembly, showing how a frontend client can communicate with a backend API.

## Project structure
The solution consists of two main parts:
- **Backend** (ASP.NET Core Web API) – Responsible for managing ToDo items and exposing RESTful endpoints.
- **Client** (Blazor WebAssembly) – A web frontend that consumes the backend API and allows users to manage their ToDo list.

The client communicates with the backend via HTTP requests to perform CRUD operations on ToDo items.

## How to run the project
1. Build the solution:
    ```bash
    dotnet build
    ```
2. Run the API:
    ```bash
    dotnet run --project backend
    ```
3. Run the client:
    ```bash
    dotnet run --project client
    ```
Once both projects are running, open your browser and navigate to the client URL to start using the application.