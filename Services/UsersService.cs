using MongoDB.Driver;
using PrimeraWebAPI.Models;

public class UsersService {
    private static readonly MongoClient client = MongoConnection.GetClient() != null ? MongoConnection.GetClient() : new MongoClient();
    private static readonly IMongoDatabase database = client.GetDatabase("Users");
    private static readonly IMongoCollection<User> usersDb = database.GetCollection<User>("Users");

    public UsersService() {
    }

    public static IMongoCollection<User> GetUsersDb() {
        return usersDb;
    }
    public static List<User> GetUsers() {
        return usersDb.Find(_ => true).ToList();
    }

    public static User SearchUser(string username) {
        var users = usersDb.Find(u => u.Username == username).ToList();
        return users.FirstOrDefault();
    }
    public static async Task UpdateUser(User user) {
        var filtro = Builders<User>.Filter.Eq(u => u.Username, user.Username);
        await usersDb.ReplaceOneAsync(filtro, user);
    }

    public static void AddUser(User user) {
        usersDb.InsertOneAsync(user);
    }

    public static bool DeleteUser(Guid id) {
        var filtro = Builders<User>.Filter.Eq(u => u.Id, id);
        var resultado = usersDb.DeleteOne(filtro);
        return resultado.DeletedCount > 0;
    }

    public static void AproveLoan(string username, LoanRequestModel loanRequest) {
        User user = SearchUser(username);
        user.LoanNews = true;
        user.LoanNewsState = true;
        loanRequest.Loan.LastPaid = DateTime.Now.AddMonths(1);
        user.Loans.Add(loanRequest.Loan);
        user.Balance += (int) loanRequest.Loan.Value;
        UpdateUser(user);
    }

    public static void RejectLoan(string username) {
        User user = SearchUser(username);
        user.LoanNews = true;
        user.LoanNewsState = false;
        UpdateUser(user);
    }
}
