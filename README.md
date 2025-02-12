# Library Management System

The Library Management System is an ASP.NET Core Web API project designed to manage books and members.
It includes features like user authentication, role-based access control, and JWT token-based security.

## Features

- **User Authentication**:
    - Register and log in with email and password.
    - JWT token-based authentication.
- **Role-Based Access Control**:
    - Roles: `Admin`, `Librarian`, `Member`.
    - Custom claims for fine-grained access control.
- **Database Relationships**:
    - One-to-One: A `Member` has one `LibraryCard`.
    - One-to-Many: An `Author` can write many `Books`.
    - Many-to-Many: A `Book` can belong to many `Categories`.
- **API Endpoints**:
    - Account: Register and login.
    - Books: Manage books (CRUD operations).
    - Members: Manage members (CRUD operations).
- **AutoMapper**:
    - Maps entities to DTOs for clean separation of concerns.

## API Endpoints

### Account Controller

| Method | Endpoint                   | Description                |
|--------|----------------------------|----------------------------|
| POST   | `/api/v1/Account/register` | Register a new user.       |
| POST   | `/api/v1/Account/login`    | Log in and get JWT tokens. |

### Books Controller

| Method | Endpoint            | Description       |
|--------|---------------------|-------------------|
| POST   | `/api/v1/Book`      | Add a new book.   |
| GET    | `/api/v1/Book`      | Get all books.    |
| GET    | `/api/v1/Book/{id}` | Get a book by ID. |
| PUT    | `/api/v1/Book/{id}` | Update a book.    |
| DELETE | `/api/v1/Book/{id}` | Delete a book.    |

### Members Controller

| Method | Endpoint              | Description         |
|--------|-----------------------|---------------------|
| POST   | `/api/v1/Member`      | Add a new member.   |
| GET    | `/api/v1/Member`      | Get all members.    |
| GET    | `/api/v1/Member/{id}` | Get a member by ID. |
| PUT    | `/api/v1/Member/{id}` | Update a member.    |
| DELETE | `/api/v1/Member/{id}` | Delete a member.    |
