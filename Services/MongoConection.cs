using MongoDB.Driver;
using MongoDB.Bson;

public class MongoConnection {
    private static MongoClient _client;
    private static readonly object _lock = new object();

    public static MongoClient GetClient() {
        if (_client == null) {
            lock (_lock) {
                if (_client == null) {
                    string connectionUri = "mongodb+srv://joacoferman:" + Environment.GetEnvironmentVariable("MongoDbPassword") + "@jferman.agip7.mongodb.net/?retryWrites=true&w=majority&appName=JFerman";
                    var settings = MongoClientSettings.FromConnectionString(connectionUri);
                    settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                    _client = new MongoClient(settings);
                    try {
                        var result = _client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                        Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
                    } catch (Exception ex) {
                        Console.WriteLine("MongoDB Connection Error: " + ex.Message);
                    }
                }
            }
        }
        return _client;
    }
}
