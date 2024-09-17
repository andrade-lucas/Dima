using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;

namespace Dima.Api.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id}", HandleAsync)
            .WithName("Categories: Update")
            .WithSummary("Atualiza uma nova categoria")
            .WithOrder(3)
            .Produces<Response<Category?>>();
    }

    public static async Task<IResult> HandleAsync(long id, ICategoryHandler handler, UpdateCategoryRequest request)
    {
        request.Id = id;
        request.UserId = "test@balta.io";
        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(new Response<Category?>(result.Data))
            : TypedResults.BadRequest(new Response<Category?>(null, message: "Erro ao atualizar categoria"));
    }
}
