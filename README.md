---
This is a fullstack proyect, with the purpose of simulate a virtual wallet application.

The main uses of this app are to deposit, check balance, transfer, and manage loans. An administrator account could as well reject/approve any pending loan and manage accounts.

# Start

To start the application one must first install the .NET SDK and Node.js

The second step is to create files for the public and private keys used by the app (leave it with Program.cs) as private/public.pem, and declare enviroment variables for the Jwt key, the mongoDb credentials, and BCrypt secret. Example:

```bash
export Jwt__Key="JwtKey"  
export MongoDbAccess="mongodb+srv://your_acess"
export BCryptSecret="secret"
```
For the database from mongoDb one must first create a database named Users (former labeling error), with inside the collections Users and LoanRequests. The Users collection must have at least one user created for the app to work properly, because otherwise one could not create another users or manage the app in general. The user must have a Username, Password, and IsAdmin=True.

Download dependencies from .csproj, only the first time its run:
```bash
dotnet restore
```
And then run the proyect from the main directory:

```bash
dotnet run
```
# Usage

The front is designed to be just for the normal users, so exclusively Postman can be used for the admin functions.

The first screen is the login one, where one can enter its credentials

Once the login is succesfull, there is the home page or main menu, where you can access the main functions of the app:
- Balance: Check ones balance.
- Deposit: Input and enter an amount, which is directly added to the users account. (the purpose of this method is to have a fast way of getting money in the account, even though a thing like this doesnt make sense in a real wallet)
- Transfer: Enter a valid account username and an amount, and if its all valid, the accion gets done.
- Loans: Check ones active and defaulted loans, and request a custom loan. (for it to be active, an administrator must first approve it, and no more than one can be requested at a time)

# Additional Instructions

In the "Postman" directory there are instructions on how to make the requests in the forementioned tool, and the "Screenshots" one contains photos in the different screens of the app.

# Security Measures

- Once logged in, the app provides a JWT (saved as a cookie if logged from the front), which is needed for all the following functions. It contains the username and the admin statement, the first to identify in the functions, and the second to authorize the admin functions.
- The JWT firm is too encrypted using a private key. It is absolutely not necessary given the previous measure, but serves both as a good practice and as a way to provide a federated autentication if needed in the future.
- The IDs are not secuential GUIDs.
- The passwords are located in a mongoDb Atlas database, but hashed using the SHA384 logarithm.
