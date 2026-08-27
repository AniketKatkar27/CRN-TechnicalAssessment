using CRN.Application.DTOs;

namespace CRN.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductResponse>> GetPagedAsync(int pageNumber, int pageSize);

    Task<ProductResponse?> GetByIdAsync(int id);

    Task<ProductResponse> CreateAsync(CreateProductRequest request);

    Task<bool> UpdateAsync(int id, UpdateProductRequest request);

    Task<bool> DeleteAsync(int id);
}