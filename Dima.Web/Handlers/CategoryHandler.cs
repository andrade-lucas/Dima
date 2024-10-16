using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using System.Net.Http.Json;

namespace Dima.Web.Handlers;

public class CategoryHandler(IHttpClientFactory httpClientFactory) : ICategoryHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        var result = await _client.PostAsJsonAsync("/v1/categories", request);

        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
            ?? new Response<Category?>(null, 400, "Erro ao criar categoria");
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        var result = await _client.DeleteAsync($"/v1/categories/{request.Id}");

        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
            ?? new Response<Category?>(null, 400, "Erro ao exluir categoria");
    }

    public async Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request)
    {
        return await _client.GetFromJsonAsync<PagedResponse<List<Category>>>("/v1/categories")
            ?? new PagedResponse<List<Category>>(new List<Category>());
    }

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        return await _client.GetFromJsonAsync<Response<Category?>>($"/v1/categories/{request.Id}")
            ?? new Response<Category?>(null, 404, "Categoria não encontrada");
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        var result = await _client.PutAsJsonAsync($"/v1/categories/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
            ?? new Response<Category?>(null, 400, "Erro ao atualizar categoria");
    }
}
