using System.Linq.Expressions;
using Application.Contracts;
using Application.Contracts.Specification;

namespace Application.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public virtual Expression<Func<T, bool>> Criteria { get; set; } = _ => true;

    public virtual Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }

    public virtual Func<IQueryable<T>, IQueryable<T>>? Includes { get; set; }

    public int PageSize { get; set; } = 10;
    
    public int PageNumber { get; set; } = 1;
    
    public void ApplyOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    public void ApplyCriteria(Expression<Func<T, bool>> criteriaExpression)
    {
        Criteria = criteriaExpression;
    }
}

