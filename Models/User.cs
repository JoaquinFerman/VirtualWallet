using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BCrypt.Net;
namespace PrimeraWebAPI.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public ObjectId MongoId { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Guid")]
        public Guid Id { get; set; }

        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int Balance { get; set; }
        public bool IsAdmin { get; set; }
        public List<LoanModel> Loans { get; set; }
        public bool LoanNews { get; set; }
        public bool LoanNewsState { get; set; }

        public User(string name, string surname, string username, string password) : this() {
            Name = name;
            Surname = surname;
            Username = username;
            Password = password;
        }

        public User(){
            Id = Guid.NewGuid();
            Username = "NoUsername";
            Name = "NoName";
            Surname = "NoSurname";
            Password = "NoPassword";
            Balance = 0;
            IsAdmin = false;
            Loans = new List<LoanModel>();
            LoanNews = false;
            LoanNewsState = false;
        }
    }
}
