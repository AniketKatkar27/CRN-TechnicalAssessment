using CRN.Application.Interfaces;
using CRN.Domain.Entities;
using CRN.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRN.Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Products.AsNoTracking().OrderBy(p => p.Id);

        var totalCount = await query.CountAsync();

        var products = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (products, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
    }
}