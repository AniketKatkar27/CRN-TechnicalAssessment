using CRN.Application.DTOs;
using CRN.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;

namespace CRN.IntegrationTests.Products;

public class ProductsApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/v1/Products");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    private async Task AuthenticateAsync()
    {
        var loginRequest = new
        {
            UserName = "admin",
            Password = "Admin@12345"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/Auth/login",
            loginRequest);

        response.EnsureSuccessStatusCode();

        var authResponse =
            await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (authResponse is null)
        {
            throw new InvalidOperationException(
                "Authentication response was empty.");
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.AccessToken);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync(
            "/api/v1/Products/999999");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenAdminIsAuthenticated()
    {
        // Arrange
        await AuthenticateAsync();

        var request = new
        {
            ProductName = "Integration Test Product",
            CreatedBy = "admin"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/Products",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    private async Task AuthenticateAsUserAsync()
    {
        var loginRequest = new
        {
            UserName = "testuser",
            Password = "User@12345"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/Auth/login",
            loginRequest);

        response.EnsureSuccessStatusCode();

        var authResponse =
            await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (authResponse is null)
        {
            throw new InvalidOperationException(
                "Authentication response was empty.");
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.AccessToken);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        await AuthenticateAsUserAsync();

        var request = new
        {
            ProductName = "Unauthorized Integration Product",
            CreatedBy = "testuser"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/Products",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        await AuthenticateAsync();

        var request = new
        {
            ProductName = "",
            CreatedBy = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/Products",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Admin_ShouldBeAbleToUpdateAndDeleteProduct()
    {
        // Arrange
        await AuthenticateAsync();

        var createRequest = new
        {
            ProductName = "CRUD Integration Product",
            CreatedBy = "admin"
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/Products",
            createRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdProduct =
            await createResponse.Content
                .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(createdProduct);

        // Act - Update
        var updateRequest = new
        {
            ProductName = "Updated CRUD Product",
            ModifiedBy = "admin"
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/v1/Products/{createdProduct.Id}",
            updateRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        // Act - Delete
        var deleteResponse = await _client.DeleteAsync(
            $"/api/v1/Products/{createdProduct.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync(
            $"/api/v1/Products/{createdProduct.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnPagedResult()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync(
            "/api/v1/Products?pageNumber=1&pageSize=2");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<PagedResult<ProductResponse>>();

        Assert.NotNull(result);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.True(result.TotalCount >= 0);
    }
}