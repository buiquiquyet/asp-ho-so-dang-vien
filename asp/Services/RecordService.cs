using asp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
namespace asp.Respositories
{


    public class RecordService
    {
        private readonly IMongoCollection<Records> _collection;

        public RecordService(IOptions<MongoDbSetting> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionURI);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);
            _collection = database.GetCollection<Records>(typeof(Records).Name.ToLower());
        }

        public async Task<List<Records>> GetAllAsync(int skipAmount, int pageSize)
        {
            var sortDefinition = Builders<Records>.Sort.Descending(x => x.Id);

            return await _collection.Find(_ => true)
                                    .Skip(skipAmount)
                                    .Sort(sortDefinition)
                                    .Limit(pageSize)
                                    .ToListAsync();
        }
        public async Task<long> CountAsync()
        {
            return await _collection.CountDocumentsAsync(_ => true);
        }
        public async Task<Records?> GetByIdAsync(string id) =>
            await _collection.Find(Builders<Records>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync();
        /* public async Task<Records?> GetByIdRecordstAsync(string idRecord) =>
             await _collection.Find(Builders<Records>.Filter.Eq("id_khoa", idRecord)).FirstOrDefaultAsync();*/

        public async Task<String> CreateAsync(Records newEntity)
        {
            await _collection.InsertOneAsync(newEntity);

            return newEntity.Id;
        }

        public async Task UpdateAsync(string id, Records updatedEntity)
        {
            var filter = Builders<Records>.Filter.Eq("_id", ObjectId.Parse(id));
            await _collection.ReplaceOneAsync(filter, updatedEntity);
        }

        public async Task RemoveAsync(string id)
        {
            var filter = Builders<Records>.Filter.Eq("_id", ObjectId.Parse(id));
            await _collection.DeleteOneAsync(filter);
        }
        public async Task<long> DeleteByIdsAsync(List<string> ids)
        {
            var filter = Builders<Records>.Filter.In("_id", ids.Select(ObjectId.Parse));
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }


    }
}

