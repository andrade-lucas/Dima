using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using System.Net.Http.Json;

namespace Dima.Web.Handlers;

public class ProductHandler(IHttpClientFactory httpClientFactory) : IProductHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<PagedResponse<List<Product>?>> GetAllAsync(GetAllProductsRequest request)
    {
        return await _client.GetFromJsonAsync<PagedResponse<List<Product>?>>($"/v1/products?pageNumber={request.PageNumber}&pageSize={request.PageSize}")
            ?? new PagedResponse<List<Product>?>(null, 400, "Erro ao obter produtos");
    }

    public async Task<Response<Product?>> GetBySlugAsync(GetProductBySlugRequest request)
    {
        return await _client.GetFromJsonAsync<Response<Product?>>($"/v1/products/{request.Slug}")
            ?? new Response<Product?>(null, 400, "Erro ao obter produto");
    }
}
