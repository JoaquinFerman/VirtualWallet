using MongoDB.Driver;
using PrimeraWebAPI.Models;

public class LoansService {
    static string password = Environment.GetEnvironmentVariable("MongoDbPassword");
    static string connectionUri = "mongodb+srv://joacoferman:" + password + "@jferman.agip7.mongodb.net/?retryWrites=true&w=majority";
    static MongoClient client = new MongoClient(connectionUri);
    static IMongoDatabase database = client.GetDatabase("Users");
    static IMongoCollection<LoanRequestModel> loansDb = database.GetCollection<LoanRequestModel>("LoanRequests");

    public LoansService() {
    }
    public static List<LoanRequestModel> GetLoans() {
        return loansDb.Find(_ => true).ToList();
    }

    public static List<LoanRequestModel> GetPendingLoans() {
        return loansDb.Find(l => l.State == "Pending").ToList();
    }

    public static List<LoanRequestModel> GetDefaultedLoans() {
        return loansDb.Find(l => l.State == "Defaulted").ToList();
    }

    public static List<LoanRequestModel> GetActiveLoans() {
        return loansDb.Find(l => l.State == "Active").ToList();
    }

    public static List<LoanRequestModel> GetRejectedLoans() {
        return loansDb.Find(l => l.State == "Rejected").ToList();
    }

    public static List<LoanModel> GetUserLoans(User user) {
        List<LoanModel> userLoans = user.Loans;
        List<LoanModel> rejectedLoans = GetLoans().FindAll(l => l.Username == user.Username && l.State == "Rejected").Select(l => l.Loan).ToList();
        return userLoans.Concat(rejectedLoans).ToList();
    }

    public static int GetNextUserId(User user) {
        for (int i = 1; i <= user.Loans.Count() + 1; i++){
            if (user.Loans.Find(l => l.Id == i) == null){
                return i;
            }
        }
        return 0;
    }

    public static bool UpdateLoan(LoanRequestModel loanRequest) {
        var filtro = Builders<LoanRequestModel>.Filter.Eq(l => l.Id, loanRequest.Id);
        var resultado = loansDb.ReplaceOne(filtro, loanRequest);

        return resultado.ModifiedCount > 0;
    }

    public static int GetNextId() {
        var loans = loansDb.Find(_ => true).ToList();
        return loans.Count() == 0 ? 1 : loans.Max(l => l.Id) + 1;
    }

    public static void AddLoanRequest(LoanRequestModel loanRequest) {
        loansDb.InsertOneAsync(loanRequest);
    }

    public static void RejectLoanRequest(LoanRequestModel loanRequest) {
        LoanRequestModel loan = loansDb.Find(l => l.Id == loanRequest.Id).FirstOrDefault();
        loan.State = "Rejected";
        UpdateLoan(loan);
    }

    public static void ApproveLoanRequest(LoanRequestModel loanRequest) {
        LoanRequestModel loan = loansDb.Find(l => l.Id == loanRequest.Id).FirstOrDefault();
        loan.Loan.LastPaid = DateTime.Now;
        loan.State = "Active";
        UpdateLoan(loan);
    }

    public static void FinishLoanRequest(User user, LoanModel loan) {
        LoanRequestModel loanRequest = loansDb.Find(l => l.Username == user.Username && l.Loan.Id == loan.Id).FirstOrDefault();
        loanRequest.State = "Payed off";
        UpdateLoan(loanRequest);
    }

    public static void DefaultLoan(User user, LoanModel loan) {
        LoanRequestModel loanRequest = loansDb.Find(l => l.Username == user.Username && l.Loan.Id == loan.Id).FirstOrDefault();
        loanRequest.State = "Defaulted";
        UpdateLoan(loanRequest);
    }
}