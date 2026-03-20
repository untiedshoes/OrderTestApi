# OrderTestApi

![GitHub repo size](https://img.shields.io/github/repo-size/untiedshoes/OrderTestApi)
![GitHub last commit](https://img.shields.io/github/last-commit/untiedshoes/OrderTestApi)
![GitHub license](https://img.shields.io/github/license/untiedshoes/OrderTestApi)
![Build Status](https://github.com/untiedshoes/OrderTestApi/actions/workflows/test.yml/badge.svg?branch=master)

A small ASP.NET Core Web API designed to demonstrate clean, testable API patterns and automated testing in a simple order management sample.

This project is intended as a learning and demonstration API, showcasing best practices for ASP.NET Core Web API design, dependency injection, testing, and CI/CD readiness.

---

## 🚀 Overview

OrderTestApi is a minimal ASP.NET Core Web API that exposes basic endpoints for working with order resources. It includes:

- RESTful API controllers  
- Models and DTOs  
- Automated tests (unit tests)  
- Example project structure for scalable APIs  

This project is useful as a reference or starting point for building production‑ready APIs in .NET. It’s also used internally for learning, experimentation, and clean architecture demonstration.

---

## 🛠 Features

- RESTful API endpoints for working with orders  
- Structured project layout  
- Automated testing project included  
- Simple, clean code that’s easy to extend  
- Designed with testability and maintainability in mind

---

## 📦 Getting Started

### Prerequisites

Make sure you have the following installed:

- [.NET SDK 7.0 or newer](https://dotnet.microsoft.com/download)
- A code editor such as Visual Studio, Visual Studio Code, or JetBrains Rider

---

### 🏗 Build and Run

Clone the repo:

```bash
git clone https://github.com/untiedshoes/OrderTestApi.git
cd OrderTestApi
```

Build the solution:
```bash
dotnet build
```

Run the API:
```bash
dotnet run --project OrderTest.Api
```

By default, the API will start on http://localhost:5000 (or similar).

---

### 📋 Example Endpoints
Replace localhost:5000 with your configured base URL if different.

**Get All Orders**
```
GET /api/orders
```

**Get Order by ID**
```
GET /api/orders/{id}
```

**Create a New Order**
```
POST /api/orders
Content-Type: application/json

{
  "orderNumber": "ORD123",
  "customerName": "Alice",
  "items": [
    {
      "productId": "ABC",
      "quantity": 2
    }
  ]
}
```

Successful responses return HTTP 200 OK or 201 Created with the new resource details.

---

### 🧪 Running Tests

OrderTestApi includes a test project showing how to structure automated tests for web APIs.

Run tests using the .NET CLI:
```bash
dotnet test
```

---

### 🧠 Project Structure

```
OrderTestApi/
├── OrderTest.Api/            # Main Web API project
├── OrderTest.Api.Tests/      # Automated tests for API
└── OrderTest.sln             # Solution file
```
This layout keeps API logic separate from tests, improving maintainability and clarity.

---

### ℹ️ Notes
This project is intended to help developers explore clean ASP.NET Core Web API design. It is not a full eCommerce backend, but rather a simple demonstration of API principles, routing, models, and testing.

### 📌 Author
Craig Richards
.NET Developer
👉 https://github.com/untiedshoes
