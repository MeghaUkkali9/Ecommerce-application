using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbSharedLibrary;

namespace InventoryService.Data;

public class Inventory : IEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("productid")]
    public ObjectId ProductId { get; set; }
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}