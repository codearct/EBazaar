namespace Ebazaar.Web.Services;

public interface ICatalogService
{
    Task<GetProductsResponse> GetProducts(int? PageNumber = 1, int? PageSize = 10);
    Task<GetProductByIdResponse> GetProductById(Guid id);
    Task<GetProductsByCategoryResponse> GetProductsByCategory(string category);
}
