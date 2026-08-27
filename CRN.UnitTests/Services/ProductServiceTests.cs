using CRN.Application.DTOs;
using CRN.Application.Interfaces;
using CRN.Application.Services;
using CRN.Domain.Entities;
using Moq;

namespace CRN.UnitTests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateProductAndSaveChanges()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var request = new CreateProductRequest
        {
            ProductName = "Test Laptop",
            CreatedBy = "TestUser"
        };

        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product product) =>
            {
                product.Id = 1;
                return product;
            });

        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Laptop", result.ProductName);
        Assert.Equal("TestUser", result.CreatedBy);

        repositoryMock.Verify(
            r => r.AddAsync(It.Is<Product>(p =>
                p.ProductName == "Test Laptop" &&
                p.CreatedBy == "TestUser")),
            Times.Once);

        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            ProductName = "Laptop",
            CreatedBy = "Admin",
            CreatedOn = DateTime.UtcNow
        };

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.ProductName);
        Assert.Equal("Admin", result.CreatedBy);

        repositoryMock.Verify(
            r => r.GetByIdAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);

        repositoryMock.Verify(
            r => r.GetByIdAsync(999),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            ProductName = "Old Laptop",
            CreatedBy = "Admin",
            CreatedOn = DateTime.UtcNow.AddDays(-1)
        };

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var request = new UpdateProductRequest
        {
            ProductName = "New Laptop",
            ModifiedBy = "Admin"
        };

        // Act
        var result = await service.UpdateAsync(1, request);

        // Assert
        Assert.True(result);

        Assert.Equal("New Laptop", product.ProductName);
        Assert.Equal("Admin", product.ModifiedBy);
        Assert.NotNull(product.ModifiedOn);

        repositoryMock.Verify(
            r => r.GetByIdAsync(1),
            Times.Once);

        repositoryMock.Verify(
            r => r.UpdateAsync(product),
            Times.Once);

        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var request = new UpdateProductRequest
        {
            ProductName = "New Laptop",
            ModifiedBy = "Admin"
        };

        // Act
        var result = await service.UpdateAsync(999, request);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            r => r.GetByIdAsync(999),
            Times.Once);

        repositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Product>()),
            Times.Never);

        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            ProductName = "Laptop",
            CreatedBy = "Admin",
            CreatedOn = DateTime.UtcNow
        };

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        repositoryMock
            .Setup(r => r.DeleteAsync(product))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        // Act
        var result = await service.DeleteAsync(1);

        // Assert
        Assert.True(result);

        repositoryMock.Verify(
            r => r.GetByIdAsync(1),
            Times.Once);

        repositoryMock.Verify(
            r => r.DeleteAsync(product),
            Times.Once);

        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            r => r.GetByIdAsync(999),
            Times.Once);

        repositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Product>()),
            Times.Never);

        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedProducts()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            ProductName = "Laptop",
            CreatedBy = "Admin",
            CreatedOn = DateTime.UtcNow
        },
        new()
        {
            Id = 2,
            ProductName = "Keyboard",
            CreatedBy = "Admin",
            CreatedOn = DateTime.UtcNow
        }
    };

        repositoryMock
            .Setup(r => r.GetPagedAsync(1, 2))
            .ReturnsAsync((products, 5));

        var service = new ProductService(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        // Act
        var result = await service.GetPagedAsync(1, 2);

        // Assert
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(3, result.TotalPages);

        Assert.Equal(2, result.Items.Count());

        var firstProduct = result.Items.First();

        Assert.Equal(1, firstProduct.Id);
        Assert.Equal("Laptop", firstProduct.ProductName);

        repositoryMock.Verify(
            r => r.GetPagedAsync(1, 2),
            Times.Once);

        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Never);
    }
}