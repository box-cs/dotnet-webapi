#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Users.Api.Entities;

namespace Users.Api.Repositories
{
    public class MongoDbReactionTimesRepository : IReactionTimesRepository
    {
        private readonly IMongoCollection<ReactionTime> reactionTimesCollection;
        private const string databaseName = "testdb";
        private const string collectionName = "reaction-times";

        public MongoDbReactionTimesRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            reactionTimesCollection = database.GetCollection<ReactionTime>(collectionName);
        }

        public async Task<ReactionTime> GetReactionTime(Guid id)
        {
            return await reactionTimesCollection.Find(Builders<ReactionTime>
                    .Filter.Eq(statistic => statistic.Id, id))
                .SingleOrDefaultAsync();
        }
        
        public async Task<IEnumerable<ReactionTime>> GetReactionTimesAsync()
        {
            return await reactionTimesCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task CreateReactionTime(ReactionTime statistic)
        {
            await reactionTimesCollection.InsertOneAsync(statistic);
        }
    }
}