using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface ICategoriesRepository : IRepositoryBase<Category>
{
    Task<Category?> GetCategoryById(Guid categoryId, bool trackChanges, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByName(string name, bool trackChanges, CancellationToken cancellationToken);
}
