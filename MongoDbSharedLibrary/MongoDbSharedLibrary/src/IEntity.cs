using MongoDB.Bson;

namespace MongoDbSharedLibrary;

public interface IEntity
{
    ObjectId Id { get; set; }
}