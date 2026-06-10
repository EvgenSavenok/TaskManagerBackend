using System.Linq.Expressions;
using MongoDB.Driver;
using NotificationsService.Application.Contracts.RepositoryContracts;

namespace NotificationsService.Infrastructure.Repositories;

public abstract class RepositoryBase<T>(
    IMongoDatabase database, 
    string collectionName) 
    : IRepositoryBase<T> where T : class
{
    public readonly IMongoCollection<T> _collection = database.GetCollection<T>(collectionName);

    public async Task<IEnumerable<T>> FindAll(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindByCondition(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(expression).ToListAsync(cancellationToken);
    }

    public async Task Create(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task Update(T entity, CancellationToken cancellationToken = default)
    {
        var idProperty = typeof(T).GetProperty("Id");

        var idValue = idProperty!.GetValue(entity);
        var filter = Builders<T>.Filter.Eq("Id", idValue);
        await _collection.ReplaceOneAsync(
            filter,
            entity, 
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task Delete(Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }
}