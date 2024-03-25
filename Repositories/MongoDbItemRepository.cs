using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories{
    public class MongoDbItemRepository :IItemRepository{
       
       private const string databaseName= "Catalog";
       private const string collectionName = "items" ;
       private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

       private readonly IMongoCollection<Item> itemsCollection;
       
       public MongoDbItemRepository(IMongoClient mongoClient){

        IMongoDatabase database = mongoClient.GetDatabase(databaseName);
        itemsCollection = database.GetCollection<Item>(collectionName);

       }
       
        public async Task CreateItemAsync(Item item) {
            await itemsCollection.InsertOneAsync(item);
        }
        public async Task UpdateItemAsync(Item item){
            var filter = filterBuilder.Eq(existingItem => existingItem.Id,item.Id);
          await  itemsCollection.ReplaceOneAsync(filter,item);
        
        }
        public async Task DeleteItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id,id);
          await  itemsCollection.DeleteOneAsync(filter);
        }
        public async Task<Item> GetItemAsync(Guid id){
            var filter = filterBuilder.Eq(Item =>Item.Id,id);
          return await  itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync() {
           return  await itemsCollection.Find(new BsonDocument()).ToListAsync();  
        }
    }
}