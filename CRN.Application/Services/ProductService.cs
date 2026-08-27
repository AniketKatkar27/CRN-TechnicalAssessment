using CRN.Application.DTOs;
using CRN.Application.Interfaces;
using CRN.Domain.Entities;

namespace CRN.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var (products, totalCount) = await _productRepository.GetPagedAsync(pageNumber, pageSize);

        return new PagedResult<ProductResponse>
        {
            Items = products.Select(MapToResponse),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        return product is null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            ProductName = request.ProductName,
            CreatedBy = request.CreatedBy,
            CreatedOn = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(createdProduct);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);

        if (existingProduct is null)
        {
            return false;
        }

        existingProduct.ProductName = request.ProductName;
        existingProduct.ModifiedBy = request.ModifiedBy;
        existingProduct.ModifiedOn = DateTime.UtcNow;

        await _productRepository.UpdateAsync(existingProduct);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);

        if (existingProduct is null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(existingProduct);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            ProductName = product.ProductName,
            CreatedBy = product.CreatedBy,
            CreatedOn = product.CreatedOn,
            ModifiedBy = product.ModifiedBy,
            ModifiedOn = product.ModifiedOn
        };
    }
}