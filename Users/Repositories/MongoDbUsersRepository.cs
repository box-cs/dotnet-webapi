using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Users.Entities;

namespace Users.Repositories
{
    public class MongoDbUsersRepository :IUsersRepository
    {
        private readonly IMongoCollection<User> usersCollection;
        private const string databaseName = "monkeyBenchmark";
        private const string collectionName = "users";

        public MongoDbUsersRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            usersCollection = database.GetCollection<User>(collectionName);
        }
        public void CreateUser(User user)
        {
            usersCollection.InsertOne(user);
        }

        public void DeleteUser(Guid id)
        {
            usersCollection.DeleteOne(user => user.Id == id);
        }
        
        public User GetUser(Guid id)
        {
            return usersCollection.Find(Builders<User>.Filter.Eq(item => item.Id, id))
                .SingleOrDefault();
        }
        
        public IEnumerable<User> GetUsers()
        {
            return usersCollection.Find(new BsonDocument()).ToList();
        }
        
        public void UpdateUser(User user)
        {
            usersCollection.ReplaceOne(Builders<User>.Filter.Eq(existingUser => existingUser.Id, user.Id), user);
        }
    }
}