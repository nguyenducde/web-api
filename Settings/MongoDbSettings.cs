namespace Catalog.Settings{
    public class MongoDBSettings{
        public string Host { get; set; }
        public int Port { get; set; }
        public string ConnectionString { get{
            return "mongodb+srv://ducdecoder:lalang@cluster0.tz8jqkl.mongodb.net/Catalog";

        } }

    }
}