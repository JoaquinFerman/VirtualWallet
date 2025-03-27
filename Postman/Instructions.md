---
Take in mind that all requests except the login one require the proper jwt token included in the response of the forementioned one. It must be included as a header under the "Authorization" Key, following "Bearer + token" as the value 

# User Functions

### Login:
POST
URL: base/login
Body:
{
    "Username" : username,
    "Password" : password
}

### Balance:
GET
URL: base/balance

### Deposit:
PUT
URL: base/deposit?amount=amount

### Transfer:
POST
URL: base/transfer
Body:
{
    "Username" : target,
    "Amount" : amount
}

### Check Loans:
GET
URL: base/loans

### Request Loan:
POST
URL: base/loans
Body:
{
    "Value" : value,
    "LoanType" : loantype,
    "Interest" : interest
}

### Pay Loan:
PUT
URL: base/loans
Body:
{
    "Id" : loanid,
    "Amount" : amount
}

# Admin Functions

### Add User
POST
URL: base/admin
Body:
{
    "Name" : name,
    "Surname" : surname,
    "Username" : username,
    "Password" : password
}

### Get All Users
GET
URL: base/admin

### Get Loans in a Certain State
GET
URL: base/loans/admin?state=state

could be pending ones to approve, defaulted to close accounts, etc, etc.

### Approve/Reject Loan
PUT
URL: base/loans/admin
Body:
{
    "LoanId" : loanid,
    "State" : approve/reject
}