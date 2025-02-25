using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbSharedLibrary;

public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _mongoDbCollection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public Repository(IMongoDatabase database, string collectionName)
    {
        _mongoDbCollection = database.GetCollection<T>(collectionName);
    }
    
    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        return await _mongoDbCollection.Find(filter ?? _filterBuilder.Empty).ToListAsync();
    }
    
    public async Task<T> GetByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        return await _mongoDbCollection.Find(p => p.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<T> GetByIdAsync(Expression<Func<T, bool>> filter)
    {
        return await _mongoDbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _mongoDbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var filter = _filterBuilder.Eq(p => p.Id, entity.Id);
        await _mongoDbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = _filterBuilder.Eq(p => p.Id, objectId);
        await _mongoDbCollection.DeleteOneAsync(filter);
    }
}