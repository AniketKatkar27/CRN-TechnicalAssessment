using CRN.Application.DTOs;

namespace CRN.Application.Interfaces;

/// <summary>
/// Defines application operations for managing products.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves a paginated list of products.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of products per page.</param>
    /// <returns>A paginated result containing products.</returns>
    Task<PagedResult<ProductResponse>> GetPagedAsync(int pageNumber, int pageSize);

    Task<ProductResponse?> GetByIdAsync(int id);

    Task<ProductResponse> CreateAsync(CreateProductRequest request);

    Task<bool> UpdateAsync(int id, UpdateProductRequest request);

    Task<bool> DeleteAsync(int id);
}