---
This is a fullstack proyect, with the purpose of simulate a virtual wallet application.

The main uses of this app are to deposit, check balance, transfer, and manage loans. An administrator account could as well reject/approve any pending loan and manage accounts.

# Start

To start the application one must first install the .NET SDK and Node.js

The second step is to create files for the public and private keys used by the app (leave it with Program.cs), and declare enviroment variables for the Jwt key, the mongoDb credentials, and BCrypt secret. Example:

```bash
export Jwt__Key="JwtKey"  
export MongoDbUsername="username"  
export MongoDbPassword="password"  
export BCryptSecret="secret"
```
For the database from mongoDb one must first create a database named Users (former labeling error), with inside the collections Users and LoanRequests. The Users collection must have at least one user created for the app to work properly, because otherwise one could not create another users or manage the app in general. The user must have a Username, Password, and IsAdmin=True.

Download .csproj dependencies, only the first time its run:
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
