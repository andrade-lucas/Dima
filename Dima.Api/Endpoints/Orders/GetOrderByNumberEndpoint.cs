using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.Api.Endpoints.Orders;

public class GetOrderByNumberEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{number}", HandleAsync)
            .WithName("Orders: Get by Number")
            .WithSummary("Obtém um pedido pelo número")
            .WithDescription("Obtém um pedido pelo número")
            .WithOrder(4)
            .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(IOrderHandler handler, ClaimsPrincipal user, [FromRoute] string number)
    {
        var request = new GetOrderByNumberRequest
        {
            Number = number,
            UserId = user.Identity!.Name ?? string.Empty
        };

        var result = await handler.GetByNumberAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
