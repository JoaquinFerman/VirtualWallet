using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeraWebAPI.Models;

namespace PrimeraWebAPI.Controllers {
    [ApiController]
    [Route("api/v0/[controller]")]
    [Authorize]
    public class LoansController : ControllerBase {
        [HttpPost]
        [Authorize]
        public IActionResult RequestLoan([FromBody] LoanModel loan) {
            List<User> users = UsersService.GetUsers();
            User user = users.Find(u => u.Username == User.FindFirst(ClaimTypes.NameIdentifier).ToString().Split(" ")[1]);
            loan.UpdateInterest(user);
            if(user.Loans.Count() > 5) {
                return BadRequest("You have reached the maximum number of loans");
            }
            if(user.Loans.Any(l => l.Defaulting)) {
                return BadRequest("You have a defaulting loan, pay off it before taking a new one");
            }
            if(loan.Value == 0) {
                return BadRequest("Amount cannot be zero");
            }
            if(loan.Value < 0) {
                return BadRequest("Amount cannot be negative");
            }
            if(loan.Interest == 0) {
                return BadRequest("Interest cannot be zero");
            }
            if(loan.Interest < 0) {
                return BadRequest("Interest cannot be negative");
            }
            if (!new HashSet<string> { "SimpleInterest", "CompoundInterest", "Mortgage" }.Contains(loan.LoanType)) {
                return BadRequest("Invalid loan type. Must be SimpleInterest, CompoundInterest, or Mortgage.");
            }
            if(LoansService.GetPendingLoans().Any(l => l.Username == user.Username)){
                return BadRequest("You already have a loan request awaiting approval");
            }

            loan.Defaulting = false;
            loan.RemainingValue = loan.Value;
            loan.Id = LoansService.GetNextUserId(user);
            LoanRequestModel newLoan = new LoanRequestModel(user.Username, loan);
            LoansService.AddLoanRequest(newLoan);
            return Ok("Applied to loan successfully");
        }

        public class LoanPaymentModel {
            public int Id { get; set; }
            public int Amount { get; set; }
        }
        
        [HttpPut]
        [Authorize]
        public IActionResult PayLoan([FromBody] LoanPaymentModel payment) {
            List<User> users = UsersService.GetUsers();
            User user = users.Find(u => u.Username == User.FindFirst(ClaimTypes.NameIdentifier).ToString().Split(" ")[1]);
            LoanModel loan = user.Loans.Find(l => l.Id == payment.Id);
            if (loan == null) {
                return NotFound("Loan not found");
            }
            loan.UpdateInterest(user);
            if(payment.Amount == 0) {
                return BadRequest("Amount cannot be zero");
            }
            if(payment.Amount < 0) {
                return BadRequest("Amount cannot be negative");
            }
            if(payment.Amount > loan.RemainingValue){
                return BadRequest("Amount cannot be greater than the remaining value, remaining value: $" + loan.RemainingValue);
            }
            if(payment.Amount > user.Balance){
                return BadRequest("Amount cannot be greater than the user balance, user balance: $" + user.Balance);
            }
            user.Balance -= payment.Amount;
            loan.Pay(payment.Amount);
            if (loan.RemainingValue == 0){
                user.Loans.Remove(loan);
                LoansService.FinishLoanRequest(user, loan);
                UsersService.UpdateUser(user);
                return Ok("Payment successful, loan fully paid");
            } else {
                UsersService.UpdateUser(user);  
                return Ok("Payment successful, remaining value: $" + loan.RemainingValue + "\nNew balance: $" + user.Balance);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetLoans() {
            List<User> users = UsersService.GetUsers();
            User user = users.Find(u => u.Username == User.FindFirst(ClaimTypes.NameIdentifier).ToString().Split(" ")[1]);
            foreach (LoanModel loan in user.Loans){
                loan.UpdateInterest(user);
            }   
            return Ok( new {Loans = user.Loans.Where(l => !l.Defaulting)});
        }

        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAwaitingApproval([FromQuery] string state = "all"){

            switch (state) {
                case "all":
                    return Ok(LoansService.GetLoans());
                case "pending":
                    return Ok(LoansService.GetPendingLoans());
                case "active":
                    return Ok(LoansService.GetActiveLoans());
                case "rejected":
                    return Ok(LoansService.GetRejectedLoans());
                case "defaulted":
                    return Ok(LoansService.GetDefaultedLoans());
                default:
                    return BadRequest("Invalid state");
            }
        }

        [HttpGet("admin/{username}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetUserLoans(string username){
            List<User> users = UsersService.GetUsers();
            User user = users.Find(u => u.Username == username);
            if(user == null){
                return NotFound("User not found");
            }
            foreach (LoanModel loan in user.Loans){
                loan.UpdateInterest(user);
            }
            return Ok(new {Loans = user.Loans});
        }

        public class ApprovalModel {
            public int LoanId { get; set; }
            public string State { get; set; }
        }

        [HttpPut("admin")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Approve([FromBody] ApprovalModel approval){
            List<User> users = UsersService.GetUsers();
            List<LoanRequestModel> loans = LoansService.GetLoans();
            LoanRequestModel loanRequest = loans.Find(l => l.Id == approval.LoanId);
            User user = users.Find(u => u.Username == loanRequest.Username);
            if(approval.State == "Approve"){
                UsersService.AproveLoan(user.Username, loanRequest);
                LoansService.ApproveLoanRequest(loanRequest);
                return Ok("Loan approved");
            }else if (approval.State == "Reject"){
                UsersService.RejectLoan(user.Username);
                LoansService.RejectLoanRequest(loanRequest);
                return Ok("Loan rejected");
            } else {
                return BadRequest("Invalid loan state");
            }
        }
    }
}