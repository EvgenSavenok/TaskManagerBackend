using Application.Contracts.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class CategoriesRepository(ApplicationContext repositoryContext)
    : RepositoryBase<Category>(repositoryContext), ICategoriesRepository
{
    public async Task<Category?> GetCategoryById(
        Guid categoryId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        return await repositoryContext.Categories
            .Where(c => c.Id == categoryId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryByName(
        string name,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        return await repositoryContext.Categories
            .Where(c => c.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
