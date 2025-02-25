using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbSharedLibrary;

namespace ProductService.Data.Models;

public class Product : IEntity
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("categoryid")]
    public ObjectId CategoryId { get; set; } 

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("createdat")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("modifiedat")]
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
}