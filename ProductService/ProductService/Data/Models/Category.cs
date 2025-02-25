using MongoDB.Bson;
using MongoDbSharedLibrary;

namespace ProductService.Data.Models;

using MongoDB.Bson.Serialization.Attributes;
using System;

public class Category : IEntity
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("createdat")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("modifiedat")]
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
}
