- Running the Project
Start the project normally.
If you can see the Swagger UI, the project is running correctly.

- Database Setup:
1. Create a new database in SQL Server
Example name: EcommerceDb
2. Update connection string in file file appsettings.json: Ecommerce-api/appsettings.json
3. Apply Entity Framework migrations:
Open Package Manager Console
Choose default project EcommerceApi
Run "Update-Database"
This will create all tables inside EcommerceDb.

- Authentication (Login):
Start the project and open Swagger
Use the login endpoint: api/auth/login
With admin credentials: admin@test.com / 123456
Copy the returned JWT token
-> Click Authorize (top right of Swagger): Bearer **yourToken**
Verify token using: api/auth/auth-check, If the response is: "message": "Your token is valid".
-> Authentication is working properly.
  
- Adding Sample Products
After authentication:
Go to POST /api/products
Add a sample product with variants
If the product is created successfully, your API setup is complete.

- How to Test This Project

After starting both FE & API:

1. Open the login page  
2. Use one of the following demo accounts:

**Admin**
- Email: `admin@test.com`
- Password: `123456`

**Staff**
- Email: `staff@test.com`
- Password: `123456`
