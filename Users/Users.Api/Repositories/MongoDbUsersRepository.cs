#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Users.Api.Entities;

namespace Users.Api.Repositories
{
    public class MongoDbUsersRepository :IUsersRepository
    {
        private readonly IMongoCollection<User> usersCollection;
        private const string databaseName = "testdb";
        private const string collectionName = "users";

        public MongoDbUsersRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            usersCollection = database.GetCollection<User>(collectionName);
        }
        public async Task CreateUserAsync(User user)
        {
            await usersCollection.InsertOneAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await usersCollection.DeleteOneAsync(user => user.Id == id);
        }
        
        public async Task<User> GetUserAsync(Guid id)
        {
            return await usersCollection.Find(Builders<User>.Filter.Eq(user => user.Id, id))
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await usersCollection.Find(Builders<User>.Filter.Eq(user => user.Email, email))
                .SingleOrDefaultAsync();
        }
        
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await usersCollection.Find(new BsonDocument()).ToListAsync();
        }
        
        public async Task UpdateUserAsync(User user)
        {
            await usersCollection.ReplaceOneAsync(Builders<User>.Filter.Eq(existingUser => existingUser.Id, user.Id), user);
        }
    }
}