using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.Api.Endpoints.Categories;

public class DeleteCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", HandleAsync)
            .WithName("Categories: Delete")
            .WithSummary("Deleta uma nova categoria")
            .WithOrder(4)
            .Produces<Response<Category?>>();
    }

    public static async Task<IResult> HandleAsync(ClaimsPrincipal user, long id, ICategoryHandler handler)
    {
        var request = new DeleteCategoryRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty,
        };
        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
