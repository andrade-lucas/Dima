using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dima.Api.Endpoints.Categories;

public class GetAllCategoriesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", HandleAsync)
            .WithName("Categories: Get All")
            .WithSummary("Obter todas as categorias de um usuário")
            .WithOrder(0)
            .Produces<PagedResponse<List<Category>?>>();
    }

    public static async Task<IResult> HandleAsync(
        ICategoryHandler handler, 
        [FromQuery]int pageNumber, 
        [FromQuery]int pageSize
    )
    {
        var request = new GetAllCategoriesRequest
        {
            UserId = "test@balta.io",
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(new PagedResponse<List<Category>?>(result.Data))
            : TypedResults.BadRequest(new Response<Category?>(null, message: "Erro ao atualizar categoria"));
    }
}
