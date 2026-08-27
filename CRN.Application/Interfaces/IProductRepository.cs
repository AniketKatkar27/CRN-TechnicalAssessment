using CRN.Domain.Entities;

namespace CRN.Application.Interfaces;

public interface IProductRepository
{
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

    Task<Product?> GetByIdAsync(int id);

    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(Product product);
}