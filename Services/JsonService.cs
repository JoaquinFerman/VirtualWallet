namespace PrimeraWebAPI.Services {

using System.Text.Json;
using Models;
    public class JsonService {
        private static readonly string _filePath = "Data/users.json";

        public static List<User> GetUsers() {
            if (!File.Exists(_filePath)) {
                return new List<User>();
            }

            string jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonString) ?? new List<User>();
        }

        private static void SaveUserFile(List<User> users) {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public static User SearchUser(string username) {
            List<User> users = GetUsers();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public static void UpdateUser(User user) {
            var index = GetUsers().FindIndex(u => u.Id == user.Id);
            List<User> users = GetUsers();
            users[index] = user;
            SaveUserFile(users);
        }

        public static void AddUser(User user) {
            List<User> users = GetUsers();
            user.Id = Guid.NewGuid();
            users.Add(user);
            SaveUserFile(users);
        }

        public static void DeleteUser(Guid id) {
            List<User> users = GetUsers();
            users.RemoveAll(u => u.Id == id);
            SaveUserFile(users);
        }
    }
}