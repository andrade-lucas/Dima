using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;

namespace Dima.Api.Endpoints.Categories;

public class GetCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", HandleAsync)
            .WithName("Categories: Get By Id")
            .WithSummary("Obter categoria por Id")
            .WithOrder(1)
            .Produces<Response<Category?>>();
    }

    public static async Task<IResult> HandleAsync(long id, ICategoryHandler handler)
    {
        var request = new GetCategoryByIdRequest
        {
            Id = id,
            UserId = "test@balta.io"
        };
        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(new Response<Category?>(result.Data))
            : TypedResults.BadRequest(new Response<Category?>(null, message: "Erro ao atualizar categoria"));
    }
}
