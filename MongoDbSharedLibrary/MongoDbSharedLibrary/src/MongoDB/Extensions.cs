using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDbSharedLibrary.Settings;

namespace MongoDbSharedLibrary.MongoDB;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var mongoDbSetting = configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>();
            var serviceSetting = configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
            var mongoClient = new MongoClient(mongoDbSetting.ConnectionString);
            return mongoClient.GetDatabase(serviceSetting.ServiceName);
        });
        return serviceCollection;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection serviceCollection,
        string collectionName) where T : IEntity
    {
        serviceCollection.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new Repository<T>(database, collectionName);
        });
        return serviceCollection;
    }
}