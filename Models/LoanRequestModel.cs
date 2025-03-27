using PrimeraWebAPI.Services;

namespace PrimeraWebAPI.Models {
    public class LoanRequestModel {
        public int Id { get; set; }
        public string Username { get; set; }
        public LoanModel Loan { get; set; }
        public string State { get; set; }

        public LoanRequestModel(string username, LoanModel loan) {
            Id = LoansService.GetNextId();
            Username = username;
            Loan = loan;
            State = "Pending";
        }
    }
}