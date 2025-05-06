using System.Linq.Expressions;

namespace Application.Contracts.Specification;

public interface ISpecification<T> 
{
    Expression<Func<T, bool>> Criteria { get; set; }
    Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
    Func<IQueryable<T>, IQueryable<T>>? Includes { get; set; }
    int PageSize { get; }
    int PageNumber { get; }
}
