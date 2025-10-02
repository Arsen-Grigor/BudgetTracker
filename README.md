## Budget Tracker

A RESTful API for personal budget tracking and expense management. Implements Clean Architecture, SOLID principles, JWT authentication, logging, and MySQL database management with Entity Framework Core.
Git: https://github.com/Arsen-Grigor/BudgetTracker

<sub>*I’d be glad if you might also take a look at another project built on the Clean Architecture principle.*</sub>

<sub>https://github.com/Arsen-Grigor/Meeting-Scheduler</sub>

### Prerequisites

- .NET 9.0.
- Visual Studio or JetBrains Rider.
- xUnit.
- MySQL 8.0.
- Entity Framework Core CLI tools.

### API Endpoints

| Method | Description |
|-|-|
`POST /api/auth/login` | Authenticate and get JWT token
`GET  /api/categories`| Get all categories with subcategories
`GET  /api/records`| Get records by year/month (optional: subcategoryId)
`POST /api/records`| Create new financial record
`PUT /api/records/{id}`| Update existing record
`DELETE /api/records/{id}`| Delete record
`GET /api/budgets`| Get budgets by year/month
`POST /api/budgets`| Create new budget
`PUT /api/budgets/{id}` | Update budget planned amount
`DELETE /api/budgets/{id}`| Delete budget

### Examples of Requests

`POST /api/auth/login`

`{`

    `"username": "admin",`
    
    `"password": "Admin123!"`
    
`}`

`POST /api/records`

`{`

    `"amount": -50.00,`
    
    `"categoryId": 2,`
    
    `"subcategoryId": 5,`
    
    `"dateTime": "2025-09-25T14:30:00Z",`
    
    `"description": "Grocery shopping",`
    
    `"currency": "USD"`
    
`}`

`POST /api/budgets`

`{`

    `"year": 2025,`
    
    `"month": 9,`
    
    `"subcategoryId": 5,`
    
    `"plannedAmount": 500.00,`
    
    `"currency": "USD"`
    
`}`
